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

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<RegionParameter<RatesModel>>))]
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
                List<WashParameter<RatesModel>> result = new List<WashParameter<RatesModel>>();
                bool returnError = true;

                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/rates/wash/{w.code}");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var emptyWash = new WashParameter<RatesModel> { washCode = w.code, washName = w.name };
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

                    var washResult = JsonConvert.DeserializeObject<WashParameter<RatesModel>>(response.ResultMessage);
                    washResult.washName = w.name;
                    result.Add(washResult);

                    returnError = false;
                }

                if (returnError)
                {
                    _logger.LogError($"Ни с одной мойки не получилось получить текущие тарифы для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы с моек", "fail"));
                }

                return Ok(ParameterToRegion<RatesModel>.WashesToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(RegionParameter<RatesModel>))]
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
                
                var result = JsonConvert.DeserializeObject<WashParameter<RatesModel>>(response.ResultMessage);
                result.washName = SqlHelper.GetWashByCode(wash).name;

                return Ok(ParameterToRegion<RatesModel>.WashToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Отправка новых тарифов на несколько постов")]
        [SwaggerResponse(200, Type = typeof(List<SetParameterResultPost>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpPost("set/posts")]
        public IActionResult SetRateManyPosts(SetPostsParameter<RatesModel> model)
        {
            try
            {
                List<SetParameterResultPost> result = new List<SetParameterResultPost>();
                foreach (string post in model.posts)
                {
                    PostParameter<RatesModel> param = new PostParameter<RatesModel> { postCode = post, value = model.value };

                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/rates/set/post", JsonConvert.SerializeObject(param));
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
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotation
        [SwaggerOperation(Summary = "Отправка новых тарифов на мойку")]
        [SwaggerResponse(200, Type = typeof(SetParameterResultWash))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        //[Authorize]
        [HttpPost("set/wash")]
        public IActionResult SetRateWash(SetWashParameter<RatesModel> model)
        {
            try
            {
                SetParameterResultWash result = new SetParameterResultWash();
                result.wash = model.washCode;

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/rates/set/wash", JsonConvert.SerializeObject(model));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"Не найдена мойка {model.washCode}" + Environment.NewLine);
                            return NotFound(new Error("Не найдена мойка", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Произошла ошибка в сервисе управления постами", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с мойкой {model.washCode}" + Environment.NewLine);
                            return StatusCode(424, new Error($"Не удалось соединиться с мойкой {model.washCode}", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами", "connection"));
                        case System.Net.HttpStatusCode.RequestTimeout:
                            _logger.LogError($"postrc Request timed out. wash = {model.washCode}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом управления постами", "service"));
                    }
                }

                result = JsonConvert.DeserializeObject<SetParameterResultWash>(response.ResultMessage);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
}

        [HttpGet("fake")]
        public IActionResult GetFake()
        {
            RateViewModel intensive = new RateViewModel
            {
                Code = "intensive",
                Func = "Мойка дисков",
                Rate = 18
            };

            RateViewModel active = new RateViewModel
            {
                Code = "active",
                Func = "Активная химия",
                Rate = 20
            };

            RateViewModel foam = new RateViewModel
            {
                Code = "foam",
                Func = "Пена",
                Rate = 18
            };

            RateViewModel foam1 = new RateViewModel
            {
                Code = "foam",
                Func = "Пена",
                Rate = 14
            };

            RateViewModel shampoo = new RateViewModel
            {
                Code = "shampoo",
                Func = "Вода-шампунь",
                Rate = 16
            };

            RateViewModel brush = new RateViewModel
            {
                Code = "brush",
                Func = "Щетка",
                Rate = 16
            };

            RateViewModel brush1 = new RateViewModel
            {
                Code = "brush",
                Func = "Щетка",
                Rate = 10
            };

            RateViewModel water = new RateViewModel
            {
                Code = "water",
                Func = "Вода",
                Rate = 14
            };

            RateViewModel wax = new RateViewModel
            {
                Code = "wax",
                Func = "Воск",
                Rate = 20
            };

            RateViewModel osmosis = new RateViewModel
            {
                Code = "osmosis",
                Func = "Осмос",
                Rate = 20
            };

            RateViewModel stop = new RateViewModel
            {
                Code = "stop",
                Func = "Пауза",
                Rate = 2
            };


            PostParameter<RatesModel> p131 = new PostParameter<RatesModel>
            {
                postCode = "13-1",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam, shampoo, brush, water, wax, osmosis, stop } }
            };

            PostParameter<RatesModel> p132 = new PostParameter<RatesModel>
            {
                postCode = "13-2",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam, shampoo, brush, water, wax, osmosis, stop } }
            };

            PostParameter<RatesModel> p133 = new PostParameter<RatesModel>
            {
                postCode = "13-2",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam, shampoo, brush, water, wax, osmosis, stop } }
            };

            WashParameter<RatesModel> w13 = new WashParameter<RatesModel>
            {
                washCode = "M13",
                washName = "Мойка М13, всё норм на ней",
                posts = new List<PostParameter<RatesModel>> { p131, p132, p133 },
                value = new RatesModel()
            };

            PostParameter<RatesModel> p141 = new PostParameter<RatesModel>
            {
                postCode = "14-1",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam, shampoo, brush, water, wax, osmosis, stop } }
            };

            PostParameter<RatesModel> p142 = new PostParameter<RatesModel>
            {
                postCode = "14-2",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam, shampoo, brush1, water, wax, osmosis, stop } }
            };

            PostParameter<RatesModel> p143 = new PostParameter<RatesModel>
            {
                postCode = "14-2",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam, shampoo, brush, water, wax, osmosis, stop } }
            };

            PostParameter<RatesModel> p144 = new PostParameter<RatesModel>
            {
                postCode = "14-2",
                value = new RatesModel() { rates = new List<RateViewModel> { intensive, active, foam1, shampoo, brush1, water, wax, osmosis, stop } }
            };

            WashParameter<RatesModel> w14 = new WashParameter<RatesModel>
            {
                washCode = "M14",
                washName = "Мойка М14, разные посты",
                posts = new List<PostParameter<RatesModel>> { p141, p142, p143, p144 },
                value = new RatesModel()
            };

            WashParameter<RatesModel> w15 = new WashParameter<RatesModel>
            {
                washCode = "M15",
                washName = "Мойка М15, пустые посты и тарифы",
                posts = null,
                value = null
            };

            RegionParameter<RatesModel> r1 = new RegionParameter<RatesModel>
            {
                regionCode = 1,
                regionName = "Первый регион",
                washes = new List<WashParameter<RatesModel>> { w13 }
            };

            RegionParameter<RatesModel> r2 = new RegionParameter<RatesModel>
            {
                regionCode = 2,
                regionName = "Второй регион",
                washes = new List<WashParameter<RatesModel>> { w14, w15 }
            };

            return Ok(new List<RegionParameter<RatesModel>> { r1, r2 });
        }
    }

}
