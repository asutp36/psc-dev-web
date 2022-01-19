using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostSyncService.Controllers.BindingModels;
using PostSyncService.Controllers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostEvent(EventBindingModel evnt)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

                if (!sqlHelper.IsSessionExsists(evnt.cardNum, evnt.uuid))
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия для карты {evnt.cardNum} с uuid = {evnt.uuid}" });
                if (!sqlHelper.IsDeviceExsists(evnt.deviceCode))
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {evnt.deviceCode}" });
                if (!sqlHelper.IsEventKindExsists(evnt.eventKindCode))
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип event {evnt.eventKindCode}" });

                int id = await sqlHelper.WriteEventAsync(evnt);

                Response.Headers.Add("ServerID", id.ToString());
                return Created(id.ToString(), null);
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "command":
                        _logger.LogError("Произошла ошибка при выполнении запроса к бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при выполнении запроса к бд" });

                    default:
                        _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpexted", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }
    }
}
