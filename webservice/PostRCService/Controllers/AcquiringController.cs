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
    public class AcquiringController : ControllerBase
    {
        ILogger<AcquiringController> _logger;

        public AcquiringController(ILogger<AcquiringController> logger)
        {
            _logger = logger;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на посту по коду")]
        [SwaggerResponse(200, Type = typeof(PostAcquiring))]
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

                PostAcquiring result = new PostAcquiring();
                result.post = post;

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

                result.acquiring = JsonConvert.DeserializeObject<AcquiringModel>(response.ResultMessage);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(WashAcquiring))]
        [SwaggerResponse(404, Description = "Не найдена мойка")]
        [SwaggerResponse(424, Description = "Нет связи ни с одним постом на мойке")]
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

                WashAcquiring result = new WashAcquiring();
                result.wash = washCode;
                result.posts = new List<PostAcquiring>();

                List<string> postCodes = SqlHelper.GetPostCodes(washCode);
                foreach(string p in postCodes)
                {
                    PostAcquiring postAcquiring = new PostAcquiring();
                    postAcquiring.post = p;

                    string ip = SqlHelper.GetPostIp(p);
                    if (ip == null)
                    {
                        _logger.LogError($"Не найден ip поста {p}");
                        continue;
                    }

                    //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/get/acquiring");
                    HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/get/acquiring");

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

                    postAcquiring.acquiring = JsonConvert.DeserializeObject<AcquiringModel>(response.ResultMessage);
                    result.posts.Add(postAcquiring);
                }
                if(result.posts.Count < 1)
                {
                    _logger.LogError($"Нет связи с мойкой {washCode}" + Environment.NewLine);
                    return StatusCode(424);
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на нескольких мойках по кодам")]
        [SwaggerResponse(200, Type = typeof(List<WashAcquiring>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("manywash")]
        public IActionResult GetByManyWash([FromBody]string[] washes)
        {
            try
            {
                List<WashAcquiring> result = new List<WashAcquiring>();

                foreach (string wash in washes)
                {
                    if (!SqlHelper.IsWashExists(wash))
                    {
                        _logger.LogError($"Не найдена мойка {wash}" + Environment.NewLine);
                        continue;
                    }

                    WashAcquiring washAcquiring = new WashAcquiring();
                    washAcquiring.wash = wash;
                    washAcquiring.posts = new List<PostAcquiring>();

                    List<string> postCodes = SqlHelper.GetPostCodes(wash);
                    foreach (string p in postCodes)
                    {
                        PostAcquiring postAcquiring = new PostAcquiring();
                        postAcquiring.post = p;
                        postAcquiring.acquiring = new AcquiringModel();

                        string ip = SqlHelper.GetPostIp(p);
                        if (ip == null)
                        {
                            _logger.LogError($"Не найден ip поста {p}");
                            continue;
                        }

                        //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/get/acquiring");
                        HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/get/acquiring");

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

                        postAcquiring.acquiring = JsonConvert.DeserializeObject<AcquiringModel>(response.ResultMessage);
                        washAcquiring.posts.Add(postAcquiring);
                    }

                    if (washAcquiring.posts.Count < 1)
                    {
                        _logger.LogInformation($"Нет связи с мойкой {wash}" + Environment.NewLine);
                    }

                    result.Add(washAcquiring);
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
        [SwaggerOperation(Summary = "Изменене настоек эквайринга на посту по коду")]
        [SwaggerResponse(200, Type = typeof(ChangeParameterResult))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("change/post")]
        public IActionResult ChangeByPost(PostAcquiring change)
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

                //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/set/acquiring", JsonConvert.SerializeObject(change.rates));
                HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/set/acquiring", JsonConvert.SerializeObject(change.acquiring));

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
        [SwaggerOperation(Summary = "Изменить настройки эквайринга на мойках по коду")]
        [SwaggerResponse(200, Type = typeof(List<ChangeParameterWashResult>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("change/wash")]
        public IActionResult ChangeByWash(ChangeAcquiringWash change)
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

                        //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/set/acquiring", JsonConvert.SerializeObject(change.acquiring));
                        HttpResponse response = HttpSender.SendPost("http://192.168.201.5:5000/api/post/set/acquiring", JsonConvert.SerializeObject(change.acquiring));
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
