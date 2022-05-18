using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GateWashSyncService.Controllers.BindingModels;
using GateWashSyncService.Controllers.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GateWashSyncService.Models.GateWash;
using Newtonsoft.Json;

namespace GateWashSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionsController : ControllerBase
    {
        private readonly ILogger<SessionsController> _logger;
        private readonly GateWashDbContext _model;

        public SessionsController(ILogger<SessionsController> logger, GateWashDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        [HttpPost]
        public async Task<IActionResult> PostSessionAsync(SessionBindingModel session)
        {
            _logger.LogInformation("Запуск с параметрами: " + JsonConvert.SerializeObject(session));
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper(_model);
                if (!sqlHelper.IsCardExsists(session.cardNum))
                {
                    _logger.LogError($"Карта {session.cardNum} не найдена");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Карта {session.cardNum} не найдена" });
                }
                if (!sqlHelper.IsProgramExsists(session.functionCode))
                {
                    _logger.LogError($"Функция {session.functionCode} не найдена");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Функция {session.functionCode} не найдена" });
                }
                if (sqlHelper.IsSessionExsists(session.cardNum, session.uuid))
                {
                    _logger.LogError($"Сессия с картой {session.cardNum} и uuid {session.uuid} уже записана");
                    return Conflict(new Error() { errorCode = "badvalue", errorMessage = $"Сессия с картой {session.cardNum} и uuid {session.uuid} уже записана" });
                }

                int id = await sqlHelper.WriteSessionAsync(session);

                Response.Headers.Add("ServerID", id.ToString());
                return Created(id.ToString(), null);
            }
            catch(Exception e)
            {
                switch (e.Message)
                {
                    case "command":
                        _logger.LogError("Произошла ошибка при выполнении запроса к бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при выполнении запроса к бд" });
                    case "db":
                        _logger.LogError("Произошла ошибка при обновлении данных бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при обновлении данных бд" });
                    default:
                        _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpexted", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PostPaySessionAsync(PaySessionBindingModel psession)
        {
            _logger.LogInformation("Запуск с параметрами: " + JsonConvert.SerializeObject(psession));
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper(_model);
                if (!sqlHelper.IsProgramExsists(psession.functionCode))
                {
                    _logger.LogError($"Функция {psession.functionCode} не найдена");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Функция {psession.functionCode} не найдена" });
                }
                if (!sqlHelper.IsDeviceExsists(psession.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {psession.deviceCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {psession.deviceCode}" });
                }
                if (sqlHelper.IsPaySessionExsists(psession.deviceCode, psession.idSessionOnPost))
                {
                    _logger.LogError($"Сессия на посте {psession.deviceCode} с id={psession.idSessionOnPost} уже записана");
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
