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
        [Authorize]
        [HttpGet("chat")]
        public IActionResult GetChat()
        {
            Supplies.HttpResponse response = HttpSender.SendGet("");

            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                List<WhattsAppChatModel> chats = JsonConvert.DeserializeObject<List<WhattsAppChatModel>>(response.ResultMessage);

                return Ok(chats);
            }
            else
            {
                _logger.LogError("От сервиса hangfire пришёл ответ: " + response.ResultMessage + Environment.NewLine);
                return StatusCode(424, new Error("Не удалось получить список чатов", "dependency"));
            }

            
        }
    }
}
