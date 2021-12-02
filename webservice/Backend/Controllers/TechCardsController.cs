using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpResponse = Backend.Controllers.Supplies.HttpResponse;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechCardsController : ControllerBase
    {
        ILogger<TechCardsController> _logger;
        IConfiguration _config;

        public TechCardsController(ILogger<TechCardsController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить технические карты по коду мойки")]
        [SwaggerResponse(200, Type = typeof(List<GroupViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("wash/{washCode}")]
        public IActionResult GetCardsByWash(string washCode)
        {
            try
            {
                if (!SqlHelper.IsWashExists(washCode))
                    return NotFound(new Error("Мойка не найдена.", "badvalue"));

                var cards = SqlHelper.GetGroupsTechCardsByWash(washCode);
                return Ok(cards);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить технические карты по коду группы")]
        [SwaggerResponse(200, Type = typeof(GroupViewModel))]
        [SwaggerResponse(404, Type = typeof(Error))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("group/{groupCode}")]
        public IActionResult GetCardsByGroup(string groupCode)
        {
            try
            {
                var cards = SqlHelper.GetTechCardsByGroup(groupCode);
                return Ok(cards);
            }
            catch (Exception e)
            {
                if (e.Message == "404")
                    return NotFound(new Error("Не найдена группа.", "badvalue"));

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить типы технических карт")]
        [SwaggerResponse(200, Type = typeof(List<CardTypeViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("types")]
        public IActionResult GetCardTypes()
        {
            try
            {
                return Ok(SqlHelper.GetTechCardTypes());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Создать новую техническую карту")]
        [SwaggerResponse(201)]
        [SwaggerResponse(400, Type = typeof(Error), Description = "Некорректные входные параметры")]
        [SwaggerResponse(409, Type = typeof(Error), Description = "Карта с таким номером уже существует")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpPost]
        public IActionResult PostTechCard(TechCardModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Model not valid: " + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return BadRequest(new Error("Некорректно заданы значения.", "badvalue"));
                }

                if (SqlHelper.IsCardExists(model.cardNum))
                    return UpdateTechCardGroups(model.cardNum, model.groupCode);

                SqlHelper.WriteTechCard(model);

                return StatusCode(201);
            }
            catch (Exception e)
            {
                if (e.Message == "command")
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error("Произошла ошибка в ходе обращения к базе данных.", "db"));
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Добавить техническую карту в группу")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Карта с таким номером или группа не существует")]
        [SwaggerResponse(409, Type = typeof(Error), Description = "Карта уже находится в этой группе")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpPatch("{cardNum}/group")]
        public IActionResult UpdateTechCardGroups(string cardNum, string groupCode)
        {
            try
            {
                if (!SqlHelper.IsCardExists(cardNum))
                    return NotFound(new Error("Карта с таким номером не найдена.", "badvalue"));

                if (!SqlHelper.IsGroupExists(groupCode))
                    return NotFound(new Error("Группа не найдена.", "badvalue"));

                SqlHelper.AddTechCardGroup(cardNum, groupCode);

                return Ok();
            }
            catch(Exception e)
            {
                if (e.Message == "constraint")
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return Conflict(new Error($"Карта {cardNum} уже находится в группе {groupCode}.", "badvalue"));
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Удалить техническую карту")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Карта с таким номером не существует")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpDelete("{cardNum}")]
        public IActionResult DeleteTechCard(string cardNum)
        {
            try
            {
                if (!SqlHelper.IsCardExists(cardNum))
                    return NotFound(new Error("Карта с таким номером не найдена", "badvalue"));

                SqlHelper.DeleteCard(cardNum);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Отправить техническую карту на мойку")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Карта или мойка не существует")]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Не получилось связаться с мойкой или сервисом управления постами")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpPost("send/{cardNum}/{groupCode}")]
        public IActionResult SendTechCardOnWash(string cardNum, string groupCode, [FromBody]string[] washCodes)
        {
            try
            {
                if (!SqlHelper.IsCardExists(cardNum))
                {
                    _logger.LogError($"Карта {cardNum} не найдена" + Environment.NewLine);
                    return NotFound(new Error($"Карта {cardNum} не найдена", "badvalue"));
                }

                if (!SqlHelper.IsGroupExists(groupCode))
                {
                    _logger.LogError($"Группа {groupCode} не найдена" + Environment.NewLine);
                    return NotFound(new Error($"Группа {groupCode} не найдена", "badvalue"));
                }

                foreach (string washCode in washCodes)
                {
                    if (!SqlHelper.IsWashExists(washCode))
                    {
                        _logger.LogError($"Мойка {washCode} не найдена" + Environment.NewLine);
                        continue;
                        //return NotFound(new Error($"Мойка {washCode} не найдена", "badvalue"));
                    }

                    SetWashParameter<TechCardDeviceBindingModel> param = new SetWashParameter<TechCardDeviceBindingModel>()
                    {
                        washCode = washCode,
                        value = new TechCardDeviceBindingModel() { cardNum = cardNum, cardType = SqlHelper.GetCardTypeByNum(cardNum).code, cardName = cardNum }
                    };

                    SetParameterResultWash setResult = new SetParameterResultWash();
                    setResult.wash = washCode;

                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/techcards/create/wash", JsonConvert.SerializeObject(param));
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.InternalServerError:
                                _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                                continue;
                            //return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами", "service"));
                            case (System.Net.HttpStatusCode)424:
                                _logger.LogError($"Не удалось соединиться с мойкой {washCode}" + Environment.NewLine);
                                continue;
                            //return StatusCode(424, new Error($"Не удалось соединиться с мойкой {washCode}", "connection"));
                            case (System.Net.HttpStatusCode)0:
                                _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                                continue;
                            //return StatusCode(424, new Error("Нет связи с сервисом управления постами", "connection"));
                            case System.Net.HttpStatusCode.RequestTimeout:
                                _logger.LogError($"postrc request timed out. wash = {washCode}" + Environment.NewLine);
                                continue;
                            //return StatusCode(424, new Error("Нет связи с сервисом управления постами", "connection"));
                            default:
                                _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                                continue;
                                //return StatusCode(424, new Error("Нет связи с сервисом управления постами", "service"));
                        }
                    }
                    try
                    {
                        SqlHelper.WriteCardWash(cardNum, washCode);
                    }
                    catch (Exception e)
                    {
                        switch (e.Message)
                        {
                            case "constraint":
                                _logger.LogError($"Карта {cardNum} уже присоединена к мойке {washCode}" + Environment.NewLine);
                                continue;
                            //return StatusCode(500, new Error("Карта уже отправлена на мойку", "db"));
                            case "db":
                                _logger.LogError("Ошибка при записи в базу: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                                continue;
                            //return StatusCode(500, new Error("Ошибка записи в базу", "db"));
                            default:
                                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                                continue;
                                //return StatusCode(500, new Error("Произошла внутренняя ошибка сервера", "unexpexted"));
                        }
                    }
                }

                return Ok(SqlHelper.GetTechCardsByGroup(groupCode));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpexted"));
            }
        }
    }
}
