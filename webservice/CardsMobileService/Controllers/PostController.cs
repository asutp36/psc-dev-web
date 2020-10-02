using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardsMobileService.Controllers
{
    [Route("api/post")]
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

        /// <summary>
        /// Запуск поста
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Успешно</response>
        /// <response code="400">Модель не прошла валидацию</response>
        /// <response code="404">Карта не найдена</response>
        /// <response code="405">Пост не найден</response>
        /// <response code="417">Неудачный ответ поста</response>
        /// <response code="422">Сумма недостаточна для включения</response>
        /// <response code="423">Пост занят</response>
        /// <response code="424">Нет связи с постом</response>
        [HttpPost("start")]
        public IActionResult Start(PostActionModel model)
        {
            _logger.LogInformation("StartPost: запуск с параметрами\n" + JsonConvert.SerializeObject(model));

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
        /// Отправка конца мойки на мобильное приложение
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Удачно отправлено</response>
        /// <response code="400">Модель не прошла валидацию</response>
        /// <response code="404">Карта не найдена</response>
        /// <response code="1000">Остальные коды ответов приходят с мобильного приложения</response>
        [HttpPost("stop")]
        public IActionResult Stop(PostActionModel model)
        {
            _logger.LogInformation("StopPost: запуск с параметрами\n" + JsonConvert.SerializeObject(model));

            if (!ModelState.IsValid)
            {
                _logger.LogError("StopPost: модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (!_cardsApi.IsExist(model.cardNum))
            {
                _logger.LogError("StopPost: карты с номером {0} не существует" + Environment.NewLine);
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
                    _logger.LogInformation("StopPost: списание записано id = " + decreaseResult);
                }
                else
                {
                    _logger.LogError("StopPost: списание не записано");
                }
            }
            catch(Exception e)
            {
                _logger.LogError("StopPost: " + e.Message);
            }

            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            Supplies.HttpResponse response = HttpSender.SendPost("http://loyalty.myeco24.ru/api/externaldb/set-waste", JsonConvert.SerializeObject(new DecreaseToMobile(dtime, model.cardNum, dtime, model.post, model.amount)));

            _logger.LogInformation("StopPost: ответ сервера мобильного приложения:\n" + JsonConvert.SerializeObject(response) + Environment.NewLine);

            return StatusCode((int)response.StatusCode, response.ResultMessage);
        }

        /// <summary>
        /// Получить номер карты по номеру владельца
        /// </summary>
        /// <param name="phone">Номер владельца</param>
        /// <returns></returns>
        /// <response code="200">Номер карты в теле</response>
        /// <response code="404">Не найдены карты</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("cards/{phone}")]
        public IActionResult GetCards(string phone)
        {
            _logger.LogInformation("POST GetCards: запуск с параметром: " + phone);
            try
            {
                List<string> cards = _cardsApi.GetCardsByPhone(phone);
                _logger.LogInformation("POST GetCards: найдено:" + JsonConvert.SerializeObject(cards) + Environment.NewLine);
                if (cards.Count > 0)
                    return Ok(cards);
                else
                    return NotFound();
            }
            catch(Exception e)
            {
                _logger.LogError("POST GetCards: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Новая карта с разменника
        /// </summary>
        /// <param name="model"></param>
        /// <response code="201">Удачно</response>
        /// <response code="400">Модель не прошла валидацию</response>
        /// <response code="409">Карта с таким номером уже существует</response>
        /// <response code="417">Ошибка при записи в базу</response>
        /// <response code="500">Не удалось записать карту</response>
        /// <response code="503">Не удалось записать пополнение</response>
        [HttpPost]
        [Route("card")]
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
                    _logger.LogDebug("MOBILE PostCard: удачно всё записано. SeverID = " + Environment.NewLine);
                    return StatusCode(500, "Не удалось записать новую карту в базу");
                }

                _logger.LogDebug("MOBILE PostCard: записана новая карта. SeverID = " + serverID);
                _logger.LogDebug("MOBILE PostCard: отправка новой карты на сервер приложения");
                string sendCardResult = _cardsApi.SendCardToApp(model);

                if (!sendCardResult.Equals("ok"))
                {
                    _logger.LogError("MOBILE PostCard: отправка неудачная. Ответ сервера: " + sendCardResult);
                }
                else
                {
                    _logger.LogDebug("MOBILE PostCard: карта отправлена успешно");
                }

                _logger.LogDebug("MOBILE PostCard: запись внесения");
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
                    _logger.LogError("MOBILE PostCard: не удалось записать внесение" + Environment.NewLine);
                    return StatusCode(503, "Не удалось записать пополнение");
                }

                _logger.LogDebug("MOBILE PostCard: отправка пополнения на сервер приложения");
                string sendIncreaseResult = _cardsApi.SendIncreaseToApp(increase);

                if (!sendIncreaseResult.Equals("ok"))
                {
                    _logger.LogError("MOBILE PostCard: не удалось отправить списание. Ответ сервера: " + sendIncreaseResult);
                }
                else
                {
                    _logger.LogDebug("MOBILE PostCard: пополнение отправлено успешно");
                }

                return StatusCode(201);
            }
            catch (Exception e)
            {
                _logger.LogError("MOBILE PostCard: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, "Непредвиденная ошибка");
            }
        }

        /// <summary>
        /// Получить номер телефона и баланс по номеру карты
        /// </summary>
        /// <param name="cardNum"></param>
        /// <response code="200">Ок</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpGet]
        [Route("phone/{cardNum}")]
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
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }

    }
}