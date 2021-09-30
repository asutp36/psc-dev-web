using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
    public class RatesController : ControllerBase
    {
        ILogger<RatesController> _logger;
        IConfiguration _config;

        public RatesController(ILogger<RatesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Отправка новых тарифов на мойки")]
        [SwaggerResponse(200, Type = typeof(List<SetParameterResult>))]
        #endregion
        [HttpPost]
        public IActionResult SetRates(ChangeRateViewModel model)
        {
            try
            {
                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/rates/change/wash", JsonConvert.SerializeObject(model));
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось подключиться к сервису управления постами", "service"));
                }

                List<SetParameterResult> result = JsonConvert.DeserializeObject<List<SetParameterResult>>(response.ResultMessage);

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Отправка новых тарифов на один пост")]
        [SwaggerResponse(200, Type = typeof(SetParameterResultPost))]
        [SwaggerResponse(404, Type = typeof(Error))]
        [SwaggerResponse(424, Type = typeof(Error))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpPost("post")]
        public IActionResult SetRatePost(ChangeRatePostViewModel model)
        {
            try
            {
                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/rates/change/post", JsonConvert.SerializeObject(model));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    switch (response.StatusCode) 
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"Не найден пост {model.PostCode}" + Environment.NewLine);
                            return NotFound(new Error($"Пост {model.PostCode} не найден", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            break;
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с постом {model.PostCode}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с постом", "connection "));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось подключиться к сервису управления постами", "connection"));
                    }

                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось подключиться к сервису управления постами", "service"));
                }

                SetParameterResultPost result = JsonConvert.DeserializeObject<SetParameterResultPost>(response.ResultMessage);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Не удалось получить данные ни с одной мойки")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                UserInfo uInfo = new UserInfo(User.Claims.ToList());

                List<WashViewModel> washes = uInfo.GetWashes();
                List<WashRatesViewModel> result = new List<WashRatesViewModel>();

                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/rates/wash/{w.code}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.NotFound:
                                _logger.LogError($"postrc не нашёл мойку {w.code}" + Environment.NewLine);
                                continue;
                            case System.Net.HttpStatusCode.InternalServerError:
                                _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                                continue;
                            case (System.Net.HttpStatusCode)424:
                                _logger.LogError($"Не удалось соединиться с мойкой {w.code}" + Environment.NewLine);
                                continue;
                            case (System.Net.HttpStatusCode)0:
                                _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                                continue;
                            default:
                                _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                                continue;
                        }

                    var washResult = JsonConvert.DeserializeObject<WashRatesViewModel>(response.ResultMessage);

                    result.Add(washResult);
                }

                if (result.Count < 1)
                {
                    _logger.LogError($"Ни с одной мойки не получилось получить текущие тарифы для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы с моек", "fail"));
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(WashRatesViewModel))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдена мойка")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpGet("wash/{wash}")]
        public IActionResult GetByWash(string wash)
        {
            try
            {
                if (!SqlHelper.IsWashExists(wash))
                {
                    _logger.LogError($"Не найдена мойка {wash}" + Environment.NewLine);
                    return NotFound(new Error("Не найдена мойка", "badvalue"));
                }

                HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/rates/wash/{wash}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                }
                //string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                var result = JsonConvert.DeserializeObject<WashRatesViewModel>(response.ResultMessage);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках по коду региона")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдены мойки по коду региона")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("region/{region}")]
        public IActionResult GetByRegion(int region)
        {
            try
            {
                List<WashViewModel> washes = SqlHelper.GetWashesByRegion(region);
                if(washes.Count <= 0)
                {
                    _logger.LogError($"Не найдены коды моек в регионе {region}" + Environment.NewLine);
                    return NotFound(new Error("Не найдены мойки", "badvalue"));
                }

                List<string> washCodes = new List<string>();
                foreach(WashViewModel w in washes)
                    washCodes.Add(w.code);

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/rates/manywash", JsonConvert.SerializeObject(washCodes));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                }
                //string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                var result = JsonConvert.DeserializeObject<List<WashRatesViewModel>>(response.ResultMessage);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
