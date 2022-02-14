using GateWashSyncService.Controllers.BindingModels;
using GateWashSyncService.Controllers.Helpers;
using GateWashSyncService.Models.GateWash;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectsController : ControllerBase
    {
        private readonly ILogger<CollectsController> _logger;
        private readonly GateWashDbContext _model;

        public CollectsController(ILogger<CollectsController> logger, GateWashDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        [HttpPost]
        public async Task<IActionResult> PostCollect(CollectBindingModel collect)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper(_model);

                if (!sqlHelper.IsDeviceExsists(collect.deviceCode))
                {
                    _logger.LogError($"Не найден девайс {collect.deviceCode}");
                    return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {collect.deviceCode}" });
                }

                int id = await sqlHelper.WriteCollectAsync(collect);

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
