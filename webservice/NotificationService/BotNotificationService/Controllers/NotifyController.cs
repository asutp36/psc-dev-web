using BotNotificationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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

        [HttpPost("update")]
        public async Task<IActionResult> PostUpdate([FromBody]Update update)
        {
            _logger.LogInformation("Получено обновление: " + JsonConvert.SerializeObject(update));


            if (update.message != null && 
                update.message.from.Id == 134083432 && 
                update.message.entities.Any(e => e.type == "bot_command") && 
                update.message.text.Split('@')[0].StartsWith("/get_chat_info"))
            {
                _logger.LogInformation($"Отправляю chat_id={update.message.chat.id} группы {update.message.chat.title}");

                SendMessage(new SendMessageWhattsAppModel { chatId = 134083432.ToString(), body = $"Группа {update.message.chat.title}: chat_id={update.message.chat.id}" });

                return Ok();
            }

            if(update.message != null && update.message.chat != null  && update.message.group_chat_created)
            {
                _logger.LogInformation($"Создана группа {update.message.chat.title} chat_id={update.message.chat.id}");

                SendMessage(new SendMessageWhattsAppModel { chatId = update.message.chat.id.ToString(), body = $"chat_id={update.message.chat.id}" });
            }

            return Ok();
        }

        [HttpGet("updates")]
        public async Task<IActionResult> GetUpdatesAsync()
        { 
            var response = await SendRequestAsync(HttpMethod.Get, "getUpdates", null);
            string data = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<UpdateResult>(data);

            return Ok(result);
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
                    {
                        _logger.LogError($"Чат {message.chatId} не найден");
                        return NotFound();
                    }
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
