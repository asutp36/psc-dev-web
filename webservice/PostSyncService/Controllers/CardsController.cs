using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PostSyncService.Controllers.BindingModels;
using PostSyncService.Controllers.Helpers;
using PostSyncService.Models.GateWash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostSyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostCardAsync(CardBindingModel card)
        {
            GateWashSqlHelper sqlHelper = new GateWashSqlHelper();
            
            if(sqlHelper.IsCardExsists(card.cardNum))
            {
                return Conflict(new Error() { errorCode = "badvalue", errorMessage = "Карта с таким номером уже существует" });
            }

            int id = await sqlHelper.WriteCardAsync(card);

            Response.Headers.Add("ServerID", id.ToString());
            return Created(id.ToString(), null);
        }
    }
}
