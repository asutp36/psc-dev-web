using MangoAPIService.Helpers;
using MangoAPIService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MangoAPIService.Services
{
    public class LoyalityService : ILoyalityServiceCaller
    {
        private readonly HttpSender _httpSender;
        private readonly ILogger _logger;

        public LoyalityService(string serviceBaseUrl, ILogger logger)
        {
            _httpSender = new HttpSender(serviceBaseUrl);
            _logger = logger;
        }

        public async void HandleNewCallAsync(CallCacheModel call)
        {
            HttpResponseMessage response = await _httpSender.PostJsonAsync("api/calls", JsonConvert.SerializeObject(call));

            try 
            {
                response.EnsureSuccessStatusCode();
            }
            catch(HttpRequestException e)
            {
                string content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"| LoyalityService.HandleNewCall | Ответ от сервиса лояльности не ок. StatusCode: {response.StatusCode}, Message: {content}, Exception: {e.Message}");
            }
        }
    }
}
