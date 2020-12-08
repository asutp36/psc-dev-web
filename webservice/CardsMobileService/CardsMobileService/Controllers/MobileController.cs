using CardsMobileService.Controllers.Supplies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        private readonly ILogger<MobileController> _logger;
        private CardsApi _cardsApi;
        PostApi _postApi;
        public MobileController(ILogger<MobileController> logger, CardsApi cardsApi, PostApi postApi)
        {
            _logger = logger;
            _cardsApi = cardsApi;
            _postApi = postApi;
        }

        [SwaggerOperation(Summary = "Пополнение с мобильного приложения")]
        [SwaggerResponse(201, Description = "Успешно записано")]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(401, Description = "Хэш не прошёл проверку")]
        [SwaggerResponse(404, Description = "Карты с таким номером не существует")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        [SwaggerResponse(503, Description = "Ошибка записи в базу")]
        [HttpPost("increase")]
        public IActionResult PostIncrease(IncreaseFromMobile model)
        {
            _logger.LogDebug("запуск с параметрами\n" + JsonConvert.SerializeObject(model));

            if (!ModelState.IsValid)
            {
                _logger.LogError("модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                _logger.LogError("хэш не прошёл проверку" + Environment.NewLine);
                return Unauthorized();
            }

            if (!_cardsApi.IsExist(model.cardNum))
            {
                _logger.LogError("карта не найдена" + Environment.NewLine);
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
                    _logger.LogError("транзакция не прошла" + Environment.NewLine);
                    return StatusCode(503, "Ошибка записи в базу");
                }

                _logger.LogDebug("пополнение записано id = " + serverID + Environment.NewLine);

                return StatusCode(201);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        [SwaggerOperation(Summary = "Узнать баланс карты")]
        [SwaggerResponse(200, Description = "Успешно", Type = typeof(int))]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(401, Description = "Хэш не прошёл проверку")]
        [SwaggerResponse(404, Description = "Не найдена такая карта")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        [HttpPost("get_balance")]
        public IActionResult GetBalance(GetBalanceFromMobile model)
        {
            _logger.LogInformation("запуск с параметрами:\n" + JsonConvert.SerializeObject(model));
            try
            {
                if (!CryptHash.CheckHashCode(model.hash, model.dtime))
                {
                    _logger.LogError("хэш не прошёл проверку" + Environment.NewLine);
                    return Unauthorized();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (!_cardsApi.IsExist(model.cardNum))
                {
                    _logger.LogError("карта с таким номером не найдена");
                    return NotFound();
                }

                int? balance = _cardsApi.GetBalance(model.cardNum);

                if (balance == null)
                {
                    _logger.LogError("баланс null" + Environment.NewLine);
                    return StatusCode(500);
                }

                _logger.LogInformation("баланс = " + balance + Environment.NewLine);

                return Ok(balance);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }


        [SwaggerOperation(Summary = "Запуск поста")]
        [SwaggerResponse(200, Description = "Успешно")]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(401, Description = "Хэш не прошёл проверку")]
        [SwaggerResponse(404, Description = "Не найдена такая карта")]
        [SwaggerResponse(405, Description = "Пост не найден")]
        [SwaggerResponse(417, Description = "Неудачный ответ поста")]
        [SwaggerResponse(422, Description = "Сумма недостаточна для включения")]
        [SwaggerResponse(423, Description = "Пост занят")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        [HttpPost("start_post")]
        public IActionResult StartPost(StartModelMobile model)
        {
            _logger.LogInformation("запуск с параметрами\n" + JsonConvert.SerializeObject(model));

            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                _logger.LogError("хэш не прошёл проверку" + Environment.NewLine);
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (!_cardsApi.IsExist(model.cardNum))
            {
                _logger.LogError("карты с номером {0} не существует" + Environment.NewLine);
                return NotFound();
            }

            if (!_postApi.IsExist(model.post))
            {
                _logger.LogError("не найден код поста" + Environment.NewLine);
                return StatusCode(405);
            }

            string result = _postApi.Start(model);

            switch (result)
            {
                case "weak":
                    _logger.LogError("сумма недостаточна для включения" + Environment.NewLine);
                    return StatusCode(422);

                case "unavailible":
                    _logger.LogError("не удалось установить связь с постом" + Environment.NewLine);
                    return StatusCode(424);

                case "busy":
                    _logger.LogError("пост занят" + Environment.NewLine);
                    return StatusCode(423);
                case "ok":
                    _logger.LogError("мойка начата" + Environment.NewLine);
                    return Ok();

                default:
                    _logger.LogError(result + Environment.NewLine);
                    return StatusCode(417);
            }
        }

        [SwaggerOperation(Summary = "Запись новой карты")]
        [SwaggerResponse(201, Description = "Создано успешно")]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(401, Description = "Хэш не прошёл проверку")]
        [SwaggerResponse(409, Description = "На этот номер уже записана карта")]
        [SwaggerResponse(417, Description = "Ошибка при записи в базу")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        [HttpPost("new_card")]
        public IActionResult PostCard(NewCardFromMobile model)
        {
            _logger.LogInformation("запуск с параметрами: " + JsonConvert.SerializeObject(model));

            if (!CryptHash.CheckHashCode(model.hash, model.dtime))
            {
                _logger.LogError("хэш не прошёл проверку" + Environment.NewLine);
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (_cardsApi.GetCardsByPhone(model.phone).Count > 0)
            {
                _logger.LogError("на этот номер уже записаны карты" + Environment.NewLine);
                return Conflict();
            }

            try
            {
                NewCardFromChanger card = new NewCardFromChanger
                {
                    changer = "MOB-EM",
                    localizedID = 0,
                    amount = 0,
                    dtime = model.dtime,
                    cardNum = _cardsApi.GetNewCardNum(),
                    phone = model.phone
                };

                int serverID = _cardsApi.WriteNewCard(card);

                if (serverID > 0)
                {
                    _logger.LogDebug("удачно всё запсано. SeverID = " + serverID + Environment.NewLine);
                    return StatusCode(201, card.cardNum);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(417, "Не найдена база данных");
            }

            return StatusCode(500);
        }
    }
}
