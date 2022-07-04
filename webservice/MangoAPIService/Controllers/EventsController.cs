using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangoAPIService.Models;
using MangoAPIService.Helpers;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using MangoAPIService.Services;

namespace MangoAPIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ILogger<EventsController> _logger;
        private readonly string _apiSalt;
        private readonly string _apiKey;
        private readonly IMangoAPICaller _mangoApiCallerService;

        public EventsController(ILogger<EventsController> logger, IConfiguration config, IMangoAPICaller mangoApiCallerService)
        {
            _logger = logger;
            _apiSalt = config.GetValue<string>("ApiSalt", null);
            _apiKey = config.GetValue<string>("ApiKey", null);
            _mangoApiCallerService = mangoApiCallerService;
        }

        [HttpPost("call")]
        public async Task<IActionResult> Call([FromForm] MangoAPIRequestParameters parameters)
        {
            try
            {
                _logger.LogInformation("Параметры запуска: " + JsonConvert.SerializeObject(parameters));
                //проверка хэша
                if (!Hasher.VerifyHash(HashAlgorithm.Create("SHA256"), parameters.sign, parameters.vpbx_api_key + parameters.json + _apiSalt))
                {
                    _logger.LogError("Подпись не прошла проверку" + Environment.NewLine);
                    return Unauthorized();
                }

                // парсинг данных запроса
                MangoAPIIncomingCall incomingCallInfo = JsonConvert.DeserializeObject<MangoAPIIncomingCall>(parameters.json);
                _logger.LogInformation($"От кого: {incomingCallInfo.from.number}, куда: {incomingCallInfo.to.number}");

                // вызов манго API чтобы завершить звонок
                _mangoApiCallerService.CallHangupAsync(incomingCallInfo.call_id);

                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestSignature()
        {
            var a = HashAlgorithm.Create("SHA256");
            string sign = Hasher.GetHash(a, _apiKey + "{\"command_id\":\"24.06.2022 16:49:36\",\"call_id\":\"MToxMDE4MDk0NzoxMDE6NDI4NzA1Njk1OjE=\"}" + _apiSalt);
            return Ok(new string[] { sign, "03bcba3efe9f66a45752e337cb8164dfdf8a0fbf59f4611e36c60a04020759a1" });
        }
    }
}
