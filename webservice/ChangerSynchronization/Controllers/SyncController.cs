using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangerSynchronization.Controllers.Supplies;
using ChangerSynchronization.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace ChangerSynchronization.Controllers
{
    [Route("api/sync")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        /// <summary>
        /// Синхронизация события разменника
        /// </summary>
        /// <param name="model">Всё событие на разменнике целиком</param>
        /// <returns></returns>
        /// <response code="400">Входные данные некорректны</response>
        /// <response code="200">Ок, в теле айдишники</response>
        [HttpPost("event")]
        public IActionResult PostEvent(EventChangerFull model)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }



            return Ok();
        }

        private void WriteEventChanger(EventChangerFull model)
        {
            using ModelDbContext modelDb = new ModelDbContext();

            try
            {
                IDbContextTransaction transaction = modelDb.Database.BeginTransaction();

                modelDb.EventChanger.Add(new EventChanger
                {
                    Idchanger = modelDb.Changers.Where(c => c.Name.Equals(model.changer)).FirstOrDefault().Idchanger,
                    IdeventChangerKind = modelDb.EventChangerKind.Where(evk => evk.Code.Equals(model.eventKindCode)).FirstOrDefault().IdeventChangerKind,
                    Dtime = model.dtime
                });

                
            }
            catch(Exception e)
            {

            }
        }

    }
}