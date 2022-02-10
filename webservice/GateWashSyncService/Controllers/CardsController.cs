using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GateWashSyncService.Controllers.BindingModels;
using GateWashSyncService.Controllers.Helpers;
using GateWashSyncService.Models.GateWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ILogger<CardsController> _logger;
        public CardsController(ILogger<CardsController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostCardAsync(CardBindingModel card)
        {
            try
            {
                GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

                if (sqlHelper.IsCardExsists(card.cardNum))
                {
                    return Conflict(new Error() { errorCode = "badvalue", errorMessage = "Карта с таким номером уже существует" });
                }

                int id = await sqlHelper.WriteCardAsync(card);

                Response.Headers.Add("ServerID", id.ToString());
                _logger.LogInformation($"Добавлена карта {card.cardNum}, id = {id}" + Environment.NewLine);
                return Created(id.ToString(), null);
            }
            catch (Exception e)
            {
                switch (e.Message) {
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
