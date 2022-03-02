using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using PostBackgroundServices.Helpers;
using PostBackgroundServices.Models.WashCompany;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PostBackgroundServices
{
    class MobileAppWasteSenderService : BackgroundService
    {
        private readonly ILogger<MobileAppWasteSenderService> _logger;
        private readonly WashCompanyContext _context;
        private readonly HttpClient _httpClient;

        public MobileAppWasteSenderService(ILogger<MobileAppWasteSenderService> logger, WashCompanyContext context, HttpClient httpClient)
        {
            _logger = logger;
            _context = context;

            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://ptsv2.com/t/zuicy-1646128586/");
            _httpClient.DefaultRequestHeaders.Add(
                HeaderNames.Accept, "application/json");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                List<MobileSending> mobileSendings = SqlHelper.GetUnsentWastes(_context);
                if (mobileSendings.Count > 0) 
                {
                    foreach(MobileSending ms in mobileSendings)
                    {
                        try
                        {
                            using var response = await SendWaste(ms);

                            ms.StatusCode = (int)response.StatusCode;
                            ms.ResultMessage = await response.Content.ReadAsStringAsync(stoppingToken);
                        }
                        catch(HttpRequestException e)
                        {
                            _logger.LogError(e.Message + Environment.NewLine);
                            ms.StatusCode = (int)e.StatusCode;
                            ms.ResultMessage = e.Message;
                        }
                        catch(Exception e)
                        {
                            _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        }

                        try
                        {
                            await SqlHelper.UpdateWaste(_context, ms);
                        }
                        catch (DbUpdateException e)
                        {
                            _logger.LogError("Ошибка при обновлении бд: " + e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine);
                        }
                        catch (SqlException e)
                        {
                            _logger.LogError("Ошибка при воплнении запроса к бд: " + e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine);
                        }
                    }
                }

                await Task.Delay(1000, stoppingToken);
            }
        }

        private async Task<HttpResponseMessage> SendWaste(MobileSending waste)
        {
            var data = new StringContent(JsonConvert.SerializeObject(waste), Encoding.UTF8, Application.Json);
            
            var httpResponseMessage = await _httpClient.PostAsync("post", data);

            return httpResponseMessage.EnsureSuccessStatusCode();
        }
    }
}
