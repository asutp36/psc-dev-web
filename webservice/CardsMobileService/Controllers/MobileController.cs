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

            cardsApi.WriteIncrease(new IncreaseFromChanger
            {
                cardNum = model.cardNum,
                changer = "MOB-EM",
                dtime = model.dtime,
                amount = model.amount,
                operationType = model.operationType,
                localizedID = 0
            });

            return Ok();
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