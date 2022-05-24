using BotNotificationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotNotificationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private readonly ILogger<NotifyController> _logger;
        public NotifyController(ILogger<NotifyController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoAsync()
        {
            var response = await SendRequestAsync();
            string data = await response.Content.ReadAsStringAsync();
            return Ok(data);
        }

        private async Task<HttpResponseMessage> SendRequestAsync()
        {
            try
            {
                BotCredentials bot = new BotCredentials();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://api.telegram.org/bot{bot.Token}/");

                HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "getMe");

                var result = await client.SendAsync(message);

                return result;
            }
            catch(HttpRequestException e)
            {
                HttpResponseMessage failedMessage = new HttpResponseMessage();

                return null;
            }
        }
    }
}
