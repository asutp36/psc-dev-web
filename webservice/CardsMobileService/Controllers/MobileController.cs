using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardsMobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        ILogger<MobileController> _logger;
        CardsApi _cardsApi;

        public MobileController(ILogger<MobileController> logger, CardsApi cardsApi)
        {
            _logger = logger;
            _cardsApi = cardsApi;
        }

        /// <summary>
        /// Пополнение с мобильного приложения
        /// </summary>
        /// <response code="201">Успешно записано</response>
        /// <response code="400">Некорректные входные данные</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="404">Карты с таким номером не существует</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        /// <response code="503">Ошибка записи в базу</response>
        [HttpPost("increase")]
        public IActionResult PostIncrease(IncreaseFromMobile model)
        {
            _logger.LogDebug("PostIncrease: запуск с параметром: " + JsonConvert.SerializeObject(model));

            if (!ModelState.IsValid)
            {
                _logger.LogError("PostIncrease: модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                _logger.LogError("PostIncrease: хэш не прошёл проверку" + Environment.NewLine);
                return Unauthorized();
            }

            if (!_cardsApi.IsExsisting(model.cardNum))
            {
                _logger.LogError("PostIncrease: карта не найдена" + Environment.NewLine);
                return NotFound();
            }

            try
            {
                int serverID = _cardsApi.WriteIncrease(new IncreaseFromChanger
                {
                    cardNum = model.cardNum,
                    changer = "MOB-EM",
                    dtime = model.dtime,
                    amount = model.amount,
                    operationType = model.operationType,
                    localizedID = 0
                });
                
                if (serverID == -1)
                {
                    _logger.LogError("PostIncrease: транзакция не прошла" + Environment.NewLine);
                    return StatusCode(503, "Ошибка записи в базу");
                }

                _logger.LogDebug("PostIncrease: пополнение записано id = " + serverID + Environment.NewLine);

                //return Created("/mobile/increase", serverID);
                return StatusCode(201);
            }
            catch (Exception e)
            {
                _logger.LogCritical("PostIncrease: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
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