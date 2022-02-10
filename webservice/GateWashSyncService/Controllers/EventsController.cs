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

        [HttpPost("payout")]
        public async Task<IActionResult> PostEventPayout(EventPayoutBindingModel epayout)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

                if (!sqlHelper.IsDeviceExsists(epayout.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {epayout.deviceCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {epayout.deviceCode}" });
                }
                if (!sqlHelper.IsEventKindExsists(epayout.eventKindCode))
                {
                    _logger.LogError($"Не найден тип события {epayout.eventKindCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип события {epayout.eventKindCode}" });
                }
                if (!sqlHelper.IsPaySessionExsists(epayout.deviceCode, epayout.idSessionOnPost))
                {
                    _logger.LogError($"Не найдена сессия на посту {epayout.deviceCode} с id={epayout.idSessionOnPost}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия на посту {epayout.deviceCode} с id={epayout.idSessionOnPost}" });
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
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

                if (!sqlHelper.IsDeviceExsists(eincr.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {eincr.deviceCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {eincr.deviceCode}" });
                }
                if (!sqlHelper.IsEventKindExsists(eincr.eventKindCode))
                {
                    _logger.LogError($"Не найден тип события {eincr.eventKindCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип события {eincr.eventKindCode}" });
                }
                if (!sqlHelper.IsPaySessionExsists(eincr.deviceCode, eincr.idSessionOnPost))
                {
                    _logger.LogError($"Не найдена сессия на посту {eincr.deviceCode} с id={eincr.idSessionOnPost}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия на посту {eincr.deviceCode} с id={eincr.idSessionOnPost}" });
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

                    default:
                        _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpexted", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }

        [HttpPost("collect")]
        public async Task<IActionResult> PostEventCollect(EventCollectBindingModel ecollect)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

                if (!sqlHelper.IsDeviceExsists(ecollect.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {ecollect.deviceCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {ecollect.deviceCode}" });
                }
                if (!sqlHelper.IsEventKindExsists(ecollect.eventKindCode))
                {
                    _logger.LogError($"Не найден тип события {ecollect.eventKindCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип события {ecollect.eventKindCode}" });
                }
                if (!sqlHelper.IsPaySessionExsists(ecollect.deviceCode, ecollect.idSessionOnPost))
                {
                    _logger.LogError($"Не найдена сессия на посту {ecollect.deviceCode} с id={ecollect.idSessionOnPost}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия на посту {ecollect.deviceCode} с id={ecollect.idSessionOnPost}" });
                }

                int id = await sqlHelper.WriteEventCollectAsync(ecollect);

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
