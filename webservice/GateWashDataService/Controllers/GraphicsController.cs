using GateWashDataService.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetEachIncrease([FromQuery]DateTime startDate, DateTime endDate, string eventKindCode = null, string groupingType = null)
        {
            List<string> washCodes = new List<string> { "M41" };
            GraphicsDataModel graphic;
           
            switch (groupingType)
            {
                case null:
                    graphic = await _graphicsRepository.GetCommulativeTotalSplitTerminalsGraphicDataAsync(startDate, endDate, washCodes);
                    break;

                case "byhour":
                    graphic = await _graphicsRepository.GetCommulativeTotalSplitTerminalsGraphicData_ByHourAsync(startDate, endDate, washCodes);
                    break;

                case "byday":
                    graphic = await _graphicsRepository.GetCommulativeTotalSplitTerminalsGraphicData_ByDayAsync(startDate, endDate, washCodes);
                    break;

                case "bymonth":
                    graphic = await _graphicsRepository.GetCommulativeTotalSplitTerminalsGraphicData_ByMonthAsync(startDate, endDate, washCodes);
                    break;

                default:
                    graphic = null;
                    break;
            }
            
            if(graphic == null)
            {
                return NoContent();
            }

            return Ok(graphic);
        }
    }
}
