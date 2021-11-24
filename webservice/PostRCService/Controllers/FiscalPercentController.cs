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
    public class FiscalPercentController : ControllerBase
    {
        ILogger<FiscalPercentController> _logger;

        public FiscalPercentController(ILogger<FiscalPercentController> logger)
        {
            _logger = logger;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущий процент фискализации на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(WashParameter<FiscalPercentModel>))]
        [SwaggerResponse(404, Description = "Не найдена мойка")]
        [SwaggerResponse(424, Description = "Нет связи с мойкой")]
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
                WashParameter<FiscalPercentModel> result = new WashParameter<FiscalPercentModel>();
                result.washCode = washCode;
                result.posts = new List<PostParameter<FiscalPercentModel>>();

                List<string> postCodes = SqlHelper.GetPostCodes(washCode);
                bool returnError = true;

                foreach (string p in postCodes)
                {
                    PostParameter<FiscalPercentModel> postRates = new PostParameter<FiscalPercentModel>();
                    postRates.postCode = p;
                    postRates.value = new FiscalPercentModel();

                    string ip = SqlHelper.GetPostIp(p);
                    if (ip == null)
                    {
                        _logger.LogError($"Не найден ip поста {p}");
                        continue;
                    }

                    //HttpResponse response = HttpSender.SendGet("http://" + ip + "/api/post/rate/get");
                    HttpResponse response = HttpSender.SendGet("http://192.168.201.5:5000/api/post/get/fiscalpercent");

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        var emptyPost = new PostParameter<FiscalPercentModel> { postCode = p };
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

                    postRates.value = JsonConvert.DeserializeObject<FiscalPercentModel>(response.ResultMessage);
                    result.posts.Add(postRates);
                    returnError = false;
                }

                if (returnError)
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
        [SwaggerOperation(Summary = "Изменене процента фискалзации на посту по коду")]
        [SwaggerResponse(200, Type = typeof(SetParameterPostResult))]
        [SwaggerResponse(404, Description = "Не найден пост")]
        [SwaggerResponse(424, Description = "Нет связи с постом")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("set/post")]
        public IActionResult SetByPost(PostParameter<FiscalPercentModel> param)
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

                //HttpResponse response = HttpSender.SendPost($"http://{ip}/api/post/rate", JsonConvert.SerializeObject(change.rates));
                HttpResponse response = HttpSender.SendPatch($"http://192.168.201.5:5000/api/post/set/fiscalpercent", JsonConvert.SerializeObject(param.value));

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
        [SwaggerOperation(Summary = "Изменение процента фискализации на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(SetParameterWashResult))]
        [SwaggerResponse(424, Description = "Нет связи с мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("set/wash")]
        public IActionResult SetByWash(SetParametersWash<FiscalPercentModel> parameter)
        {
            try
            {
                if (!SqlHelper.IsWashExists(parameter.washCode))
                {
                    _logger.LogError($"Не найдена мойка {parameter.washCode}");
                    return NotFound();
                }

                SetParameterWashResult result = new SetParameterWashResult
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
                    HttpResponse response = HttpSender.SendPatch($"http://192.168.201.5:5000/api/post/set/fiscalpercent", JsonConvert.SerializeObject(parameter.value));

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
