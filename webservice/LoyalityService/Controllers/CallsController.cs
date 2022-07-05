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
        private readonly WashDiscountService _washDiscountService;

        public CallsController(WashDiscountService washDiscountService)
        {
            _washDiscountService = washDiscountService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IncomeCallModel call)
        {
            if (call == null || long.TryParse(call.From, out long userPhone) || long.TryParse(call.To, out long devicePhone) || DateTime.Now - call.When > TimeSpan.FromDays(31))
            {
                return BadRequest();
            }

            // запускает пост, если удачно - записывает мойку
            _washDiscountService.StartPostAsync(call);

            return Ok();
        }
    }
}
