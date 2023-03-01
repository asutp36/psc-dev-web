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
using GateWashSyncService.Services;
using GateWashSyncService.Exceptions;
using System.Net;

namespace GateWashSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly GateWashDbContext _model;
        EventIncreaseService _eventIncreaseService;

        public EventsController(ILogger<EventsController> logger, GateWashDbContext model, EventIncreaseService eventIncreaseService)
        {
            _logger = logger;
            _model = model;
            _eventIncreaseService = eventIncreaseService;
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
                int id = await _eventIncreaseService.InsertAsync(eincr);

                Response.Headers.Add("ServerID", id.ToString());
                return Created(id.ToString(), null);
            }
            catch (CustomStatusCodeException e) 
            {
                return StatusCode((int)e.StatusCode, e.Message);
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
