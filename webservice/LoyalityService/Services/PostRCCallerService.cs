using LoyalityService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace LoyalityService.Services
{
    public class PostRCCallerService : IPostRCCaller
    { 
        private readonly HttpSender _httpSender;
        private readonly ILogger _logger;

        public PostRCCallerService(ILogger logger, string postRCUrl)
        {
            _logger = logger;
            _httpSender = new HttpSender(postRCUrl);
        }

        public async Task<HttpResponseMessage> StartPostAsync(StartPostParameters parameters)
        {
            try 
            {
                _logger.LogInformation($"| PostRCCallerService.StartPostAsync | Запускается пост {parameters.DeviceCode} пользователем {parameters.ClientPhone} со скидкой {parameters.DiscountPercent}% или {parameters.DiscountRub}руб");
                HttpResponseMessage response = await _httpSender.PostJsonAsync("api/state/start", JsonConvert.SerializeObject(parameters));

                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
                else
                {
                    _logger.LogError($"| PostRCCallerService.StartPostAsync | Ответ от PostRC не ок. StatusCode: {response.StatusCode}, Message: {content}");
                    return null;
                }
            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"| PostRCCallerService.StartPostAsync | Нет связи с PostRC. HttpRequestException: {e.Message}");
                return null;
            }
        }
    }
}
