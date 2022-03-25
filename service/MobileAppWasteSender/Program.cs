using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using MobileAppWasteSender.Models;
using MobileAppWasteSender.Models.WashCompany;
using Newtonsoft.Json;
using NLog;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MobileAppWasteSender
{
    class Program
    {
        private static WashCompanyContext _context;
        private static HttpClient _httpClient;

        static async Task Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                  .WriteTo.File($"Logs/{DateTime.Now:yyyy-MM-dd}.log")
                  .CreateLogger();

                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri("http://188.225.79.69/api/externaldb/");
                _httpClient.DefaultRequestHeaders.Add(
                  HeaderNames.Accept, "application/json");
                Log.Logger.Debug("Работает");
                DateTime curDate = DateTime.Now.Date;

                while (true)
                {
                    if (curDate != DateTime.Now.Date)
                    {
                        Log.CloseAndFlush();

                        Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Debug()
                            .WriteTo.File($"Logs/{DateTime.Now:yyyy-MM-dd}.log")
                            .CreateLogger();

                        curDate = DateTime.Now.Date;
                    }

                    _context = new WashCompanyContext();
                    List<MobileSending> mobileSendings = GetUnsentWastes();
                    if (mobileSendings.Count > 0)
                    {
                        foreach (MobileSending ms in mobileSendings)
                        {
                            try
                            {
                                using var response = await SendWaste(ms);

                                ms.StatusCode = (int)response.StatusCode;
                                ms.ResultMessage = await response.Content.ReadAsStringAsync();
                            }
                            catch (Exception e)
                            {
                                ms.StatusCode = 0;
                                ms.ResultMessage = e.Message;
                                Log.Logger.Error("Ошибка при отправке. " + e.Message + Environment.NewLine);
                            }
                            finally
                            {
                                Log.Logger.Debug("Отправлена запись id=" + ms.IdmobileSending);
                            }

                            try
                            {
                                await UpdateWaste(ms);
                                Log.Logger.Debug("Обновлена запись id=" + ms.IdmobileSending + Environment.NewLine);
                            }
                            catch (DbUpdateException e)
                            {
                                Log.Logger.Error("Ошибка при обновлении бд: " + e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine);
                            }
                            catch (SqlException e)
                            {
                                Log.Logger.Error("Ошибка при выполнении запроса к бд: " + e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine);
                            }
                        }
                    }

                    List<UnstoppedSessionModel> unstopped = GetUnstoppedSessions();
                    if (unstopped.Count > 0)
                        Notification.SendUnstoppedSessions(unstopped);

                    await _context.DisposeAsync();

                    await Task.Delay(5000);
                }
            }
            catch(Exception e)
            {
                Log.Logger.Error("Перехвачена общая ошибка. " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                Notification.SendCritical(e);
            }
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
            MobileAppWasteModel model = new MobileAppWasteModel()
            {
                wash_id = await GetWashCode(waste.Idpost),
                time_send = waste.DtimeEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                operation_time = waste.DtimeEnd.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                card = GetCardNum(waste.Idcard),
                value = waste.Amount.Value
            };
            var data = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, Application.Json);

            var httpResponseMessage = await _httpClient.PostAsync("set-waste", data);

            return httpResponseMessage;
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
            return _context.MobileSendings.Where(s => (s.StatusCode == null || s.ResultMessage == null) && EF.Functions.DateDiffMinute(s.DtimeStart, DateTime.Now) > 30)
                                          .Include(c => c.IdcardNavigation)
                                          .Include(p => p.IdpostNavigation).ThenInclude(d => d.IddeviceNavigation)
                                          .Select(m => new UnstoppedSessionModel
                                          {
                                              CardNum = m.IdcardNavigation.CardNum,
                                              Post = m.IdpostNavigation.IddeviceNavigation.Code,
                                              DTimeStart = m.DtimeStart
                                          })
                                          .ToList();
            
        }
    }
}
