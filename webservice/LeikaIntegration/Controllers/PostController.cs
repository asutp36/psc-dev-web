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

        [HttpGet("state/{post}/{clientID}")]
        public IActionResult State(string post, string clientID = "")
        {
            try 
            {
                //string postIp = GetPosIp(post);
                //if (postIp.Equals(""))
                //{
                //    _logger.LogError("не нашёлся пост " + post + Environment.NewLine);
                //    return NotFound();
                //}

                Supplies.HttpResponse response = Supplies.HttpSender.SendGet("http://" + "192.168.201.4:5000" + "/api/post/state?ClientId=" + clientID);
                switch (response.StatusCode)
                {
                    case (HttpStatusCode)0:
                        _logger.LogInformation("пост " + post + " недоступен" + Environment.NewLine);
                        return StatusCode(424);

                    case (HttpStatusCode)423:
                        return StatusCode(423);

                    case HttpStatusCode.OK:
                        return Ok();

                    default:
                        _logger.LogInformation("пост " + post + " недоступен (default)" + Environment.NewLine);
                        return StatusCode(424);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

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
                    if(model.amount <= 0 || model.amount > 500)
                    {
                        _logger.LogError("некорректный баланс: " + model.amount + Environment.NewLine);
                        return BadRequest("Incorrect amount");
                    }
                }

                //string postIp = GetPosIp(model.post);
                //if (postIp.Equals(""))
                //{
                //    _logger.LogError("не найден пост " + model.post + Environment.NewLine);
                //    return NotFound();
                //}

                if(model.clientID == null)
                {
                    model.clientID = "";
                }

                Supplies.HttpResponse response = Supplies.HttpSender.SendPost("http://" + "192.168.201.4:5000" + "/api/post/balance/increase/mob_app", JsonConvert.SerializeObject(new StartPostPostBindingModel { Amount = model.amount, ClientId = model.clientID }));
                switch (response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return Ok();

                    case (HttpStatusCode)0:
                        _logger.LogInformation("пост " + model.post + " недоступен" + Environment.NewLine);
                        return StatusCode(424);

                    case (HttpStatusCode)423:
                        _logger.LogError("пост " + model.post + " занят" + Environment.NewLine);
                        return StatusCode(423);
                }

                return Ok();
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