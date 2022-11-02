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
    [ApiController]
    public class TechDashboardController : ControllerBase
    {
        private readonly TechDashboardService _techDashboardService;

        public TechDashboardController(TechDashboardService techDashboardService)
        {
            _techDashboardService = techDashboardService;
        }

        [HttpPost("payoutcash/{payoutCash}")]
        public async Task GetTerminals(PayoutCashInsertionModel payoutCash)
        {

        }

        [Authorize]
        [HttpGet("wash")]
        public async Task<IActionResult> GetWashes()
        {
            var result = await _techDashboardService.GetWashesWithTerminalsActions(User.Claims.Where(o => o.Type == "GateWash").Select(o => o.Value));
            return Ok(result);
        }
    }
}
