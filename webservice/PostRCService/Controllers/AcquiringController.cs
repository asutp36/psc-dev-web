﻿using Microsoft.AspNetCore.Http;
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

                WashParameter<AcquiringModel> result = new WashParameter<AcquiringModel>();
                result.washCode = washCode;
                result.posts = new List<PostParameter<AcquiringModel>>();

                List<string> postCodes = SqlHelper.GetPostCodes(washCode);
                foreach(string p in postCodes)
                {
                    PostParameter<AcquiringModel> postAcquiring = new PostParameter<AcquiringModel>();
                    postAcquiring.postCode = p;

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

                    postAcquiring.value = JsonConvert.DeserializeObject<AcquiringModel>(response.ResultMessage);
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
        [SwaggerOperation(Summary = "Изменене настоек эквайринга на посту по коду")]
        [SwaggerResponse(200, Type = typeof(SetParameterPostResult))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("set/post")]
        public IActionResult SetByPost(PostAcquiring change)
        {
            try
            {
                if (!SqlHelper.IsPostExists(change.post))
                {
                    _logger.LogError($"Не найден пост {change.post}" + Environment.NewLine);
                    return NotFound();
                }

                SetParameterPostResult result = new SetParameterPostResult();
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
        [SwaggerResponse(200, Type = typeof(List<SetParameterWashResult>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("set/wash")]
        public IActionResult SetByWash(ChangeAcquiringWash change)
        {
            try
            {
                List<SetParameterWashResult> result = new List<SetParameterWashResult>();
                foreach (string wash in change.washes)
                {
                    if (!SqlHelper.IsWashExists(wash))
                    {
                        _logger.LogError($"Не найдена мойка {wash}");
                        continue;
                    }

                    SetParameterWashResult washResult = new SetParameterWashResult
                    {
                        wash = wash,
                        posts = new List<SetParameterPostResult>()
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

                        washResult.posts.Add(new SetParameterPostResult
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
