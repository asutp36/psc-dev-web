﻿using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpResponse = Backend.Controllers.Supplies.HttpResponse;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcquiringController : ControllerBase
    {
        ILogger<AcquiringController> _logger;
        IConfiguration _config;

        public AcquiringController(ILogger<AcquiringController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<WashDiscountViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                UserInfo uInfo = new UserInfo(User.Claims.ToList());

                List<WashViewModel> washes = uInfo.GetWashes();
                List<WashAcquiringViewModel> result = new List<WashAcquiringViewModel>();
                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/postdiscount/get", JsonConvert.SerializeObject(w.code));

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"По мойке {w.code} не удалось получить текущие скидки. postrc response: " + response.ResultMessage);
                        //return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                    }

                    string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                    var posts = JsonConvert.DeserializeObject<List<PostAcquiringViewModel>>(str);

                    result.Add(new WashAcquiringViewModel
                    {
                        Wash = w.code,
                        Posts = posts
                    });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойке по коду")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдена мойка")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("wash/{wash}")]
        public IActionResult GetByWash(string wash)
        {
            try
            {
                if (!SqlHelper.IsWashExists(wash))
                {
                    _logger.LogError($"Не найдена мойка {wash}" + Environment.NewLine);
                    return NotFound(new Error("Не найдена мойка", "badvalue"));
                }

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/postdiscount/get", JsonConvert.SerializeObject(wash));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                }
                string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                var result = JsonConvert.DeserializeObject<List<PostAcquiringViewModel>>(str);

                return Ok(new WashAcquiringViewModel
                {
                    Wash = wash,
                    Posts = result
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие настройки эквайринга на мойках по коду региона")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найдены мойки по коду региона")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("region/{region}")]
        public IActionResult GetByRegion(int region)
        {
            try
            {
                List<WashViewModel> washes = SqlHelper.GetWashesByRegion(region);
                if (washes.Count <= 0)
                {
                    _logger.LogError($"Не найдены коды моек в регионе {region}" + Environment.NewLine);
                    return NotFound(new Error("Не найдены мойки", "badvalue"));
                }

                List<WashAcquiringViewModel> result = new List<WashAcquiringViewModel>();
                foreach (WashViewModel w in washes)
                {
                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/postdiscount/get", JsonConvert.SerializeObject(w.code));

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        _logger.LogError($"По мойке {w.code} не удалось получить текущие скидки. postrc response: " + response.ResultMessage);
                        //return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                    }

                    string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                    var posts = JsonConvert.DeserializeObject<List<PostAcquiringViewModel>>(str);

                    result.Add(new WashAcquiringViewModel
                    {
                        Wash = w.code,
                        Posts = posts
                    });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Установить настройки эквайринга на мойках по постам")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpPost("post")]
        public IActionResult Set(List<PostAcquiringViewModel> model)
        {
            try
            {
                List<SetRateResultPost> result = new List<SetRateResultPost>();
                foreach (PostAcquiringViewModel discount in model)
                {
                    HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/postdiscount/set", JsonConvert.SerializeObject(discount));
                    result.Add(new SetRateResultPost
                    {
                        postCode = discount.Post,
                        result = response
                    });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}