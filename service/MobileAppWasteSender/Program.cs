using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
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
        //private static ILogger _logger;

        static async Task Main(string[] args)
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                  .WriteTo.File($"Logs/{DateTime.Now:yyyy-MM-dd}.log")
                  .CreateLogger();

                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri("https://ptsv2.com/t/zuicy-1646128586/");
                _httpClient.DefaultRequestHeaders.Add(
                  HeaderNames.Accept, "application/json");
                Log.Logger.Debug("Работает");
                while (true)
                {
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

                    await _context.DisposeAsync();

                    await Task.Delay(10000);
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
            var data = new StringContent(JsonConvert.SerializeObject(waste), Encoding.UTF8, Application.Json);

            var httpResponseMessage = await _httpClient.PostAsync("post", data);

            return httpResponseMessage.EnsureSuccessStatusCode();
        }
    }
}
