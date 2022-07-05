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
    public class PostRCCallerService
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
            HttpResponseMessage response = await _httpSender.PostJsonAsync("", JsonConvert.SerializeObject(parameters));
           
            try 
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                string content = await response.Content.ReadAsStringAsync();
                _logger.LogError($"| PostRCCallerService.StartPostAsync | От PostRC ответ не ок. StatusCode: {response.StatusCode}, Content: {content}");
            }

            return response;
        }
    }
}
