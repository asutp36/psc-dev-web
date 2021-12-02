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
        [SwaggerResponse(200, Type = typeof(List<RegionParameter<AcquiringModel>>))]
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
                List<WashParameter<AcquiringModel>> result = new List<WashParameter<AcquiringModel>>();
                bool returnError = true;

                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/acquiring/wash/{w.code}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var emptyWash = new WashParameter<AcquiringModel> { washCode = w.code, washName = w.name };
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

                    var washResult = JsonConvert.DeserializeObject<WashParameter<AcquiringModel>>(response.ResultMessage);
                    washResult.washName = w.name;
                    result.Add(washResult);

                    returnError = false;
                }

                if (returnError)
                {
                    _logger.LogError($"Ни с одной мойки не получилось получить текущие настройки эквайринга для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить текущие настройки эквайринга с моек", "connection"));
                }

                return Ok(ParameterToRegion<AcquiringModel>.WashesToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(RegionParameter<AcquiringModel>))]
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
                    return NotFound(new Error("Не найдена мойка.", "badvalue"));
                }

                HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/acquiring/wash/{wash}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"postrc не нашёл мойку {wash}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка.", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {wash}" + Environment.NewLine);
                            return StatusCode(424, new Error($"Не удалось соединиться с мойкой {wash}.", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами.", "connection"));
                        case System.Net.HttpStatusCode.RequestTimeout:
                            _logger.LogError($"postrc Request timed out. wash = {wash}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами.", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                    }

                var result = JsonConvert.DeserializeObject<WashParameter<AcquiringModel>>(response.ResultMessage);
                result.washName = SqlHelper.GetWashByCode(wash).name;

                return Ok(ParameterToRegion<AcquiringModel>.WashToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Отправка новых настроек эквайринга на несколько постов")]
        [SwaggerResponse(200, Type = typeof(List<SetParameterResultPost>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpPost("set/posts")]
        public IActionResult SetRateManyPosts(SetPostsParameter<AcquiringModel> model)
        {
            try
            {
                List<SetParameterResultPost> result = new List<SetParameterResultPost>();
                foreach (string post in model.posts)
                {
                    PostParameter<AcquiringModel> param = new PostParameter<AcquiringModel> { postCode = post, value = model.value };

                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/acquiring/set/post", JsonConvert.SerializeObject(param));
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.NotFound:
                                _logger.LogError($"Не найден пост {post}" + Environment.NewLine);
                                break;
                            case System.Net.HttpStatusCode.InternalServerError:
                                _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                                break;
                            case (System.Net.HttpStatusCode)424:
                                _logger.LogError($"Не удалось соединиться с постом {post}" + Environment.NewLine);
                                break;
                            case (System.Net.HttpStatusCode)0:
                                _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                                break;
                            default:
                                _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                                break;
                        }
                    }

                    result.Add(new SetParameterResultPost { post = post, result = response });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Отправка новых настрек эквайрнга на мойку")]
        [SwaggerResponse(200, Type = typeof(SetParameterResultWash))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpPost("set/wash")]
        public IActionResult SetRateWash(SetWashParameter<AcquiringModel> model)
        {
            try
            {
                SetParameterResultWash result = new SetParameterResultWash();
                result.wash = model.washCode;

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/acquiring/set/wash", JsonConvert.SerializeObject(model));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"Не найдена мойка {model.washCode}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка.", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {model.washCode}" + Environment.NewLine);
                            return StatusCode(424, new Error($"Не удалось соединиться с мойкой {model.washCode}.", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами.", "connection"));
                        case System.Net.HttpStatusCode.RequestTimeout:
                            _logger.LogError($"postrc Request timed out. wash = {model.washCode}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами.", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                    }
                }

                result = JsonConvert.DeserializeObject<SetParameterResultWash>(response.ResultMessage);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        [HttpGet("fake")]
        public IActionResult GetFake()
        {
            AcquiringModel a1 = new AcquiringModel { BankAmountMax = 700, BankAmountMin = 50, BankAmountStep = 10 };
            AcquiringModel a2 = new AcquiringModel { BankAmountMax = 1000, BankAmountMin = 30, BankAmountStep = 10 };

            PostParameter<AcquiringModel> p131 = new PostParameter<AcquiringModel> { postCode = "13-1", value = a1 };
            PostParameter<AcquiringModel> p132 = new PostParameter<AcquiringModel> { postCode = "13-2", value = a1 };
            PostParameter<AcquiringModel> p133 = new PostParameter<AcquiringModel> { postCode = "13-3", value = a1 };
            PostParameter<AcquiringModel> p134 = new PostParameter<AcquiringModel> { postCode = "13-4", value = a1 };

            WashParameter<AcquiringModel> w13 = new WashParameter<AcquiringModel>
            {
                washCode = "M13",
                washName = "М13, всё как надо",
                posts = new List<PostParameter<AcquiringModel>> { p131, p132, p133, p134 },
                value = null
            };

            PostParameter<AcquiringModel> p141 = new PostParameter<AcquiringModel> { postCode = "14-1", value = a1 };
            PostParameter<AcquiringModel> p142 = new PostParameter<AcquiringModel> { postCode = "14-2", value = a2 };
            PostParameter<AcquiringModel> p143 = new PostParameter<AcquiringModel> { postCode = "14-3", value = a1 };
            PostParameter<AcquiringModel> p144 = new PostParameter<AcquiringModel> { postCode = "14-4", value = a1 };

            WashParameter<AcquiringModel> w14 = new WashParameter<AcquiringModel>
            {
                washCode = "M14",
                washName = "М14, второй пост отличается",
                posts = new List<PostParameter<AcquiringModel>> { p141, p142, p143, p144 },
                value = null
            };

            WashParameter<AcquiringModel> w15 = new WashParameter<AcquiringModel>
            {
                washCode = "M15",
                washName = "М15, пустая",
                posts = null,
                value = null
            };

            RegionParameter<AcquiringModel> r1 = new RegionParameter<AcquiringModel>
            {
                regionCode = 1,
                regionName = "Первый регион",
                washes = new List<WashParameter<AcquiringModel>> { w13 }
            };

            RegionParameter<AcquiringModel> r2 = new RegionParameter<AcquiringModel>
            {
                regionCode = 2,
                regionName = "Второй регион",
                washes = new List<WashParameter<AcquiringModel>> { w14, w15 }
            };

            return Ok(new List<RegionParameter<AcquiringModel>> { r1, r2 });
        }

    }
}
