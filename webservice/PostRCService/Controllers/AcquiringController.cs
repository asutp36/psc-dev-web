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
        [SwaggerResponse(200, Type = typeof(WashParameter<AcquiringModel>))]
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
        public IActionResult SetByPost(PostParameter<AcquiringModel> param)
        {
            try
            {
                if (!SqlHelper.IsPostExists(param.postCode))
                {
                    _logger.LogError($"Не найден пост {param.postCode}" + Environment.NewLine);
                    return NotFound();
                }

                SetParameterPostResult result = new SetParameterPostResult();
                result.post = param.postCode;

                string ip = SqlHelper.GetPostIp(param.postCode);
                if (ip == null)
                {
                    _logger.LogError($"Не найден ip поста {param.postCode}");
                    return NotFound();
                }

                //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/set/acquiring", JsonConvert.SerializeObject(change.rates));
                HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/set/acquiring", JsonConvert.SerializeObject(param.value));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0)
                        _logger.LogInformation($"Нет соединения с постом {param.postCode}");
                    else
                        _logger.LogError($"Ответ поста {param.postCode}: {JsonConvert.SerializeObject(response)}");

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
        [SwaggerOperation(Summary = "Изменить настройки эквайринга на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(SetParameterWashResult))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("set/wash")]
        public IActionResult SetByWash(SetParametersWash<AcquiringModel> param)
        {
            try
            {
                if (!SqlHelper.IsWashExists(param.washCode))
                {
                    _logger.LogError($"Не найдена мойка {param.washCode}");
                    return NotFound();
                }

                SetParameterWashResult result = new SetParameterWashResult
                {
                    wash = param.washCode,
                    posts = new List<SetParameterPostResult>()
                };

                List<string> posts = SqlHelper.GetPostCodes(param.washCode);
                foreach (string post in posts)
                {
                    string ip = SqlHelper.GetPostIp(post);
                    if (ip == null)
                    {
                        _logger.LogError($"Не найден ip поста {post}");
                        continue;
                    }

                    //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/set/acquiring", JsonConvert.SerializeObject(change.acquiring));
                    HttpResponse response = HttpSender.SendPost("http://192.168.201.5:5000/api/post/set/acquiring", JsonConvert.SerializeObject(param.value));
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        if (response.StatusCode == 0)
                            _logger.LogInformation($"Нет соединения с постом {post}");
                        else
                            _logger.LogError($"Ответ поста {post}: {JsonConvert.SerializeObject(response)}");

                    result.posts.Add(new SetParameterPostResult
                    {
                        post = post,
                        result = response
                    });
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
