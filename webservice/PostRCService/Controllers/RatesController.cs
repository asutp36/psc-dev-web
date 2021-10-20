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
                WashParameter<RatesModel> result = new WashParameter<RatesModel>();
                result.washCode = washCode;
                result.posts = new List<PostParameter<RatesModel>>();

                List<string> postCodes = SqlHelper.GetPostCodes(washCode);
                bool returnError = true;

                foreach(string p in postCodes)
                {
                    PostParameter<RatesModel> postRates = new PostParameter<RatesModel>();
                    postRates.postCode = p;
                    postRates.value = new RatesModel();
                    postRates.value.rates = new List<FunctionRate>();

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
                        var emptyPost = new PostParameter<RatesModel> { postCode = p };
                        if (response.StatusCode == 0)
                        {
                            _logger.LogInformation($"Нет соединения с постом {p}");
                            result.posts.Add(emptyPost);
                            continue;
                        }

                        _logger.LogError($"Ответ поста {p}: {JsonConvert.SerializeObject(response)}");
                        result.posts.Add(emptyPost);
                        continue;
                    }

                    postRates.value.rates = JsonConvert.DeserializeObject<List<FunctionRate>>(response.ResultMessage);
                    result.posts.Add(postRates);
                    returnError = false;
                }

                if(returnError)
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
        [SwaggerOperation(Summary = "Изменене тарифов на посту по коду")]
        [SwaggerResponse(200, Type = typeof(SetParameterPostResult))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("set/post")]
        public IActionResult SetByPost(PostParameter<RatesModel> parameter)
        {
            try
            {
                if (!SqlHelper.IsPostExists(parameter.postCode))
                {
                    _logger.LogError($"Не найден пост {parameter.postCode}" + Environment.NewLine);
                    return NotFound();
                }

                SetParameterPostResult result = new SetParameterPostResult();
                result.post = parameter.postCode;

                string ip = SqlHelper.GetPostIp(parameter.postCode);
                if (ip == null)
                {
                    _logger.LogError($"Не найден ip поста {parameter.postCode}");
                    return NotFound();
                }

                //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/rate", JsonConvert.SerializeObject(parameter.value.rates));

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    if (response.StatusCode == 0)
                        _logger.LogInformation($"Нет соединения с постом {parameter.postCode}");
                    else
                        _logger.LogError($"Ответ поста {parameter.postCode}: {JsonConvert.SerializeObject(response)}");

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
        [SwaggerResponse(200, Type = typeof(List<SetParameterWashResult>))]
        [SwaggerResponse(424, Description = "Нет связи ни с одной мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя оибка сервера")]
        #endregion
        [HttpPost("set/wash")]
        public IActionResult SetByWash(SetParametersWash<RatesModel> parameter)
        {
            try
            {
                if (!SqlHelper.IsWashExists(parameter.washCode))
                {
                    _logger.LogError($"Не найдена мойка {parameter.washCode}");
                    return NotFound();
                }

                SetParameterWashResult washResult = new SetParameterWashResult
                {
                    wash = parameter.washCode,
                    posts = new List<SetParameterPostResult>()
                };

                List<string> posts = SqlHelper.GetPostCodes(parameter.washCode);
                foreach (string post in posts)
                {
                    string ip = SqlHelper.GetPostIp(post);
                    if (ip == null)
                    {
                        _logger.LogError($"Не найден ip поста {post}");
                        continue;
                    }

                    //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                    HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/rate", JsonConvert.SerializeObject(parameter.value.rates));

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

                return Ok(washResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }
    }
}
