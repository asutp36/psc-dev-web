using Backend.Controllers.Supplies;
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
    public class RatesController : ControllerBase
    {
        ILogger<RatesController> _logger;
        IConfiguration _config;

        public RatesController(ILogger<RatesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [SwaggerResponse(200)]
        [HttpPost]
        public IActionResult SetRates(ChangeRateViewModel model)
        {
            return Ok();
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках пользователя")]
        [SwaggerResponse(200, Type = typeof(List<WashRatesViewModel>))]
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
                List<string> washCodes = new List<string>();
                foreach (WashViewModel w in washes)
                    washCodes.Add(w.code);

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/post/getrate", JsonConvert.SerializeObject(washCodes));
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                }
                string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                var result = JsonConvert.DeserializeObject<List<WashRatesViewModel>>(str);

                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойке по коду")]
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
                List<string> washCodes = new List<string>();
                washCodes.Add(wash);

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/post/getrate", JsonConvert.SerializeObject(washCodes));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                }
                string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                var result = JsonConvert.DeserializeObject<List<WashRatesViewModel>>(str);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить текущие тарифы на мойках по коду региона")]
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
                if(washes.Count <= 0)
                {
                    _logger.LogError($"Не найдены коды моек в регионе {region}" + Environment.NewLine);
                    return NotFound(new Error("Не найдены мойки", "badvalue"));
                }

                List<string> washCodes = new List<string>();
                foreach(WashViewModel w in washes)
                    washCodes.Add(w.code);

                HttpResponse response = HttpSender.SendPost(_config["Services:postrc"] + "api/post/getrate", JsonConvert.SerializeObject(washCodes));
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogError("postrc response: " + response.ResultMessage);
                    return StatusCode(424, new Error("Не удалось получить текущие тарифы", "service"));
                }
                string str = response.ResultMessage.Substring(1, response.ResultMessage.Length - 2).Replace(@"\", "");
                var result = JsonConvert.DeserializeObject<List<WashRatesViewModel>>(str);

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
