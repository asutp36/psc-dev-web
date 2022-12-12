using GateWashDataService.Extentions;
using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
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
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _clientService;

        public ClientsController(ClientService clientService)
        {
            _clientService = clientService;
        }

        [HttpGet("filters")]
        [Authorize]
        public async Task<IActionResult> GetFilters()
        {
            ClientFilters filters = await _clientService.GetFiltersAsync(User.Claims.Where(c => c.Type == "GateWash").Select(c => c.Value));

            return Ok(filters);
        }

        [HttpGet("entrances")]
        public async Task<IActionResult> GetEntrances([FromQuery]GetClientActionsParameters parameters)
        {
            List<ClientEntrance> entrances = await _clientService.GetEntrancesAsync();

            string sortingRule;
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            entrances = Sorting.Sort<ClientEntrance>(entrances.AsQueryable(), sortingRule).ToList();

            PagedList<ClientEntrance> result = PagedList<ClientEntrance>.ToPagedList(entrances.AsQueryable(), parameters.Paging);

            return Ok(result);
        }

        [HttpGet("entrances/total-count")]
        public async Task<IActionResult> GetEntrancesTotalCount([FromQuery] GetClientActionsParameters parameters)
        {
            List<ClientEntrance> entrances = await _clientService.GetEntrancesAsync();

            return Ok(entrances.Count);
        }

        [HttpGet("exits")]
        public async Task<IActionResult> GetExits([FromQuery]GetClientActionsParameters parameters)
        {
            List<ClientExit> exits = await _clientService.GetExitsAsync();

            string sortingRule;
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            exits = Sorting.Sort<ClientExit>(exits.AsQueryable(), sortingRule).ToList();

            PagedList<ClientExit> result = PagedList<ClientExit>.ToPagedList(exits.AsQueryable(), parameters.Paging);

            return Ok(result);
        }

        [HttpGet("exits/total-count")]
        public async Task<IActionResult> GetExitsTotalCount([FromQuery] GetClientActionsParameters parameters)
        {
            List<ClientExit> exits = await _clientService.GetExitsAsync();

            return Ok(exits.Count);
        }
    }
}
