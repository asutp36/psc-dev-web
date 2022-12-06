using GateWashDataService.Extentions;
using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GateWashDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollectsController : ControllerBase
    {
        private readonly GateWashDbContext _context;
        private readonly CollectService _collectService;

        public CollectsController(GateWashDbContext context, CollectService collectService)
        {
            _context = context;
            _collectService = collectService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCollectsParameters parameters)
        {
            PagedList<CollectModel> result = PagedList<CollectModel>.ToPagedList(await _collectService.GetAsync(parameters), parameters.Paging);

            PagedList<CollectModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCountWrong([FromQuery] GetCollectsParameters parameters)
        {
            IQueryable<CollectModel> collects = await _collectService.GetAsync(parameters);

            //PagedList<CollectModel> result = PagedList<CollectModel>.ToPagedList(collects.AsQueryable(), parameters.Paging);

            //PagedList<CollectModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(collects.Count());
        }

        [HttpGet("total_count")]
        public async Task<IActionResult> GetCountRight([FromQuery] GetCollectsParameters parameters)
        {
            IQueryable<CollectModel> collects = await _collectService.GetAsync(parameters);

            return Ok(collects.Count());
        }
    }
}
