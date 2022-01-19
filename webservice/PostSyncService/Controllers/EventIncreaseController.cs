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
    public class EventIncreaseController : ControllerBase
    {
        private readonly ILogger<EventIncreaseController> _logger;

        public EventIncreaseController(ILogger<EventIncreaseController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostEventIncrease(EventIncreaseBindingModel eincr)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

                if (!sqlHelper.IsSessionExsists(eincr.cardNum, eincr.uuid))
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия для карты {eincr.cardNum} с uuid = {eincr.uuid}" });
                if (!sqlHelper.IsDeviceExsists(eincr.deviceCode))
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {eincr.deviceCode}" });

                int id = await sqlHelper.WriteEventIncreaseAsync(eincr);

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
