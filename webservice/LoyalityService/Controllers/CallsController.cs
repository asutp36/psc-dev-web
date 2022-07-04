using LoyalityService.Models;
using LoyalityService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallsController : ControllerBase
    {
        private readonly ILogger<CallsController> _logger;
        private readonly WashDiscountService _washDiscountService;

        public CallsController(ILogger<CallsController> logger, WashDiscountService washDiscountService)
        {
            _logger = logger;
            _washDiscountService = washDiscountService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IncomeCallModel call)
        {
            if (call == null || long.TryParse(call.From, out long userPhone) || long.TryParse(call.To, out long devicePhone) || DateTime.Now - call.DTime > TimeSpan.FromDays(31))
            {
                return BadRequest();
            }

            // получить скидку пользователя и код девайса
            int discount = await _washDiscountService.GetDiscountAsync(userPhone);
            string deviceCode = await _washDiscountService.GetDeviceCodeAsync(devicePhone);

            return Ok();
        }
    }
}
