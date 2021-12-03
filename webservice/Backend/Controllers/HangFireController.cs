using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HangFireController : ControllerBase
    {
        private readonly ILogger<HangFireController> _logger;

        public HangFireController(ILogger<HangFireController> logger)
        {
            _logger = logger;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить диалоги из WhattsApp API")]
        [SwaggerResponse(200, Type = typeof(List<WhattsAppChatModel>))]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Не удалось получить список чатов из сервиса HangFire")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpGet("chat")]
        public IActionResult GetChat()
        {
            try
            {
                Supplies.HttpResponse response = HttpSender.SendGet("https://cwmon.ru/hangfire/api/Chats");

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<WhattsAppChatModel> chats = JsonConvert.DeserializeObject<List<WhattsAppChatModel>>(response.ResultMessage);

                    return Ok(chats);
                }
                else
                {
                    _logger.LogError("От сервиса hangfire пришёл ответ: " + response.ResultMessage + Environment.NewLine);
                    return StatusCode(424, new Error() { errorType = "service", alert = "Не удалось получить список чатов", errorCode = "Ошибка сервиса", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }
    }
}
