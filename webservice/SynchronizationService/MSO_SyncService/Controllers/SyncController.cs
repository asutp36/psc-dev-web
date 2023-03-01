using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSO.SyncService.Models;
using MSO.SyncService.Services;

namespace MSO.SyncService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ILogger<SyncController> _logger;
        private readonly InsertService _insertService;

        public SyncController(ILogger<SyncController> logger, InsertService insertService)
        {
            _logger = logger;
            _insertService = insertService;
        }

        [HttpPost("eincrease")]
        public async Task<IActionResult> PostEventIncrease(EventIncreaseDto eventIncreaseDto)
        {
            int eventId = await _insertService.InsertEventIncreaseAsync(eventIncreaseDto);

            Response.Headers.Add("ServerID", eventId.ToString());
            return Ok();
        }

        [HttpPost("emode")]
        public async Task<IActionResult> PostEventMode(EventModeDto eventModeDto)
        {
            int eventId = await _insertService.InsertEventModeAsync(eventModeDto);

            Response.Headers.Add("ServerID", eventId.ToString());
            return Ok();
        }
    }
}
