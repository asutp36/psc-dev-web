using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsMobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        /// <summary>
        /// Пополнение с мобильного приложения
        /// </summary>
        /// <response code="201">Успешно записано</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="404">Карты с таким номером не существует</response>
        [HttpPost("increase")]
        public IActionResult PostIncrease(IncreaseFromMobile model)
        {
            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            CardsApi cardsApi = new CardsApi();

            if (!cardsApi.IsExsisting(model.cardNum))
            {
                return NotFound();
            }

            cardsApi.WriteIncrease(new IncreaseFromChanger
            {
                cardNum = model.cardNum,
                changer = "MOB-EM",
                dtime = model.dtime,
                amount = model.amount,
                operationType = model.operationType,
                localizedID = 0
            });

            return Created("/mobile/increase", 203);
        }

        [HttpGet("balance")]
        public IActionResult GetBalance(string cardNum)
        {
            if (!CryptHash.CheckHashCode("hash", "dtime"))
            {
                return Unauthorized();
            }

            if (cardNum == null || cardNum.Length < 5)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpGet("cards")]
        public IActionResult GetCards(string phone)
        {
            if (!CryptHash.CheckHashCode("hash", "dtime"))
            {
                return Unauthorized();
            }

            if (phone == null || phone.Length < 11)
            {
                return BadRequest();
            }

            return Ok();
        }

        [HttpPut("phone")]
        public IActionResult UpdatePhone()
        {
            if (!CryptHash.CheckHashCode("hash", "dtime"))
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpPost("start")]
        public IActionResult StartPost()
        {
            if (!CryptHash.CheckHashCode("hash", "dtime"))
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpPost("card")]
        public IActionResult PostCard()
        {
            if (!CryptHash.CheckHashCode("hash", "dtime"))
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}