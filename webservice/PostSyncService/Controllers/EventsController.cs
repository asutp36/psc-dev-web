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
    public class EventsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostEvent(EventBindingModel evnt)
        {
            GateWashSqlHelper sqlHelper = new GateWashSqlHelper();

            if (!sqlHelper.IsSessionExsists(evnt.cardNum, evnt.uuid))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найдена сессия для карты {evnt.cardNum} с uuid = {evnt.uuid}" });
            if(!sqlHelper.IsDeviceExsists(evnt.deviceCode))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден девайс {evnt.deviceCode}" });
            if (!sqlHelper.IsEventKindExsists(evnt.eventKindCode))
                return NotFound(new Error() { errorCode = "badvalue", errorMessage = $"Не найден тип event {evnt.eventKindCode}" });

            int id = await sqlHelper.WriteEventAsync(evnt);

            Response.Headers.Add("ServerID", id.ToString());
            return Created(id.ToString(), null);
        }
    }
}
