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
        private readonly IAsyncDiscountManager _washDiscountService;
        private readonly IPostRCCaller _postRCService;

        public CallsController(IAsyncDiscountManager washDiscountService, IPostRCCaller postRCService)
        {
            _washDiscountService = washDiscountService;
            _postRCService = postRCService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] IncomeCallModel call)
        {
            if (call == null)
            {
                return BadRequest("call пустой");
            }

            if (!long.TryParse(call.From, out long clientPhone) || !long.TryParse(call.To, out long terminalPhone) || DateTime.Now - call.When > TimeSpan.FromDays(31))
            {
                return BadRequest("Не получилось распарсить входные данные");
            }

            string terminalCode = string.Empty;
            try
            {
                terminalCode = await _washDiscountService.GetTerminalCodeByPhoneAsync(terminalPhone);
            }
            catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            

            int discount = await _washDiscountService.CalculateDiscountAsync(terminalCode, clientPhone);

            _postRCService.StartPostAsync(new StartPostParameters { DeviceCode = terminalCode, Discount = discount });

            return Accepted();
        }
    }
}
