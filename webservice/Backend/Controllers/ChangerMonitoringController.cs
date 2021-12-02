using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ChangerState;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangerMonitoringController : ControllerBase
    {
        [SwaggerOperation(Summary = "Мониторинг разменника")]
        [SwaggerResponse(200, "Ок", Type = typeof(ChangerState))]
        [SwaggerResponse(424, "Проблема со связью", Type = typeof(Error))]
        [SwaggerResponse(500, "Внутренняя ошибка сервера", Type = typeof(Error))]
        [SwaggerResponse(500, "Проблема на микросервисе postrc", Type = typeof(Error))]
        [Authorize]
        [HttpGet("{changer}")]
        public IActionResult GetInfo(string changer)
        {
            try
            {
                Supplies.HttpResponse response = HttpSender.SendGet("http://194.87.98.177/postrc/api/changer/state/" + changer);

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        ChangerState state = JsonConvert.DeserializeObject<ChangerState>(response.ResultMessage);
                        return Ok(state);

                    case (System.Net.HttpStatusCode)424:
                        return StatusCode(424, new Error("Нет связи с разменником", "connection"));

                    case System.Net.HttpStatusCode.InternalServerError:
                        return StatusCode(503, new Error($"Проблема на сервисе postrc: {response.ResultMessage}", "sevice"));

                    default:
                        return StatusCode(424, new Error("Проблема со связью", "connection"));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}