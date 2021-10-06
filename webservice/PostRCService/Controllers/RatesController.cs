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
        [SwaggerOperation(Summary = "Получить текущие тарифы на посте по коду")]
        [SwaggerResponse(200, Type = typeof(PostRates))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpGet("post/{post}")]
        public IActionResult GetByPost(string post)
        {
            try
            {
                if (!SqlHelper.IsPostExists(post))
                {
                    _logger.LogError($"Не найден пост {post}" + Environment.NewLine);
                    return NotFound();
                }

                string ip = SqlHelper.GetPostIp(post);
                if (ip == null)
                {
                    _logger.LogError($"Не найден ip поста {post}");
                    return NotFound();
                }

                PostRates result = new PostRates();
                result.post = post;
                result.prices = new List<FunctionRate>();

                //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");
                HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/rate/get");

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0)
                    {
                        _logger.LogInformation($"Нет соединения с постом {post}");
                        return StatusCode(424);
                    }

                    _logger.LogError($"Ответ поста {post}: {JsonConvert.SerializeObject(response)}");
                    return StatusCode(424);
                }

                result.prices = JsonConvert.DeserializeObject<List<FunctionRate>>(response.ResultMessage);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках по коду мойки")]
        [SwaggerResponse(200, Type = typeof(WashRates))]
        [SwaggerResponse(404, Description = "Не найдена мойка")]
        [SwaggerResponse(424, Description = "Нет связи с мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpGet("wash/{washCode}")]
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

                    //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");
                    HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/rate/get");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var emptyPost = new PostRates { post = p };
                        if (response.StatusCode == 0)
                        {
                            _logger.LogInformation($"Нет соединения с постом {p}");
                            result.rates.Add(emptyPost);
                            continue;
                        }

                        _logger.LogError($"Ответ поста {p}: {JsonConvert.SerializeObject(response)}");
                        result.rates.Add(emptyPost);
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
        [HttpPost("manywash")]
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

                        //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");
                        HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/rate/get");

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            var emptyPost = new PostRates { post = p };
                            if (response.StatusCode == 0)
                            {
                                _logger.LogInformation($"Нет соединения с постом {p}");
                                washRate.rates.Add(emptyPost);
                                continue;
                            }

                            _logger.LogError($"Ответ поста {p}: {JsonConvert.SerializeObject(response)}");
                            washRate.rates.Add(emptyPost);
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
        [SwaggerOperation(Summary = "Изменене тарифов на посту по коду")]
        [SwaggerResponse(200, Type = typeof(ChangeParameterResult))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("change/post")]
        public IActionResult ChangeRatesByPost(ChangeRatesPost change)
        {
            try
            {
                if (!SqlHelper.IsPostExists(change.postCode))
                {
                    _logger.LogError($"Не найден пост {change.postCode}" + Environment.NewLine);
                    return NotFound();
                }

                ChangeParameterResult result = new ChangeParameterResult();
                result.post = change.postCode;

                string ip = SqlHelper.GetPostIp(change.postCode);
                if (ip == null)
                {
                    _logger.LogError($"Не найден ip поста {change.postCode}");
                    return NotFound();
                }

                //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/rate", JsonConvert.SerializeObject(change.rates));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0)
                        _logger.LogInformation($"Нет соединения с постом {change.postCode}");
                    else
                        _logger.LogError($"Ответ поста {change.postCode}: {JsonConvert.SerializeObject(response)}");

                    return StatusCode(424, "Нет связи с постом");
                }

                result.result = response;

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Изменение тарифов по кодам моек")]
        [SwaggerResponse(200, Type = typeof(List<ChangeParameterWashResult>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("change/wash")]
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

                        //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                        HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/rate", JsonConvert.SerializeObject(change.rates));

                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
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
