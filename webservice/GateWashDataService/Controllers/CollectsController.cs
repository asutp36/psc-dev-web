using GateWashDataService.Extentions;
using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
using GateWashDataService.Services;
using Microsoft.AspNetCore.Authorization;
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
        private readonly CollectService _collectService;
        private readonly WashesRepository _washesRepository;

        public CollectsController(CollectService collectService, WashesRepository washesRepository)
        {
            _collectService = collectService;
            _washesRepository = washesRepository;
        }

        [HttpGet("filters")]
        [Authorize]
        public async Task<IActionResult> GetFilters()
        {
            CollectFilters filters = await _collectService.GetFiltersAsync(User.Claims.Where(c => c.Type == "GateWash" || c.Type == "RobotWash").Select(c => c.Value));
            return Ok(filters);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetCollectsParameters parameters)
        {
            IEnumerable<string> terminals = await _washesRepository.GetTerminalCodesByWashesAsync(User.Claims.Where(c => c.Type == "GatteWash" || c.Type == "RobotWash").Select(c => c.Value));
            PagedList<CollectModel> result = PagedList<CollectModel>.ToPagedList(await _collectService.GetAsync(parameters, terminals), parameters.Paging);

            PagedList<CollectModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("total_count")]
        [Authorize]
        public async Task<IActionResult> GetTotalCount([FromQuery] GetCollectsParameters parameters)
        {
            IEnumerable<string> terminals = await _washesRepository.GetTerminalCodesByWashesAsync(User.Claims.Where(c => c.Type == "GatteWash" || c.Type == "RobotWash").Select(c => c.Value));
            IQueryable<CollectModel> collects = await _collectService.GetAsync(parameters, terminals);

            return Ok(collects.Count());
        }
    }
}
