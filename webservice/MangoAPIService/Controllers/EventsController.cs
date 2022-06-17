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

namespace MangoAPIService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        public readonly ILogger<EventsController> _logger;
        public readonly string _apiSalt;

        public EventsController(ILogger<EventsController> logger, IConfiguration config)
        {
            _logger = logger;
            _apiSalt = config.GetValue<string>("ApiSalt", null);
        }

        [HttpPost("call")]
        public async Task<IActionResult> Call([FromBody] MangoAPIRequestParameters parameters)
        {
            if(!Hasher.VerifyHash(HashAlgorithm.Create("SHA256"), parameters.sign, parameters.vpbx_api_key + parameters.json + _apiSalt))
            {
                _logger.LogError("Подпись не прошла проверку" + Environment.NewLine);
                return Unauthorized();
            }

            return Ok();
        }
    }
}
