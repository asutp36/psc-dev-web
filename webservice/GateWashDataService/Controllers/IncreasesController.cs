using GateWashDataService.Extentions;
using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GateWashDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncreasesController : ControllerBase
    {
        private readonly GateWashDbContext _context;

        public IncreasesController(GateWashDbContext context)
        {
            _context = context;
        }

        private IQueryable<IncreaseModel> GetOnlyUserWashes(IEnumerable<Claim> claims, IQueryable<IncreaseModel> data)
        {
            IEnumerable<string> washCodes = claims.Where(c => c.Type == "Wash").Select(c => c.Value);
            
            var terminalCodes = _context.Washes.Where(w => washCodes.Contains(w.Code)).Select(t => t.Posts.Select(tr => tr.IddeviceNavigation.Code).First());

            IQueryable<IncreaseModel> result = data.Where(t => terminalCodes.Contains(t.TerminalCode));
            return result;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetIncreaseParameters parameters)
        { 
            IQueryable<IncreaseModel> increases = IncreasesRepository.GetAll(_context, parameters);

            if(parameters.Terminal == null)
            {
                increases = GetOnlyUserWashes(User.Claims, increases);
            }

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("days")]
        public async Task<IActionResult> GetByDays([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByDay(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                ProgramsDescription = string.Join(", ", i.Programs),
                                                TypesDescription = string.Join(", ", i.Types),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = i.Programs.Sum(p => p.Value)
                                            })
                                            .OrderBy(i => i.DTime.Date).OrderBy(i => i.TerminalCode);
            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("days/split-terminals")]
        public async Task<IActionResult> GetByDaysSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByDaySplitTerminals(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                Terminal = i.Terminal,
                                                TerminalCode = i.TerminalCode,
                                                ProgramsDescription = string.Join(", ", i.Programs),
                                                TypesDescription = string.Join(", ", i.Types),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = i.Programs.Sum(p => p.Value)
                                            })
                                            .OrderBy(i => i.DTime.Date).OrderBy(i => i.TerminalCode);
            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("months")]
        public async Task<IActionResult> GetByMonths([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByMonth(_context, parameters, washes)
                                             .Select(i => new GroupedIncreaseModel
                                             {
                                                 DTime = i.DTime,
                                                 ProgramsDescription = string.Join(", ", i.Programs),
                                                 TypesDescription = string.Join(", ", i.Types),
                                                 Amount = i.Types.Sum(t => t.Value),
                                                 ProgramCount = i.Programs.Sum(p => p.Value)
                                             })
                                             .OrderBy(i => i.DTime.Date).OrderBy(i => i.TerminalCode);
            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("months/split-terminals")]
        public async Task<IActionResult> GetByMonthsSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByMonthSplitTerminals(_context, parameters, washes)
                                             .Select(i => new GroupedIncreaseModel
                                             {
                                                 DTime = i.DTime,
                                                 Terminal = i.Terminal,
                                                 TerminalCode = i.TerminalCode,
                                                 ProgramsDescription = string.Join(", ", i.Programs),
                                                 TypesDescription = string.Join(", ", i.Types),
                                                 Amount = i.Types.Sum(t => t.Value),
                                                 ProgramCount = i.Programs.Sum(p => p.Value)
                                             })
                                             .OrderBy(i => i.DTime.Date).OrderBy(i => i.TerminalCode);
            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }
    }
}
