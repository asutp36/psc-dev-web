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
    public class AcquiringController : ControllerBase
    {
        ILogger<AcquiringController> _logger;
        IConfiguration _config;

        public AcquiringController(ILogger<AcquiringController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<WashHappyHourViewModel>))]
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
                List<WashAcquiringViewModel> result = new List<WashAcquiringViewModel>();
                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/acquiring/{w.code}");

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

                    var washResult = JsonConvert.DeserializeObject<WashAcquiringViewModel>(response.ResultMessage);

                    result.Add(washResult);
                }

                if (result.Count < 1)
                {
                    _logger.LogError($"Ни с одной мойки не получилось получить настройки эквайринга для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить настройки эквайринга с моек", "fail"));
                }
                    
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на посту по коду")]
        [SwaggerResponse(200, Type = typeof(PostAcquiringViewModel))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найден пост")]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Нет связи с постом")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpGet("post/{post}")]

        public IActionResult GetByPost(string post)
        {
            try
            {
                HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/acquiring/post/{post}");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"postrc не нашёл пост {post}" + Environment.NewLine);
                            return NotFound(new Error("Не найден пост", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось получить текущие настройки эквайринга с поста", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с постом {post}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с постом", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось получить настройки эквайринга с поста", "unexpected"));
                    }

                PostAcquiringViewModel result = JsonConvert.DeserializeObject<PostAcquiringViewModel>(response.ResultMessage);

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(WashAcquiringViewModel))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдена мойка")]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Не удалось получить данные с мойки")]
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

                HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/acquiring/{wash}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"postrc не нашёл мойку {wash}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось получить настройки эквайринга с мойки", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {wash}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с мойкой", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось получить настройки эквайринга с мойки", "unexpected"));
                    }
                }

                var result = JsonConvert.DeserializeObject<WashAcquiringViewModel>(response.ResultMessage);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойках по коду региона")]
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
                if (washes.Count <= 0)
                {
                    _logger.LogError($"Не найдены коды моек в регионе {region}" + Environment.NewLine);
                    return NotFound(new Error("Не найдены мойки", "badvalue"));
                }

                List<WashAcquiringViewModel> result = new List<WashAcquiringViewModel>();
                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/acquiring/{w.code}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"По мойке {w.code} не удалось получить текущие скидки. postrc response: " + response.ResultMessage);
                        //return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                    }

                    //string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                    var washResult = JsonConvert.DeserializeObject<WashAcquiringViewModel>(response.ResultMessage);

                    result.Add(washResult);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Установить настройки эквайринга на мойках по постам")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpPost()]
        public IActionResult Set(List<PostAcquiringViewModel> model)
        {
            try
            {
                List<SetParameterResultPost> result = new List<SetParameterResultPost>();
                foreach (PostAcquiringViewModel discount in model)
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
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
