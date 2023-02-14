using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostSynchronizationService.Controllers.Helpers;
using PostSynchronizationService.Controllers.Models;
using PostSynchronizationService.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace PostSynchronizationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly ModelDbContext _dbContext;
        public EventsController(ILogger<EventsController> logger, ModelDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost("increase")]
        public async Task<IActionResult> PostIncreaseAsync([FromBody] EventIncreaseModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Модель не прошла валидацию: " + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return BadRequest(new Error() { errorCode = "badvalue", errorMessage = "Модель не прошла валидацию" });
                }

                if (!SqlHelper.IsDeviceExists(_dbContext, model.device))
                {
                    //_logger.LogError($"Не удалось найти девайс {model.device}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {model.device}" });
                }

                if (!SqlHelper.IsEventKindExists(_dbContext, model.eventKindCode))
                {
                    //_logger.LogError($"Не удалось найти тип ивента {model.eventKindCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип ивента {model.eventKindCode}" });
                }

                int idEvent = await SqlHelper.WriteEventIncreaseAsync(_dbContext, model);
                Response.Headers.Add("ServerID", idEvent.ToString());
                return Ok();
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "command":
                        _logger.LogError("Произошла ошибка при выполнении запроса к бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при выполнении запроса к бд" });
                    case "db":
                        _logger.LogError("Произошла ошибка при обновлении данных бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "db", errorMessage = "Ошибка при обновлении бд" });
                    default:
                        _logger.LogError("Возникло неожиданное исключение. " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpected", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }

        [HttpPost("increase/with-session")]
        public async Task<IActionResult> PostIncreaseWithSessionAsync([FromBody] EventIncreaseModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Модель не прошла валидацию: " + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return BadRequest(new Error() { errorCode = "badvalue", errorMessage = "Модель не прошла валидацию" });
                }

                if (!SqlHelper.IsDeviceExists(_dbContext, model.device))
                {
                    //_logger.LogError($"Не удалось найти девайс {model.device}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {model.device}" });
                }

                if (!SqlHelper.IsEventKindExists(_dbContext, model.eventKindCode))
                {
                    //_logger.LogError($"Не удалось найти тип ивента {model.eventKindCode}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип ивента {model.eventKindCode}" });
                }

                if (!SqlHelper.IsPostSessionExists(_dbContext, model.idPostSession, model.device))
                {
                    //_logger.LogError($"Не найдена сессия для поста {model.device} с id={model.idPostSession}" + Environment.NewLine);
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия поста {model.device} с id={model.idPostSession}" });
                }

                int idEvent = await SqlHelper.WriteEventIncreaseAsync(_dbContext, model);
                Response.Headers.Add("ServerID", idEvent.ToString());
                return Ok();
            }
            catch (Exception e)
            {
                switch (e.Message)
                {
                    case "command":
                        _logger.LogError("Произошла ошибка при выполнении запроса к бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "command", errorMessage = "Ошибка при выполнении запроса к бд" });
                    case "db":
                        _logger.LogError("Произошла ошибка при обновлении данных бд: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "db", errorMessage = "Ошибка при обновлении бд" });
                    default:
                        _logger.LogError("Возникло неожиданное исключение. " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                        return StatusCode(500, new Error() { errorCode = "unexpected", errorMessage = "Непредвиденное исключение, необходимо смотреть лог сервиса" });
                }
            }
        }

        public async Task<IActionResult> PostIncreaseMobileAsync([FromBody] EventIncreaseModel model)
        {
            try
            {
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}
