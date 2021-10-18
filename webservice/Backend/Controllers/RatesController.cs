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
        [SwaggerResponse(200, Type = typeof(List<RegionParameter<List<RateViewModel>>>))]
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
                        var emptyWash = new WashParameter<RatesModel> { washCode = w.code };
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

                    result.Add(washResult);
                    returnError = false;
                }

                if (returnError)
                {
                    _logger.LogError($"Ни с одной мойки не получилось получить текущие тарифы для пользователя {User.Identity.Name}" + Environment.NewLine);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы с моек", "fail"));
                }

                //return Ok(WashesToRegionRateModel(result));
                return Ok(WashesToRegion(result));
            }
            catch(Exception e)
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
                posts = new List<PostParameter<RatesModel>> { p141, p142, p143, p144 },
                value = new RatesModel()
            };

            WashParameter<RatesModel> w15 = new WashParameter<RatesModel>
            {
                washCode = "M15",
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

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на посте по коду")]
        [SwaggerResponse(200, Type = typeof(RegionRatesModel))]
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
                HttpResponse response = HttpSender.SendGet(_config["Services:postrc"] + $"api/rates/post/{post}");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.NotFound:
                            _logger.LogError($"postrc не нашёл пост {post}" + Environment.NewLine);
                            return NotFound(new Error("Не найден пост", "badvalue"));
                        case System.Net.HttpStatusCode.InternalServerError:
                            _logger.LogError("Внутренняя ошибка на сервиса postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось получить текущие тарифы с поста", "service"));
                        case (System.Net.HttpStatusCode)424:
                            _logger.LogError($"Не удалось соединиться с постом {post}" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с постом", "connection"));
                        case (System.Net.HttpStatusCode)0:
                            _logger.LogError("Нет связи с сервисом postrc" + Environment.NewLine);
                            return StatusCode(424, new Error("Нет связи с сервисом", "connection"));
                        default:
                            _logger.LogError("Ответ postrc: " + JsonConvert.SerializeObject(response) + Environment.NewLine);
                            return StatusCode(424, new Error("Не удалось получить тарифы с поста", "unexpected"));
                    }

                PostRatesModel result = JsonConvert.DeserializeObject<PostRatesModel>(response.ResultMessage);

                return Ok(PostToRegionRateModel(result));
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(RegionRatesModel))]
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
                var result = JsonConvert.DeserializeObject<WashParameter<RatesModel>>(response.ResultMessage);

                return Ok(WashToRegion(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках по коду региона")]
        [SwaggerResponse(200, Type = typeof(List<RegionRatesModel>))]
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

                return Ok(WashesToRegionRateModel(result));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        private List<RegionRatesModel> WashesToRegionRateModel(List<WashRatesViewModel> washes)
        {
            List<RegionRatesModel> result = new List<RegionRatesModel>();

            foreach (WashRatesViewModel w in washes)
            {
                RegionViewModel region = SqlHelper.GetRegionByWash(w.wash);
                RegionRatesModel rrm = result.Find(x => x.regionCode == region.code);

                if (rrm == null)
                {
                    rrm = new RegionRatesModel
                    {
                        regionCode = region.code,
                        regionName = region.name,
                        washes = new List<WashRatesViewModel>()
                    };

                    rrm.washes.Add(w);
                }
                else
                {
                    result.Remove(rrm);
                    rrm.washes.Add(w);
                }

                result.Add(rrm);
            }
            return result;
        }

        private List<RegionParameter<RatesModel>> WashesToRegion(List<WashParameter<RatesModel>> washes)
        {
            List<RegionParameter<RatesModel>> result = new List<RegionParameter<RatesModel>>();

            foreach (WashParameter<RatesModel> w in washes)
            {
                RegionViewModel region = SqlHelper.GetRegionByWash(w.washCode);
                RegionParameter<RatesModel> rrm = result.Find(x => x.regionCode == region.code);

                if (rrm == null)
                {
                    rrm = new RegionParameter<RatesModel>
                    {
                        regionCode = region.code,
                        regionName = region.name,
                        washes = new List<WashParameter<RatesModel>>()
                    };

                    rrm.washes.Add(w);
                }
                else
                {
                    result.Remove(rrm);
                    rrm.washes.Add(w);
                }

                result.Add(rrm);
            }
            return result;
        }

        private RegionParameter<RatesModel> WashToRegion(WashParameter<RatesModel> wash)
        {
            RegionViewModel region = SqlHelper.GetRegionByWash(wash.washCode);
            RegionParameter<RatesModel> result = new RegionParameter<RatesModel>
            {
                regionCode = region.code,
                regionName = region.name,
                washes = new List<WashParameter<RatesModel>>()
            };

            result.washes.Add(wash);

            return result;
        }

        private RegionRatesModel WashToRegionRateModel(WashRatesViewModel wash)
        {
            RegionViewModel region = SqlHelper.GetRegionByWash(wash.wash);
            RegionRatesModel result = new RegionRatesModel
            {
                regionCode = region.code,
                regionName = region.name,
                washes = new List<WashRatesViewModel>()
            };

            result.washes.Add(wash);

            return result;
        }

        private RegionRatesModel PostToRegionRateModel(PostRatesModel post)
        {
            RegionViewModel region = SqlHelper.GetRegionByPost(post.post);
            WashViewModel wash = SqlHelper.GetWashByPost(post.post);
            RegionRatesModel result = new RegionRatesModel
            {
                regionCode = region.code,
                regionName = region.name,
                washes = new List<WashRatesViewModel>()
            };

            WashRatesViewModel wrm = new WashRatesViewModel
            {
                wash = wash.code,
                rates = new List<PostRatesModel>()
            };

            wrm.rates.Add(post);
            result.washes.Add(wrm);

            return result;
        }
    }
}
