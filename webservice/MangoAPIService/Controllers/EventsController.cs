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
using Microsoft.Extensions.Caching.Memory;

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
        private IMemoryCache _cache;
        private readonly ILoyalityServiceCaller _loyalityService;

        public EventsController(ILogger<EventsController> logger, IConfiguration config, IMangoAPICaller mangoApiCallerService, IMemoryCache memoryCache, ILoyalityServiceCaller loyalityService)
        {
            _logger = logger;
            _apiSalt = config.GetValue<string>("ApiSalt", null);
            _apiKey = config.GetValue<string>("ApiKey", null);
            _mangoApiCallerService = mangoApiCallerService;
            _cache = memoryCache;
            _loyalityService = loyalityService;
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

                int callIdentifier = (incomingCallInfo.from.number + incomingCallInfo.to.number).GetHashCode();

                // если нет в кэше этого вызова, то считаем его новым
                if (!_cache.TryGetValue(callIdentifier, out CallCacheModel call))
                {
                    // создаётся вызов, добавляется в кэш 
                    call = new CallCacheModel
                    {
                        From = incomingCallInfo.from.number,
                        To = incomingCallInfo.to.number,
                        When = DateTime.Now
                    };

                    // время жизни вызова в кэше - 5 секунд (с момента, когда последний раз обращались к этому вызову)
                    _cache.Set(callIdentifier, call, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(5)));

                    // вызов манго API чтобы завершить звонок
                    _mangoApiCallerService.CallHangupAsync(incomingCallInfo.call_id);

                    // отправляем на сервис лояльности сообщение о новом вызове
                    _loyalityService.HandleNewCallAsync(call);
                }

                return Accepted();
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
            _logger.LogInformation("тест логгов");
            var a = HashAlgorithm.Create("SHA256");
            string sign = Hasher.GetHash(a, _apiKey + "{\"command_id\":\"24.06.2022 16:49:36\",\"call_id\":\"MToxMDE4MDk0NzoxMDE6NDI4NzA1Njk1OjE=\"}" + _apiSalt);
            return Ok(new string[] { sign, "03bcba3efe9f66a45752e337cb8164dfdf8a0fbf59f4611e36c60a04020759a1" });
        }
    }
}
