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
    public class HappyHourController : ControllerBase
    {
        ILogger<HappyHourController> _logger;
        IConfiguration _config;

        public HappyHourController(ILogger<HappyHourController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие скидки на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<WashHappyHourViewModel>))]
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
                List<WashParameter<HappyHourModel>> result = new List<WashParameter<HappyHourModel>>();
                bool returnError = true;

                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/happyhour/wash/{w.code}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var emptyWash = new WashParameter<HappyHourModel> { washCode = w.code, washName = w.name };
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.NotFound:
                                _logger.LogError($"postrc не нашёл мойку {w.code}" + Environment.NewLine);
                                break;
                            case System.Net.HttpStatusCode.InternalServerError:
                                _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                                break;
                            case (System.Net.HttpStatusCode)424:
                                _logger.LogError($"Не удалось соединиться с мойкой {w.code}" + Environment.NewLine);
                                break;
                            case (System.Net.HttpStatusCode)0:
                                _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                                break;
                            case System.Net.HttpStatusCode.RequestTimeout:
                                _logger.LogError($"postrc Request timed out. wash = {w.code}" + Environment.NewLine);
                                break;
                            default:
                                _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                                break;
                        }

                        result.Add(emptyWash);
                        continue;
                    }

                    var washResult = JsonConvert.DeserializeObject<WashParameter<HappyHourModel>>(response.ResultMessage);
                    washResult.washName = w.name;
                    result.Add(washResult);

                    returnError = false;
                }
                if (returnError)
                {
                    _logger.LogError($"Ни с одной мойки не получилось получить настройки скидок для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить настройки скидок с моек", "fail"));
                }

                return Ok(ParameterToRegion<HappyHourModel>.WashesToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие скидки на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(WashHappyHourViewModel))]
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

                HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/happyhour/wash/{wash}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"postrc не нашёл мойку {wash}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {wash}" + Environment.NewLine);
                            return StatusCode(424, new Error($"Не удалось соединиться с мойкой {wash}", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами", "connection"));
                        case System.Net.HttpStatusCode.RequestTimeout:
                            _logger.LogError($"postrc Request timed out. wash = {wash}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами", "service"));
                    }
                }

                var result = JsonConvert.DeserializeObject<WashParameter<HappyHourModel>>(response.ResultMessage);

                return Ok(ParameterToRegion<HappyHourModel>.WashToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие скидки на мойках по коду региона")]
        [SwaggerResponse(200, Type = typeof(List<WashHappyHourViewModel>))]
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
                if (washes.Count <= 0)
                {
                    _logger.LogError($"Не найдены коды моек в регионе {region}" + Environment.NewLine);
                    return NotFound(new Error("Не найдены мойки", "badvalue"));
                }

                List<WashHappyHourViewModel> result = new List<WashHappyHourViewModel>();
                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/happyhour/wash/{w.code}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"По мойке {w.code} не удалось получить текущие скидки. postrc response: " + response.ResultMessage);
                        //return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                    }

                    //string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                    var posts = JsonConvert.DeserializeObject<List<PostHappyHourViewModel>>(response.ResultMessage);

                    result.Add(new WashHappyHourViewModel
                    {
                        wash = w.code,
                        posts = posts
                    });
                }    

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        [Authorize]
        [HttpPost("post")]
        public IActionResult Set(List<PostHappyHourViewModel> model)
        {
            try
            {
                List<SetParameterResultPost> result = new List<SetParameterResultPost>();
                foreach(PostHappyHourViewModel discount in model)
                {
                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/postdiscount/set", JsonConvert.SerializeObject(discount));
                    result.Add(new SetParameterResultPost
                    {
                        post = discount.post,
                        result = response
                    });
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
