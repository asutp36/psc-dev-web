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
            var washes = new List<string>() { "M41", "RP-R36-M01" };
            var increases = IncreasesRepository.GetGroupedByHour(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                ProgramsDescription = string.Join(", ", i.Programs),
                                                TypesDescription = string.Join(", ", i.Types),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });
            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction}";

            increases = Sort(increases, sortingRule);

            return Ok(increases);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            IQueryable<IncreaseModel> increases = IncreasesRepository.GetAll(_context, parameters, washes).ToList().AsQueryable();

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            increases = Sort(increases, sortingRule);

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }
        
        [Authorize]
        [HttpGet("total_count")]
        public async Task<IActionResult> GetTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            IQueryable<IncreaseModel> increases = IncreasesRepository.GetAll(_context, parameters, washes).ToList().AsQueryable();

            return Ok(increases.Count());
        }

        [Authorize]
        [HttpGet("commulative-total")]
        public async Task<IActionResult> GetCommulativeTotalSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            IQueryable<IncreaseCommulativeTotalModel> increases = IncreasesRepository.GetCommulativeTotalSplitTerminals(_context, parameters, washes);

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            increases = Sort(increases, sortingRule);

            PagedList<IncreaseCommulativeTotalModel> result = PagedList<IncreaseCommulativeTotalModel>.ToPagedList(increases, parameters.Paging);

            PagedList<IncreaseCommulativeTotalModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("hours")]
        public async Task<IActionResult> GetByHours([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByHour(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc";

            increases = Sort(increases, sortingRule);

            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("hours/total_count")]
        public async Task<IActionResult> GetByHoursTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByHour(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            return Ok(increases.Count());
        }

        [Authorize]
        [HttpGet("hours/split-terminals")]
        public async Task<IActionResult> GetByHoursSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByHourSplitTerminals(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                Terminal = i.Terminal,
                                                TerminalCode = i.TerminalCode,
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            increases = Sort(increases, sortingRule);

            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("hours/split-terminals/total_count")]
        public async Task<IActionResult> GetByHoursSplitTerminalsTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByHourSplitTerminals(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                Terminal = i.Terminal,
                                                TerminalCode = i.TerminalCode,
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            return Ok(increases.Count());
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
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc";

            increases = Sort(increases, sortingRule);

            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("days/total_count")]
        public async Task<IActionResult> GetByDaysTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByDay(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            return Ok(increases.Count());
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
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            increases = Sort(increases, sortingRule);

            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("days/split-terminals/total_count")]
        public async Task<IActionResult> GetByDaysSplitTerminalsTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByDaySplitTerminals(_context, parameters, washes)
                                            .Select(i => new GroupedIncreaseModel
                                            {
                                                DTime = i.DTime,
                                                Terminal = i.Terminal,
                                                TerminalCode = i.TerminalCode,
                                                ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                Amount = i.Types.Sum(t => t.Value),
                                                ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                            });

            return Ok(increases.Count());
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
                                                 ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                 TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                 Amount = i.Types.Sum(t => t.Value),
                                                 ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                             });

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc";

            increases = Sort(increases, sortingRule);

            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("months/total_count")]
        public async Task<IActionResult> GetByMonthsTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByMonth(_context, parameters, washes)
                                             .Select(i => new GroupedIncreaseModel
                                             {
                                                 DTime = i.DTime,
                                                 ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                 TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                 Amount = i.Types.Sum(t => t.Value),
                                                 ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                             });

            return Ok(increases.Count());
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
                                                 ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                 TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                 Amount = i.Types.Sum(t => t.Value),
                                                 ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                             });

            string sortingRule = "";
            if (parameters.Sorting == null || string.IsNullOrEmpty(parameters.Sorting.Field) || string.IsNullOrEmpty(parameters.Sorting.Direction))
                sortingRule = "Dtime desc,TerminalCode asc";
            else
                sortingRule = $"{parameters.Sorting.Field} {parameters.Sorting.Direction},Dtime desc,TerminalCode asc";

            increases = Sort(increases, sortingRule);

            PagedList<GroupedIncreaseModel> result = PagedList<GroupedIncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            PagedList<GroupedIncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("months/split-terminals/total_count")]
        public async Task<IActionResult> GetByMonthsSplitTerminalsTotalCount([FromQuery] GetIncreaseParameters parameters)
        {
            var washes = User.Claims.Where(c => c.Type == "Wash").Select(c => c.Value).ToList();
            var increases = IncreasesRepository.GetGroupedByMonthSplitTerminals(_context, parameters, washes)
                                             .Select(i => new GroupedIncreaseModel
                                             {
                                                 DTime = i.DTime,
                                                 Terminal = i.Terminal,
                                                 TerminalCode = i.TerminalCode,
                                                 ProgramsDescription = string.Join(", ", i.Programs.OrderBy(p => p.DisplayOrder)),
                                                 TypesDescription = string.Join(", ", i.Types.OrderBy(t => t.DisplayOrder)),
                                                 Amount = i.Types.Sum(t => t.Value),
                                                 ProgramCount = (int)i.Programs.Sum(p => p.Value)
                                             });

            return Ok(increases.Count());
        }
    }
}
