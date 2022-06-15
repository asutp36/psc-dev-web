using GateWashDataService.Extentions;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using GateWashDataService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
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

        public IQueryable<T> Sort<T>(IQueryable<T> entities, string orderByQueryString)
        {
            if (!entities.Any())
                return entities;
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                return entities;
            }
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(T).GetProperties();
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;
                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {sortingOrder}, ");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            return entities.OrderBy(orderQuery);
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public IActionResult Test([FromQuery] GetIncreaseParameters parameters)
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
                                            });
            increases = Sort(increases, "Dtime desc");
            return Ok(increases);
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

            increases = Sort(increases, "Dtime desc,TerminalCode asc");

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
                                            });
            increases = Sort(increases, "Dtime desc");
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
                                            });
            increases = Sort(increases, "Dtime desc,TerminalCode asc");
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
                                             });
            increases = Sort(increases, "Dtime desc");
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
                                             });
            increases = Sort(increases, "Dtime desc,TerminalCode asc");
            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }
    }
}
