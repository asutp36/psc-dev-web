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
    public class HappyHourController : ControllerBase
    {
        ILogger<HappyHourController> _logger;

        public HappyHourController(ILogger<HappyHourController> logger)
        {
            _logger = logger;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие скидки на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(WashHappyHour))]
        [SwaggerResponse(404, Description = "Не найдена мойка")]
        [SwaggerResponse(424, Description = "Нет связи ни с одним постом на мойке")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
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

                WashHappyHour result = new WashHappyHour();
                result.wash = washCode;
                result.posts = new List<PostHappyHour>();

                List<string> postCodes = SqlHelper.GetPostCodes(washCode);
                foreach (string p in postCodes)
                {
                    PostHappyHour postHappyHour = new PostHappyHour();
                    postHappyHour.post = p;

                    string ip = SqlHelper.GetPostIp(p);
                    if (ip == null)
                    {
                        _logger.LogError($"Не найден ip поста {p}");
                        continue;
                    }

                    //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/get/happyhours");
                    HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/get/happyhours");

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

                    postHappyHour.happyHour = JsonConvert.DeserializeObject<HappyHourModel>(response.ResultMessage);
                    result.posts.Add(postHappyHour);
                }
                if (result.posts.Count < 1)
                {
                    _logger.LogError($"Нет связи с мойкой {washCode}" + Environment.NewLine);
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
        [SwaggerOperation(Summary = "Получить текущие скидки на нескольких мойках по кодам")]
        [SwaggerResponse(200, Type = typeof(List<WashRates>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("manywash")]
        public IActionResult GetByManyWash([FromBody] string[] washes)
        {
            try
            {
                List<WashHappyHour> result = new List<WashHappyHour>();

                foreach (string wash in washes)
                {
                    if (!SqlHelper.IsWashExists(wash))
                    {
                        _logger.LogError($"Не найдена мойка {wash}" + Environment.NewLine);
                        continue;
                    }

                    WashHappyHour washHH = new WashHappyHour();
                    washHH.wash = wash;
                    washHH.posts= new List<PostHappyHour>();

                    List<string> postCodes = SqlHelper.GetPostCodes(wash);
                    foreach (string p in postCodes)
                    {
                        PostHappyHour postHH = new PostHappyHour();
                        postHH.post = p;
                        postHH.happyHour = new HappyHourModel();

                        string ip = SqlHelper.GetPostIp(p);
                        if (ip == null)
                        {
                            _logger.LogError($"Не найден ip поста {p}");
                            continue;
                        }

                        //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");
                        HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/get/happyhours");

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

                        postHH.happyHour = JsonConvert.DeserializeObject<HappyHourModel>(response.ResultMessage);
                        washHH.posts.Add(postHH);
                    }

                    if (washHH.posts.Count < 1)
                    {
                        _logger.LogInformation($"Нет связи с мойкой {wash}" + Environment.NewLine);
                    }

                    result.Add(washHH);
                }

                if (result.Count < 1)
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
        [SwaggerOperation(Summary = "Изменене настройки скидок на посту по коду")]
        [SwaggerResponse(200, Type = typeof(ChangeParameterResult))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("change/post")]
        public IActionResult ChangeByPost(PostHappyHour change)
        {
            try
            {
                if (!SqlHelper.IsPostExists(change.post))
                {
                    _logger.LogError($"Не найден пост {change.post}" + Environment.NewLine);
                    return NotFound();
                }

                ChangeParameterResult result = new ChangeParameterResult();
                result.post = change.post;

                string ip = SqlHelper.GetPostIp(change.post);
                if (ip == null)
                {
                    _logger.LogError($"Не найден ip поста {change.post}");
                    return NotFound();
                }

                //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/set/happyhours", JsonConvert.SerializeObject(change.happyHour));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0)
                        _logger.LogInformation($"Нет соединения с постом {change.post}");
                    else
                        _logger.LogError($"Ответ поста {change.post}: {JsonConvert.SerializeObject(response)}");

                    return StatusCode(424, "Нет связи с постом");
                }

                result.result = response;

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Измненить настройки скидок на нескольких мойках по коду")]
        [SwaggerResponse(200, Type = typeof(List<ChangeParameterWashResult>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("change/wash")]
        public IActionResult ChangeByWash(ChangeHappyHourWash change)
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
                    foreach (string post in posts)
                    {
                        string ip = SqlHelper.GetPostIp(post);
                        if (ip == null)
                        {
                            _logger.LogError($"Не найден ip поста {post}");
                            continue;
                        }

                        //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/set/happyhours", JsonConvert.SerializeObject(change.happyHour));
                        HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/set/happyhours", JsonConvert.SerializeObject(change.happyHour));

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
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }
    }
}
