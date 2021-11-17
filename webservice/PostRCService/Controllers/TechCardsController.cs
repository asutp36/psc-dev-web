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
    public class TechCardsController : ControllerBase
    {
        ILogger<TechCardsController> _logger;

        public TechCardsController(ILogger<TechCardsController> logger)
        {
            _logger = logger;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Отправить карту на мойку по коду")]
        [SwaggerResponse(200, Type = typeof(SetParameterWashResult))]
        [SwaggerResponse(424, Description = "Нет связи с мойкой")]
        [SwaggerResponse(500, Description = "Внутренняя ошибка сервера")]
        #endregion
        [HttpPost("post/wash")]
        public IActionResult SetByWash(SetParametersWash<TechCardModel> parameter)
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
                    HttpResponse response = HttpSender.SendPost($"http://192.168.201.5:5000/api/post/create/washcard", JsonConvert.SerializeObject(parameter.value));

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
