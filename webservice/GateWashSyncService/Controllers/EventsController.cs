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
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly GateWashDbContext _model;

        public EventsController(ILogger<EventsController> logger, GateWashDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        [HttpPost]
        public async Task<IActionResult> PostEvent(EventBindingModel evnt)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper(_model);

                if (!sqlHelper.IsSessionExsists(evnt.cardNum, evnt.uuid))
                {
                    _logger.LogError($"Не найдена сессия для карты {evnt.cardNum} с uuid = {evnt.uuid}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия для карты {evnt.cardNum} с uuid = {evnt.uuid}" });
                }
                if (!sqlHelper.IsDeviceExsists(evnt.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {evnt.deviceCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {evnt.deviceCode}" });
                }
                if (!sqlHelper.IsEventKindExsists(evnt.eventKindCode))
                {
                    _logger.LogError($"Не найден тип event {evnt.eventKindCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип event {evnt.eventKindCode}" });
                }

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
                    case "db":
                        _logger.LogError("Произошла ошибка при обновлении бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при обновлении бд" });
                    default:
                        _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpexted", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }

        [HttpPost("payout")]
        public async Task<IActionResult> PostEventPayout(EventPayoutBindingModel epayout)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper(_model);

                if (!sqlHelper.IsDeviceExsists(epayout.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {epayout.deviceCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {epayout.deviceCode}" });
                }
                if(sqlHelper.IsEventPayoutExists(epayout.deviceCode, epayout.idEventOnPost))
                {
                    _logger.LogError($"Событие с id={epayout.idEventOnPost} на посту {epayout.deviceCode} уже записано");
                    return Conflict(new Error() { errorCode = "badvalue", errorMessage = $"Событие с id={epayout.idEventOnPost} на посту {epayout.deviceCode} уже записано" });
                }
                if (!sqlHelper.IsEventKindExsists(epayout.eventKindCode))
                {
                    _logger.LogError($"Не найден тип события {epayout.eventKindCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип события {epayout.eventKindCode}" });
                }
                if (!sqlHelper.IsPaySessionExsists(epayout.deviceCode, epayout.idSessionOnPost))
                {
                    _logger.LogError($"Не найдена сессия на посту {epayout.deviceCode} с id={epayout.idSessionOnPost}");
                    return StatusCode(406, new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия на посту {epayout.deviceCode} с id={epayout.idSessionOnPost}" });
                }

                int id = await sqlHelper.WriteEventPayoutAsync(epayout);

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
                        _logger.LogError("Произошла ошибка при обновлении бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при обновлении бд" });
                    default:
                        _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpexted", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }

        [HttpPost("increase")]
        public async Task<IActionResult> PostEventIncrease(EventIncreaseBindingModel eincr)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper(_model);

                if (!sqlHelper.IsDeviceExsists(eincr.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {eincr.deviceCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {eincr.deviceCode}" });
                }
                if (sqlHelper.IsEventIncreaseExists(eincr.deviceCode, eincr.idEventOnPost))
                {
                    _logger.LogError($"Событие с id={eincr.idEventOnPost} на посту {eincr.deviceCode} уже записано");
                    return Conflict(new Error() { errorCode = "badvalue", errorMessage = $"Событие с id={eincr.idEventOnPost} на посту {eincr.deviceCode} уже записано" });
                }
                if (!sqlHelper.IsEventKindExsists(eincr.eventKindCode))
                {
                    _logger.LogError($"Не найден тип события {eincr.eventKindCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип события {eincr.eventKindCode}" });
                }
                if (!sqlHelper.IsPaySessionExsists(eincr.deviceCode, eincr.idSessionOnPost))
                {
                    _logger.LogError($"Не найдена сессия на посту {eincr.deviceCode} с id={eincr.idSessionOnPost}");
                    return StatusCode(406, new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия на посту {eincr.deviceCode} с id={eincr.idSessionOnPost}" });
                }

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
                    case "db":
                        _logger.LogError("Произошла ошибка при обновлении бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при обновлении бд" });
                    default:
                        _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpexted", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }
    }
}
