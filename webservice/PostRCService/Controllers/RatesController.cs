using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostRCService.Controllers.BindingModels;
using PostRCService.Controllers.Helpers;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpResponse = PostRCService.Controllers.Helpers.HttpResponse;

namespace PostRCService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatesController : ControllerBase
    {
        ILogger<RatesController> _logger;

        public RatesController(ILogger<RatesController> logger)
        {
            _logger = logger;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках по коду мойки")]
        [SwaggerResponse(200, Type = typeof(WashRates))]
        [SwaggerResponse(404, Description = "Не найдена мойка")]
        [SwaggerResponse(424, Description = "Нет связи с мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpGet("bywash/{washCode}")]
        public IActionResult GetByWash(string washCode)
        {
            try
            {
                if (!SqlHelper.IsWashExists(washCode))
                {
                    _logger.LogError($"Не найдена мойка {washCode}" + Environment.NewLine);
                    return NotFound();
                }
                WashRates result = new WashRates();
                result.wash = washCode;
                result.rates = new List<PostRates>();

                List<string> postCodes = SqlHelper.GetPostCodes(washCode);
                foreach(string p in postCodes)
                {
                    PostRates postRates = new PostRates();
                    postRates.post = p;
                    postRates.prices = new List<FunctionRate>();

                    string ip = SqlHelper.GetPostIp(p);
                    if(ip == null)
                    {
                        _logger.LogError($"Не найден ip поста {p}");
                        continue;
                    }

                    HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (response.StatusCode == 0)
                        {
                            _logger.LogInformation($"Нет соединения с постом {p}");
                            continue;
                        }

                        _logger.LogError($"Ответ поста {p}: {JsonConvert.SerializeObject(response)}");
                        continue;
                    }

                    postRates.prices = JsonConvert.DeserializeObject<List<FunctionRate>>(response.ResultMessage);
                    result.rates.Add(postRates);
                }

                if(result.rates.Count < 1)
                {
                    _logger.LogInformation($"Нет связи с мойкой {washCode}" + Environment.NewLine);
                    return StatusCode(424);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на нескольких мойках по кодам")]
        [SwaggerResponse(200, Type = typeof(List<WashRates>))]
        [SwaggerResponse(404, Description = "Не найдена мойка")]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("bymanywash")]
        public IActionResult GetByManyWash([FromBody]string[] washes)
        {
            try
            {
                List<WashRates> result = new List<WashRates>();

                foreach (string wash in washes)
                {
                    if (!SqlHelper.IsWashExists(wash))
                    {
                        _logger.LogError($"Не найдена мойка {wash}" + Environment.NewLine);
                        continue;
                    }

                    WashRates washRate = new WashRates();
                    washRate.wash = wash;
                    washRate.rates = new List<PostRates>();

                    List<string> postCodes = SqlHelper.GetPostCodes(wash);
                    foreach (string p in postCodes)
                    {
                        PostRates postRates = new PostRates();
                        postRates.post = p;
                        postRates.prices = new List<FunctionRate>();

                        string ip = SqlHelper.GetPostIp(p);
                        if (ip == null)
                        {
                            _logger.LogError($"Не найден ip поста {p}");
                            continue;
                        }

                        HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            if (response.StatusCode == 0)
                            {
                                _logger.LogInformation($"Нет соединения с постом {p}");
                                continue;
                            }

                            _logger.LogError($"Ответ поста {p}: {JsonConvert.SerializeObject(response)}");
                            continue;
                        }

                        postRates.prices = JsonConvert.DeserializeObject<List<FunctionRate>>(response.ResultMessage);
                        washRate.rates.Add(postRates);
                    }

                    if (washRate.rates.Count < 1)
                    {
                        _logger.LogInformation($"Нет связи с мойкой {wash}" + Environment.NewLine);
                    }

                    result.Add(washRate);
                }

                if(result.Count < 1)
                {
                    _logger.LogInformation($"Нет связи ни с одной из моек ({JsonConvert.SerializeObject(washes)})" + Environment.NewLine);
                    return StatusCode(424);
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "")]
        [SwaggerResponse(200, Type = typeof(int))]
        [SwaggerResponse(404, Description = "")]
        [SwaggerResponse(424, Description = "")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("changebywash")]
        public IActionResult ChangeRatesByWash(ChangeRatesWash change)
        {
            try
            {
                List<ChangeParameterWashResult> result = new List<ChangeParameterWashResult>();
                foreach (string wash in change.washes)
                {
                    if (!SqlHelper.IsWashExists(wash))
                    {
                        _logger.LogError($"Не найдена мойка {wash}");
                        continue;
                    }

                    ChangeParameterWashResult washResult = new ChangeParameterWashResult
                    {
                        wash = wash,
                        posts = new List<ChangeParameterResult>()
                    };

                    List<string> posts = SqlHelper.GetPostCodes(wash);
                    foreach(string post in posts)
                    {
                        string ip = SqlHelper.GetPostIp(post);
                        if (ip == null)
                        {
                            _logger.LogError($"Не найден ip поста {post}");
                            continue;
                        }

                        HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                        if(response.StatusCode != System.Net.HttpStatusCode.OK)
                            if (response.StatusCode == 0)
                                _logger.LogInformation($"Нет соединения с постом {post}");
                            else
                                _logger.LogError($"Ответ поста {post}: {JsonConvert.SerializeObject(response)}");

                        washResult.posts.Add(new ChangeParameterResult
                        {
                            post = post,
                            result = response
                        });
                    }

                    result.Add(washResult);
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }
    }
}
