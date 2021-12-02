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
        [SwaggerOperation(Summary = "Получить текущие настройки скидок на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<RegionParameter<HappyHourModel>>))]
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
                    _logger.LogError($"Ни с одной мойки не получилось получить текущие настройки скидок для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить текущие настройки скидок с моек.", "connection"));
                }

                return Ok(ParameterToRegion<HappyHourModel>.WashesToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки скидок на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(RegionParameter<HappyHourModel>))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдена мойка")]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Проблема с доступом к постам")]
        [SwaggerResponse(500, Type = typeof(Error))]
        [SwaggerResponse(503, Type = typeof(Error), Description = "Проблема с доступом к сервису управления постами")]
        #endregion
        [Authorize]
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
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"postrc не нашёл мойку {wash}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка.", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(503, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {wash}" + Environment.NewLine);
                            return StatusCode(424, new Error($"Не удалось соединиться с мойкой {wash}.", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(503, new Error("Нет связи с сервисом управления постами.", "connection"));
                        case System.Net.HttpStatusCode.RequestTimeout:
                            _logger.LogError($"postrc Request timed out. wash = {wash}" + Environment.NewLine);
                            return StatusCode(503, new Error("Нет связи с сервисом управления постами.", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(503, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                    }

                var result = JsonConvert.DeserializeObject<WashParameter<HappyHourModel>>(response.ResultMessage);
                result.washName = SqlHelper.GetWashByCode(wash).name;

                return Ok(ParameterToRegion<HappyHourModel>.WashToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Изменение настроек скидок на нескольких постах")]
        [SwaggerResponse(200, Type = typeof(List<SetParameterResultPost>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpPost("set/posts")]
        public IActionResult SetRateManyPosts(SetPostsParameter<HappyHourModel> model)
        {
            try
            {
                List<SetParameterResultPost> result = new List<SetParameterResultPost>();
                foreach (string post in model.posts)
                {
                    PostParameter<HappyHourModel> param = new PostParameter<HappyHourModel> { postCode = post, value = model.value };

                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/happyhour/set/post", JsonConvert.SerializeObject(param));
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
        [SwaggerOperation(Summary = "изменение настроек скидок на мойке")]
        [SwaggerResponse(200, Type = typeof(SetParameterResultWash))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдена мойка")]
        [SwaggerResponse(424, Type = typeof(Error), Description = "Проблема с доступом к постам")]
        [SwaggerResponse(500, Type = typeof(Error))]
        [SwaggerResponse(503, Type = typeof(Error), Description = "Проблема с доступом к сервису управления постами")]
        #endregion
        [Authorize]
        [HttpPost("set/wash")]
        public IActionResult SetRateWash(SetWashParameter<HappyHourModel> model)
        {
            try
            {
                SetParameterResultWash result = new SetParameterResultWash();
                result.wash = model.washCode;

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/happyhour/set/wash", JsonConvert.SerializeObject(model));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"Не найдена мойка {model.washCode}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка.", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(503, new Error("Произошла ошибка в сервисе управления постами.", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {model.washCode}" + Environment.NewLine);
                            return StatusCode(424, new Error($"Не удалось соединиться с мойкой {model.washCode}.", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(503, new Error("Нет связи с сервисом управления постами.", "connection"));
                        case System.Net.HttpStatusCode.RequestTimeout:
                            _logger.LogError($"postrc Request timed out. wash = {model.washCode}" + Environment.NewLine);
                            return StatusCode(503, new Error("Нет связи с сервисом управления постами.", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(503, new Error("Произошла ошибка в сервисе управления постами.", "service"));
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
            HappyHourModel hh1 = new HappyHourModel { HappyHourBeg = 23, HappyHourEnd = 6, HappyHourSale = 30 };
            HappyHourModel hh2 = new HappyHourModel { HappyHourBeg = 0, HappyHourEnd = 5, HappyHourSale = 40 };

            PostParameter<HappyHourModel> p131 = new PostParameter<HappyHourModel>
            {
                postCode = "13-1",
                value = hh1
            };

            PostParameter<HappyHourModel> p132 = new PostParameter<HappyHourModel>
            {
                postCode = "13-2",
                value = hh1
            };

            PostParameter<HappyHourModel> p133 = new PostParameter<HappyHourModel>
            {
                postCode = "13-3",
                value = hh1
            };

            PostParameter<HappyHourModel> p134 = new PostParameter<HappyHourModel>
            {
                postCode = "13-4",
                value = hh1
            };

            WashParameter<HappyHourModel> w13 = new WashParameter<HappyHourModel>
            {
                washCode = "M13",
                washName = "М13, всё как надо",
                posts = new List<PostParameter<HappyHourModel>> { p131, p132, p133, p134 },
                value = null
            };

            PostParameter<HappyHourModel> p141 = new PostParameter<HappyHourModel>
            {
                postCode = "14-1",
                value = hh1
            };

            PostParameter<HappyHourModel> p142 = new PostParameter<HappyHourModel>
            {
                postCode = "14-2",
                value = hh1
            };

            PostParameter<HappyHourModel> p143 = new PostParameter<HappyHourModel>
            {
                postCode = "14-3",
                value = hh1
            };

            PostParameter<HappyHourModel> p144 = new PostParameter<HappyHourModel>
            {
                postCode = "14-4",
                value = hh2
            };

            WashParameter<HappyHourModel> w14 = new WashParameter<HappyHourModel>
            {
                washCode = "M14",
                washName = "М13, последний пост отличается",
                posts = new List<PostParameter<HappyHourModel>> { p141, p142, p143, p144 },
                value = new HappyHourModel()
            };

            WashParameter<HappyHourModel> w15 = new WashParameter<HappyHourModel>
            {
                washCode = "M15",
                washName = "М15, пустая",
                posts = null
            };

            RegionParameter<HappyHourModel> r1 = new RegionParameter<HappyHourModel>
            {
                regionCode = 1,
                regionName = "Первый регион",
                washes = new List<WashParameter<HappyHourModel>> { w13 }
            };

            RegionParameter<HappyHourModel> r2 = new RegionParameter<HappyHourModel>
            {
                regionCode = 2,
                regionName = "Второй регион",
                washes = new List<WashParameter<HappyHourModel>> { w14, w15 }
            };

            return Ok(new List<RegionParameter<HappyHourModel>> { r1, r2 });
        }
    }
}
