using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LeikaIntegration.Controllers.Supplies;

using LeikaIntegration.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;


namespace LeikaIntegration.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ModelDbContext _model = new ModelDbContext();
        private readonly ILogger<PostController> _logger;

        public PostController(ILogger<PostController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Узнать состояние поста
        /// </summary>
        /// <returns></returns>
        /// <response code="200">Свободен</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="404">Не найден пост</response>
        /// <response code="424">Нет связи с постом</response>
        /// <response code="423">Пост занят</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        //[SwaggerRequestExample(typeof(PostStateRequestBindingModel), typeof(PostStateRequestExample))]
        [HttpPost("state")]
        public IActionResult State(PostStateRequestBindingModel model)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

                if (!CryptHash.CheckHashCode(model.hash, model.dtime))
                {
                    _logger.LogError("хэш не прошёл проверку" + Environment.NewLine);
                    return Unauthorized();
                }

                string postIp = GetPosIp(model.post);
                if (postIp.Equals(""))
                {
                    _logger.LogError("не нашёлся пост " + model.post + Environment.NewLine);
                    return NotFound();
                }

                Supplies.HttpResponse response = Supplies.HttpSender.SendGet("http://" + postIp + "/api/post/state?ClientId=" + model.clientID);
                switch (response.StatusCode)
                {
                    case (HttpStatusCode)0:
                        _logger.LogInformation("пост " + model.post + " недоступен" + Environment.NewLine);
                        return StatusCode(424);

                    case (HttpStatusCode)423:
                        return StatusCode(423);

                    case HttpStatusCode.OK:
                        return Ok();

                    default:
                        _logger.LogInformation("пост " + model.post + " недоступен (default)" + Environment.NewLine);
                        return StatusCode(424);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Запустить пост
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="200">Удачно</response>
        /// <response code="400">Некорректные входные параметры</response>
        /// <response code="401">Хэш не прошёл проверку</response>
        /// <response code="404">Пост не найден</response>
        /// <response code="424">Нет связи с постом</response>
        /// <response code="423">Пост занят</response>
        /// <response code="500">Внутренняя ошибка сервера</response>
        [HttpPost("start")]
        public IActionResult Start(StartPostRequestBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("модель не прошла валидацию: " + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return BadRequest();
                }
                else
                {
                    if(model.amount <= 200 || model.amount >= 1000)
                    {
                        _logger.LogError("некорректный баланс: " + model.amount + Environment.NewLine);
                        return BadRequest("Incorrect amount");
                    }
                }

                if (!CryptHash.CheckHashCode(model.hash, model.dtime))
                {
                    _logger.LogError("хэш не прошёл проверку" + Environment.NewLine);
                    return Unauthorized();
                }

                string postIp = GetPosIp(model.post);
                if (postIp.Equals(""))
                {
                    _logger.LogError("не найден пост " + model.post + Environment.NewLine);
                    return NotFound();
                }

                if (model.clientID == null)
                {
                    model.clientID = "";
                }

                Supplies.HttpResponse response = Supplies.HttpSender.SendPost("http://" + postIp + "/api/post/balance/increase/mob_app", JsonConvert.SerializeObject(new StartPostPostBindingModel { Amount = model.amount, ClientId = model.clientID }));
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        _logger.LogInformation($"Запущен пост {model.post}. Клиент {model.clientID}, сумма {model.amount}");
                        return Ok();

                    case (HttpStatusCode)0:
                        _logger.LogInformation("пост " + model.post + " недоступен" + Environment.NewLine);
                        return StatusCode(424);

                    case (HttpStatusCode)423:
                        _logger.LogError("пост " + model.post + " занят" + Environment.NewLine);
                        return StatusCode(423);

                    default:
                        _logger.LogInformation("пост " + model.post + " недоступен (default)" + Environment.NewLine);
                        return StatusCode(424);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine);
                return StatusCode(500);
            }
        }

        private string GetPosIp(string code)
        {
            try
            {
                return _model.Device.Find(_model.Posts.Where(p => p.Qrcode.Equals(code)).FirstOrDefault().Iddevice).IpAddress;
            }
            catch (NullReferenceException)
            {
                return "";
            }
        }
    }
}