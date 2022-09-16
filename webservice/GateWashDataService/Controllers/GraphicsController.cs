using GateWashDataService.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GraphicsController : ControllerBase
    {
        private readonly ILogger<GraphicsController> _logger;
        private readonly GraphicsRepository _graphicsRepository;

        public GraphicsController(ILogger<GraphicsController> logger, GraphicsRepository graphicsRepository)
        {
            _logger = logger;
            _graphicsRepository = graphicsRepository;
        }

        [HttpGet("each_increase")]
        public async Task<IActionResult> GetEachIncrease([FromQuery] DateTime startDate, DateTime endDate, string eventKindCode = null)
        {
            List<string> washCodes = new List<string> { "M41" };
            var result = await _graphicsRepository.GetCommulativeTotalSplitTerminalsGrapgicDataAsync(startDate, endDate, washCodes);
            return Ok(result);
        }
    }
}
