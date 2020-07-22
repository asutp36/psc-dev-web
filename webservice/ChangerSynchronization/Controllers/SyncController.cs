using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangerSynchronization.Controllers.Supplies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChangerSynchronization.Controllers
{
    [Route("api/sync")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        /// <summary>
        /// Синхронизация события разменника
        /// </summary>
        /// <param name="model">Большое событие на разменнике</param>
        /// <returns></returns>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="200">Ок, в теле айдишники</response>
        [HttpPost("event")]
        public IActionResult PostEvent(EventChanger model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }



            return Ok();
        }

        private void WriteEventChanger()
        {

        }

        private void WriteEventChangerIncrease()
        {

        }

        private void WriteEventChangerOut()
        {

        }

        private void WriteEventChangerAcquiring()
        {

        }
    }
}