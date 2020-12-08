using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CardsMobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        ILogger<PostController> _logger;
        CardsApi _cardsApi;
        PostApi _postApi;

        public PostController(ILogger<PostController> logger, CardsApi cardsApi, PostApi postApi)
        {
            _logger = logger;
            _cardsApi = cardsApi;
            _postApi = postApi;
        }

        #region Swagger Descrition
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
        #endregion
        [HttpPost("start")]
        public IActionResult Start(PostActionModel model)
        {
            _logger.LogInformation("запуск с параметрами\n" + JsonConvert.SerializeObject(model));

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

        #region Swagger Descrition
        [SwaggerOperation(Summary = "Отправка конца мойки на мобильное приложение")]
        [SwaggerResponse(200, Description = "Удачно отправлено")]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(404, Description = "Не найдена такая карта")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("stop")]
        public IActionResult Stop(PostActionModel model)
        {
            _logger.LogInformation("запуск с параметрами\n" + JsonConvert.SerializeObject(model));

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

            try
            {
                int decreaseResult = _cardsApi.WriteDecrease(new IncreaseFromChanger
                {
                    cardNum = model.cardNum,
                    changer = model.post,
                    amount = model.amount,
                    operationType = "decrease",
                    localizedID = 0
                });

                if (decreaseResult > 0)
                {
                    _logger.LogInformation("списание записано id = " + decreaseResult);
                }
                else
                {
                    _logger.LogError("списание не записано");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Supplies.HttpResponse response = HttpSender.SendPost("http://loyalty.myeco24.ru/api/externaldb/set-waste", JsonConvert.SerializeObject(new DecreaseToMobile(dtime, model.cardNum, dtime, model.post, model.amount)));

            _logger.LogInformation("ответ сервера мобильного приложения:\n" + JsonConvert.SerializeObject(response) + Environment.NewLine);

            return StatusCode((int)response.StatusCode, response.ResultMessage);
        }

        #region Swagger Descrition
        [SwaggerOperation(Summary = "Получить номер карты по номеру владельца")]
        [SwaggerResponse(200, Description = "Успешно", Type = typeof(List<string>))]
        [SwaggerResponse(404, Description = "Не найдены карты")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpGet]
        [Route("cards/{phone}")]
        public IActionResult GetCards(string phone)
        {
            _logger.LogInformation("запуск с параметром: " + phone);
            try
            {
                List<string> cards = _cardsApi.GetCardsByPhone(phone);
                _logger.LogInformation("найдено:" + JsonConvert.SerializeObject(cards) + Environment.NewLine);
                if (cards.Count > 0)
                    return Ok(cards);
                else
                    return NotFound();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Descrition
        [SwaggerOperation(Summary = "Новая карта с разменника")]
        [SwaggerResponse(201, Description = "Успешно")]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(409, Description = "Карта с таким номером уже существует")]
        [SwaggerResponse(417, Description = "Ошибка при записи в базу")]
        [SwaggerResponse(500, Description = "Не удалось записать карту")]
        [SwaggerResponse(503, Description = "Не удалось записать пополнение")]
        #endregion
        [HttpPost("card")]
        public IActionResult NewCard(NewCardFromChanger model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (_cardsApi.IsExist(model.cardNum))
            {
                return Conflict();
            }

            try
            {
                int serverID = _cardsApi.WriteNewCard(model);

                if (serverID <= 0)
                {
                    _logger.LogDebug("удачно всё записано. SeverID = " + Environment.NewLine);
                    return StatusCode(500, "Не удалось записать новую карту в базу");
                }

                _logger.LogDebug("записана новая карта. SeverID = " + serverID);
                _logger.LogDebug("отправка новой карты на сервер приложения");
                string sendCardResult = _cardsApi.SendCardToApp(model);

                if (!sendCardResult.Equals("ok"))
                {
                    _logger.LogError("отправка неудачная. Ответ сервера: " + sendCardResult);
                }
                else
                {
                    _logger.LogDebug("карта отправлена успешно");
                }

                _logger.LogDebug("запись внесения");
                IncreaseFromChanger increase = new IncreaseFromChanger
                {
                    changer = model.changer,
                    cardNum = model.cardNum,
                    dtime = model.dtime,
                    operationType = "increase",
                    amount = model.amount,
                    localizedID = model.localizedID
                };

                int increaseServerID = _cardsApi.WriteIncrease(increase);

                if (increaseServerID == -1)
                {
                    _logger.LogError("не удалось записать внесение" + Environment.NewLine);
                    return StatusCode(503, "Не удалось записать пополнение");
                }

                _logger.LogDebug("отправка пополнения на сервер приложения");
                string sendIncreaseResult = _cardsApi.SendIncreaseToApp(increase);

                if (!sendIncreaseResult.Equals("ok"))
                {
                    _logger.LogError("не удалось отправить списание. Ответ сервера: " + sendIncreaseResult);
                }
                else
                {
                    _logger.LogDebug("пополнение отправлено успешно");
                }

                return StatusCode(201);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, "Непредвиденная ошибка");
            }
        }

        #region Swagger Descrition
        [SwaggerOperation(Summary = "Получить номер телефона и баланс по номеру карты")]
        [SwaggerResponse(200, Description = "Успешно", Type = typeof(GetPhoneByCardNum))]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервиса")]
        #endregion
        [HttpGet("phone/{cardNum}")]
        public IActionResult GetPhoneByCardNum(string cardNum)
        {
            if (!_cardsApi.IsExist(cardNum))
            {
                return NotFound();
            }

            try
            {
                GetPhoneByCardNum result = new GetPhoneByCardNum();
                result.phone = _cardsApi.GetPhone(cardNum);
                result.balance = _cardsApi.GetBalance(cardNum);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }
        }
        
        #region Swagger Descrition
        [SwaggerOperation(Summary = "Получить номера всех технических карт")]
        [SwaggerResponse(200, Description = "Успешно", Type = typeof(TechCards))]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервиса")]
        #endregion
        [HttpGet("cards/tech")]
        public IActionResult GetTechCards()
        {
            try
            {
                return Ok(_cardsApi.GetTechCards());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Descrition
        [SwaggerOperation(Summary = "Получить номера технических карт по коду поста")]
        [SwaggerResponse(200, Description = "Успешно", Type = typeof(TechCards))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервиса")]
        #endregion
        [HttpGet("cards/tech/{post}")]
        public IActionResult GetTechCards(string post)
        {
            try
            {
                if (!_postApi.IsExist(post))
                    return NotFound();

                string washCode = _postApi.GetWashCode(post);

                TechCards res = _cardsApi.GetTechCards(washCode);
                return Ok(res);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Descrition
        [SwaggerOperation(Summary = "Изменить номер карты")]
        [SwaggerResponse(200, Description = "Успешно")]
        [SwaggerResponse(400, Description = "Модель не прошла валидацию")]
        [SwaggerResponse(404, Description = "не найдена карта")]
        [SwaggerResponse(409, Description = "Карта с таким номером уже существует")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервиса")]
        #endregion
        [HttpPut("card")]
        public IActionResult ChangeCardNum(ChangeCardNumModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_cardsApi.IsExist(model.oldNum))
            {
                return NotFound();
            }

            if (_cardsApi.IsExist(model.newNum))
            {
                return Conflict();
            }

            string result = _cardsApi.UpdateCardNum(model);

            switch (result)
            {
                case "ok":
                    return Ok();

                default:
                    return StatusCode(500, "Ошибка при записи в базу");
            }
        }
    }
}
