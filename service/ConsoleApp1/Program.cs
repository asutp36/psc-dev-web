using ConsoleApp1.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Serilog;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Models.WashCompany;
using System.Linq;
using System.Net;

namespace ConsoleApp1
{
    class Program
    {
        private static IConfiguration _config;
        private static WashCompanyContext _context;
        private static HttpClient _httpClient;
        private static int _updatePeriod;
        private static int _cacheExpiringTime;
        private static int _oldSessionTime;
        private static IMemoryCache _cache;
        private static int _resultMessageLength;
        private static string _mobileAppBaseUrl;
        private static string _notifyServiceBaseUrl;
        private static string _telegramChatID;

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello, World!");
                Config();
                Console.WriteLine("2");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.GetType()}: {e.Message}");
                Log.Logger.Error("Перехвачена общая ошибка. " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                Notification.SendCritical(e);
            }
        }

        private static void Config()
        {
            //_config = new ConfigurationBuilder()
            //                 .AddJsonFile("appsettings.json")
            //                 .Build();
            _updatePeriod = 60;// int.Parse(_config.GetSection("UpdatePeriod").Value);
            _oldSessionTime = 60;// int.Parse(_config.GetSection("OldSessionTime").Value);
            _resultMessageLength = 80;//int.Parse(_config.GetSection("ResultMessageLength").Value);
            _mobileAppBaseUrl = @"http://188.225.79.69/api/externaldb/";//_config.GetSection("MobileAppBaseUrl").Value;
            _notifyServiceBaseUrl = @"https://cwmon.ru/notify/";//_config.GetSection("NotifyServiceBaseUrl").Value;
            _telegramChatID = "-650370220";//_config.GetSection("TelegramChatID").Value;


            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.File($"Logs/{DateTime.Now:yyyy-MM-dd}.log")
              .CreateLogger();

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_mobileAppBaseUrl);
            _httpClient.DefaultRequestHeaders.Add(
              HeaderNames.Accept, "application/json");

            _cache = new MemoryCache(new MemoryCacheOptions());
            _cacheExpiringTime = int.Parse(_config.GetSection("CacheExpiringTime").Value);
        }

        private static List<MobileSending> GetUnsentWastes()
        {
            return _context.MobileSendings.Where(s => (HttpStatusCode)s.StatusCode != HttpStatusCode.OK && s.DtimeEnd != null && s.Amount != null).ToList();
        }

        private static async Task UpdateWaste(MobileSending waste)
        {
            _context.MobileSendings.Update(waste);
            await _context.SaveChangesAsync();
        }

        private static async Task<HttpResponseMessage> SendWaste(MobileSending waste)
        {
            try
            {
                MobileAppWasteModel model = new MobileAppWasteModel()
                {
                    wash_id = await GetWashCode(waste.Idpost),
                    time_send = waste.DtimeEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    operation_time = waste.DtimeEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                    card = GetCardNum(waste.Idcard),
                    value = waste.Amount.Value,
                    guid = Guid.Parse(waste.Guid)
                };
                var data = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, Application.Json);

                var httpResponseMessage = await _httpClient.PostAsync("set-waste", data);

                return httpResponseMessage;
            }
            catch (HttpRequestException e)
            {
                Log.Logger.Information($"Ошибка запроса: {e.Message}");
                return null;
            }
        }

        private static async Task<string> GetWashCode(int idPost)
        {
            string deviceCode = _context.Posts.Include(p => p.IddeviceNavigation).Where(p => p.Idpost == idPost).FirstOrDefault().IddeviceNavigation.Code;

            await _context.Database.OpenConnectionAsync();
            var connection = _context.Database.GetDbConnection();
            var command = connection.CreateCommand();
            command.CommandText = "select " +
                        "replace(w.Code, 'М', 'm')" +
                        "from " +
                        "Device d " +
                        "join Posts p on p.IDDevice = d.IDDevice " +
                        "join Wash w on w.IDWash = p.IDWash " +
                        "where 1 = 1 " +
                        $"and d.Code = '{deviceCode}'";

            string washCode = command.ExecuteScalar().ToString();

            await _context.Database.CloseConnectionAsync();

            return washCode;
        }

        private static string GetCardNum(int idCard)
        {
            return _context.Cards.Find(idCard).CardNum;
        }

        private static List<UnstoppedSessionModel> GetUnstoppedSessions()
        {
            return _context.MobileSendings.Where(s => (s.StatusCode == null || s.ResultMessage == null) && EF.Functions.DateDiffMinute(s.DtimeStart, DateTime.Now) > _oldSessionTime)
                                          .Include(c => c.IdcardNavigation)
                                          .Include(p => p.IdpostNavigation).ThenInclude(d => d.IddeviceNavigation)
                                          .Select(m => new UnstoppedSessionModel
                                          {
                                              CardNum = m.IdcardNavigation.CardNum,
                                              Post = m.IdpostNavigation.IddeviceNavigation.Code,
                                              DTimeStart = m.DtimeStart,
                                              Guid = m.Guid
                                          })
                                          .ToList();

        }
    }
}
