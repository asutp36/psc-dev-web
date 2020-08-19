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
        PostApi _postApi;

        public MobileController(ILogger<MobileController> logger, CardsApi cardsApi, PostApi postApi)
        {
            _logger = logger;
            _cardsApi = cardsApi;
            _postApi = postApi;
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
            _logger.LogDebug("PostIncrease: запуск с параметрами\n" + JsonConvert.SerializeObject(model));

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

            if (!_cardsApi.IsExist(model.cardNum))
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

        /// <summary>
        /// Запуск поста
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Успешно</response>
        /// <response code="400">Модель не прошла валидацию</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="404">Карта не найдена</response>
        /// <response code="405">Пост не найден</response>
        /// <response code="417">Неудачный ответ поста</response>
        /// <response code="422">Сумма недостаточна для включения</response>
        /// <response code="423">Пост занят</response>
        /// <response code="424">Нет связи с постом</response>
        [HttpPost("start")]
        public IActionResult StartPost(StartModelMobile model)
        {
            _logger.LogInformation("StartPost: запуск с параметрами\n" + JsonConvert.SerializeObject(model));

            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                _logger.LogError("StartPost: хэш не прошёл проверку" + Environment.NewLine);
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("StartPost: модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (!_cardsApi.IsExist(model.cardNum))
            {
                _logger.LogError("StartPost: карты с номером {0} не существует" + Environment.NewLine);
                return NotFound();
            }

            if (!_postApi.IsExist(model.post))
            {
                _logger.LogError("StartPost: не найден код поста" + Environment.NewLine);
                return StatusCode(405);
            }

            string result = _postApi.Start(model);

            switch (result)
            {
                case "weak":
                    _logger.LogError("StartPost: сумма недостаточна для включения" + Environment.NewLine);
                    return StatusCode(422);

                case "unavailible":
                    _logger.LogError("StartPost: не удалось установить связь с постом" + Environment.NewLine);
                    return StatusCode(424);

                case "busy":
                    _logger.LogError("StartPost: пост занят" + Environment.NewLine);
                    return StatusCode(423);
                case "ok":
                    _logger.LogError("StartPost: мойка начата" + Environment.NewLine);
                    return Ok();

                default:
                    _logger.LogError("StartPost: " + result + Environment.NewLine);
                    return StatusCode(417);
            }
        }

        /// <summary>
        /// Запись новой карты
        /// </summary>
        /// <param name="model">Данные новой карты</param>
        /// <returns></returns>
        /// <response code="201">Создано успешно</response>
        /// <response code="400">Модель не прошла валидацию</response>
        /// <response code="401">Хэш не прошёл валидацию</response>
        /// <response code="409">На этот номер уже записана карта</response>
        /// <response code="417">Ошибка при записи в базу</response>
        /// <response code="500">Внутренняя ошибка</response>
        [HttpPost("card")]
        public IActionResult PostCard(NewCardFromMobile model)
        {
            _logger.LogInformation("PostCard: запуск с параметрами: " + JsonConvert.SerializeObject(model));

            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                _logger.LogError("PostCard: хэш не прошёл проверку" + Environment.NewLine);
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("PostCard: модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (_cardsApi.GetCardsByPhone(model.phone).Count > 0)
            {
                _logger.LogError("PostCard: на этот номер уже записаны карты" + Environment.NewLine);
                return Conflict();
            }
            try
            {
                int serverID = _cardsApi.WriteNewCard(new NewCardFromChanger
                {
                    changer = "MOB-EM",
                    localizedID = 0,
                    amount = 0,
                    dtime = model.dtime,
                    cardNum = _cardsApi.GetNewCardNum(),
                    phone = model.phone
                });

                if (serverID > 0)
                {
                    _logger.LogDebug("PostCard: удачно всё запсано. SeverID = " + serverID + Environment.NewLine);
                    return Created("/api/mobile/card", null);
                }
            }
            catch(Exception e)
            {
                _logger.LogError("PostCard: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(417, "Не найдена база данных");
            }

            return StatusCode(500);
        }
    }
}