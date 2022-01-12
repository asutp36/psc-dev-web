using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class SessionsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostSessionAsync(SessionBindingModel session)
        {
            GateWashSqlHelper sqlHelper = new GateWashSqlHelper();
            if (!sqlHelper.IsCardExsists(session.idCard))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Карта {session.idCard} не найдена" });
            if (!sqlHelper.IsFunctionExsists(session.functionCode))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Функция {session.functionCode} не найдена" });
            if (sqlHelper.IsSessionExsists(session.idCard, session.uuid))
                return Conflict(new Error() { errorCode = "badvalue", errorMessage = $"Сессия с картой {session.idCard} и uuid {session.uuid} уже записана" });

            int id = await sqlHelper.WriteSessionAsync(session);

            return Ok(id);
        }
    }
}
