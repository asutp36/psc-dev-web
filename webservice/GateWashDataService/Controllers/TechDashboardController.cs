using GateWashDataService.Models;
using GateWashDataService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = "CanRefillGateWash")]
    [ApiController]
    public class TechDashboardController : ControllerBase
    {
        private readonly TechDashboardService _techDashboardService;

        public TechDashboardController(TechDashboardService techDashboardService)
        {
            _techDashboardService = techDashboardService;
        }

        [HttpGet("wash")]
        public async Task<IActionResult> GetWashes()
        {
            var result = await _techDashboardService.GetWashesWithTerminalsActions(User.Claims.Where(o => o.Type == "GateWash").Select(o => o.Value));
            return Ok(result);
        }

        [HttpGet("wash/{code}")]
        public async Task<IActionResult> GetWash(string code)
        {
            var result = await _techDashboardService.GetWashWithTerminalsActionsByCode(code);
            return Ok(result);
        }

        [HttpGet("region")]
        public async Task<IActionResult> GetRegions()
        {
            var result = await _techDashboardService.GetRegionsWithWashesTerminalAction(User.Claims.Where(o => o.Type == "GateWash").Select(o => o.Value));
            return Ok(result);
        }

        [HttpGet("counters/{terminalCode}")]
        public async Task<IActionResult> GetCurrentCounters(string terminalCode)
        {
            var counters = await _techDashboardService.GetCurrentCountersAsync(terminalCode);
            return Ok(counters);
        }

        [HttpPost("cash")]
        public async Task<IActionResult> SendPayoutCashInsertion(PayoutCashInsertionModel insertion)
        { 
            insertion.UserID = User.Claims.Where(c => c.Type == "UserID").Select(c => int.Parse(c.Value)).FirstOrDefault();

            await _techDashboardService.SendNotificationPayoutInsertion(insertion);

            await _techDashboardService.SendPayoutInsertionToTerminal(insertion);

            return Ok();
        }

        [HttpPost("cards")]
        public async Task<IActionResult> SendCardsInsertion(TerminalCardsInsertionModel insertion)
        {
            insertion.UserID = User.Claims.Where(c => c.Type == "UserID").Select(c => int.Parse(c.Value)).FirstOrDefault();

            await _techDashboardService.SendNotificationCardsInsertion(insertion);
            await _techDashboardService.SendCardInsertionToTerminal(insertion);
            return Ok();
        }
    }
}
