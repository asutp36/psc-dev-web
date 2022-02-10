using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GateWashSyncService.Controllers.BindingModels;
using GateWashSyncService.Controllers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(ILogger<SessionsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostSessionAsync(SessionBindingModel session)
        {
            GateWashSqlHelper sqlHelper = new GateWashSqlHelper();
            if (!sqlHelper.IsCardExsists(session.cardNum))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Карта {session.cardNum} не найдена" });
            if (!sqlHelper.IsFunctionExsists(session.functionCode))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Функция {session.functionCode} не найдена" });
            if (sqlHelper.IsSessionExsists(session.cardNum, session.uuid))
                return Conflict(new Error() { errorCode = "badvalue", errorMessage = $"Сессия с картой {session.cardNum} и uuid {session.uuid} уже записана" });

            int id = await sqlHelper.WriteSessionAsync(session);

            Response.Headers.Add("ServerID", id.ToString());
            return Created(id.ToString(), null);
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PostPaySessionAsync(PaySessionBindingModel psession)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();
                if (!sqlHelper.IsFunctionExsists(psession.functionCode))
                {
                    _logger.LogError($"Функция {psession.functionCode} не найдена" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Функция {psession.functionCode} не найдена" });
                }
                if (!sqlHelper.IsDeviceExsists(psession.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {psession.deviceCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {psession.deviceCode}" });
                }
                if (sqlHelper.IsPaySessionExsists(psession.deviceCode, psession.idSessionOnPost))
                {
                    _logger.LogError($"Сессия на посте {psession.deviceCode} с id={psession.idSessionOnPost} уже записана" + Environment.NewLine);
                    return Conflict(new Error() { errorCode = "badvalue", errorMessage = $"Сессия на посте {psession.deviceCode} с id={psession.idSessionOnPost} уже записана" });
                }

                int id = await sqlHelper.WritePaySessionAsync(psession);

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

                    case "db":
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
