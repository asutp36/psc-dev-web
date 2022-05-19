using GateWashDataService.Extentions;
using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
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
            IQueryable<IncreaseModel> increases = SqlHelper.GetIncreasesQueryable(_context, parameters);

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
            IQueryable<IncreaseModel> increases = SqlHelper.GetIncreasesQueryable(_context, parameters);

            if (parameters.Terminal == null)
            {
                increases = GetOnlyUserWashes(User.Claims, increases);
            }

            var temp = increases.ToList();

            var groupedIncreases = temp.GroupBy(i => i.DTime.Date,
                                                     i => new { i.Bank, i.Cash, i.Payout },
                                                     (date, values) => new IncreaseModel
                                                     {
                                                         DTime = date.Date,
                                                         Bank = values.Sum(v => v.Bank),
                                                         Cash = values.Sum(v => v.Cash),
                                                         Payout = values.Sum(v => v.Payout)
                                                     }).OrderByDescending(i => i.DTime);

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("days/split-terminals")]
        public async Task<IActionResult> GetByDaysSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            IQueryable<IncreaseModel> increases = SqlHelper.GetIncreasesQueryable(_context, parameters);

            if (parameters.Terminal == null)
            {
                increases = GetOnlyUserWashes(User.Claims, increases);
            }

            var temp = increases.ToList();

            var groupedIncreases = temp.GroupBy(i => new { i.DTime.Date, i.Terminal },
                                                                       i => new { i.Bank, i.Cash, i.Payout },
                                                                       (group, values) => new IncreaseModel
                                                                       {
                                                                           DTime = group.Date,
                                                                           Terminal = group.Terminal,
                                                                           Bank = values.Sum(v => v.Bank),
                                                                           Cash = values.Sum(v => v.Cash),
                                                                           Payout = values.Sum(v => v.Payout)
                                                                       }).OrderByDescending(i => i.DTime);

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("months")]
        public async Task<IActionResult> GetByMonths([FromQuery] GetIncreaseParameters parameters)
        {
            IQueryable<IncreaseModel> increases = SqlHelper.GetIncreasesQueryable(_context, parameters);

            if (parameters.Terminal == null)
            {
                increases = GetOnlyUserWashes(User.Claims, increases);
            }

            var temp = increases.ToList();

            var groupedIncreases = temp.GroupBy(i => new { i.DTime.Year, i.DTime.Month },
                                                i => new { i.Bank, i.Cash, i.Payout },
                                                (date, values) => new IncreaseModel
                                                {
                                                    DTime = new DateTime(date.Year, date.Month, 1),
                                                    Bank = values.Sum(v => v.Bank),
                                                    Cash = values.Sum(v => v.Cash),
                                                    Payout = values.Sum(v => v.Payout)
                                                }).OrderByDescending(i => i.DTime);

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("months/split-terminals")]
        public async Task<IActionResult> GetByMonthsSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            IQueryable<IncreaseModel> increases = SqlHelper.GetIncreasesQueryable(_context, parameters);

            if (parameters.Terminal == null)
            {
                increases = GetOnlyUserWashes(User.Claims, increases);
            }

            var temp = increases.ToList();

            var groupedIncreases = temp.GroupBy(i => new { i.DTime.Year, i.DTime.Month, i.Terminal },
                                                i => new { i.Bank, i.Cash, i.Payout },
                                                (group, values) => new IncreaseModel
                                                {
                                                    DTime = new DateTime(group.Year, group.Month, 1),
                                                    Terminal = group.Terminal,
                                                    Bank = values.Sum(v => v.Bank),
                                                    Cash = values.Sum(v => v.Cash),
                                                    Payout = values.Sum(v => v.Payout)
                                                }).OrderByDescending(i => i.DTime);

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }
    }
}
