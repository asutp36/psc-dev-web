using BotNotificationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

        [HttpGet("updates")]
        public async Task<IActionResult> GetUpdatesAsync()
        { 
            var response = await SendRequestAsync(HttpMethod.Get, "getUpdates", null);
            string data = await response.Content.ReadAsStringAsync();
            return Ok(data);
        }

        [HttpPost("message-group")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageWhattsAppModel message)
        {
            SendMessage m = new SendMessage()
            {
                chat_id = message.chatId,
                text = message.body
            };
            var response = await SendRequestAsync(HttpMethod.Post, "sendMessage", JsonConvert.SerializeObject(m));
            string data = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if(response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    BotResponse br = JsonConvert.DeserializeObject<BotResponse>(data);
                    if (br.description.Contains("chat not found"))
                        return NotFound();
                }
                _logger.LogError("Сообщение не отправлено.\n " + data + Environment.NewLine);
                return StatusCode(424, null);
            }
            return Ok(data);
        }

        private async Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, string uri, string content)
        {
            try
            {
                BotCredentials bot = new BotCredentials();
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri($"https://api.telegram.org/bot{bot.Token}/");

                HttpRequestMessage message = new HttpRequestMessage(method, uri);

                if(content != null)
                    message.Content = new StringContent(content, Encoding.UTF8, Application.Json);

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
