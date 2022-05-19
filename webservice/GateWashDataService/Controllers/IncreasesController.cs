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

        private IEnumerable<string> GetUserWashes(IEnumerable<Claim> claims)
        {
            var result = new List<string>();
            foreach (Claim c in claims)
                if (c.Type == "Wash")
                    result.Add(c.Value);
            return result;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetIncreaseParameters parameters)
        {
            
            //List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);
            //PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);
            IQueryable<IncreaseModel> increases = SqlHelper.GetIncreasesQueryable(_context, parameters);

            if(parameters.Terminal == null)
            {
                IEnumerable<string> washes = GetUserWashes(User.Claims);
                var terminals = _context.Washes.Where(w => washes.Contains(w.Code)).Select(t => t.Posts.Select(tr => tr.IddeviceNavigation.Code).First());
                increases = increases.Where(t => terminals.Contains(t.Terminal));
            }

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases, parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetCount([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            //PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            //PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(increases.Count);
        }

        [HttpGet("days")]
        public async Task<IActionResult> GetByDays([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Date })
                                                                    .Select(x => new IncreaseModel
                                                                    {
                                                                        DTime = x.Key.Date,
                                                                        Bank = x.Sum(i => i.Bank),
                                                                        Cash = x.Sum(i => i.Cash),
                                                                        Payout = x.Sum(i => i.Payout)
                                                                    });

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("days/count")]
        public async Task<IActionResult> GetByDaysCount([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Date })
                                                                    .Select(x => new IncreaseModel
                                                                    {
                                                                        DTime = x.Key.Date,
                                                                        Bank = x.Sum(i => i.Bank),
                                                                        Cash = x.Sum(i => i.Cash),
                                                                        Payout = x.Sum(i => i.Payout)
                                                                    });


            return Ok(groupedIncreases.Count());
        }

        [HttpGet("days/split-terminals")]
        public async Task<IActionResult> GetByDaysSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Date, i.Terminal })
                                                                    .Select(x => new IncreaseModel
                                                                    {
                                                                        DTime = x.Key.Date,
                                                                        Terminal = x.Key.Terminal,
                                                                        Bank = x.Sum(i => i.Bank),
                                                                        Cash = x.Sum(i => i.Cash),
                                                                        Payout = x.Sum(i => i.Payout)
                                                                    });

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("days/split-terminals/count")]
        public async Task<IActionResult> GetByDaysSplitTerminalsCount([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Date, i.Terminal })
                                                                    .Select(x => new IncreaseModel
                                                                    {
                                                                        DTime = x.Key.Date,
                                                                        Terminal = x.Key.Terminal,
                                                                        Bank = x.Sum(i => i.Bank),
                                                                        Cash = x.Sum(i => i.Cash),
                                                                        Payout = x.Sum(i => i.Payout)
                                                                    });

            

            return Ok(groupedIncreases.Count());
        }

        [HttpGet("months")]
        public async Task<IActionResult> GetByMonths([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Year, i.DTime.Month })
                                                                   .Select(x => new IncreaseModel
                                                                   {
                                                                       DTime = new DateTime(x.Key.Year, x.Key.Month, 1),
                                                                       Bank = x.Sum(i => i.Bank),
                                                                       Cash = x.Sum(i => i.Cash),
                                                                       Payout = x.Sum(i => i.Payout)
                                                                   });

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("months/count")]
        public async Task<IActionResult> GetByMonthsCount([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Year, i.DTime.Month })
                                                                   .Select(x => new IncreaseModel
                                                                   {
                                                                       DTime = new DateTime(x.Key.Year, x.Key.Month, 1),
                                                                       Bank = x.Sum(i => i.Bank),
                                                                       Cash = x.Sum(i => i.Cash),
                                                                       Payout = x.Sum(i => i.Payout)
                                                                   });


            return Ok(groupedIncreases.Count());
        }

        [HttpGet("months/split-terminals")]
        public async Task<IActionResult> GetByMonthsSplitTerminals([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Year, i.DTime.Month, i.Terminal })
                                                                   .Select(x => new IncreaseModel
                                                                   {
                                                                       DTime = new DateTime(x.Key.Year, x.Key.Month, 1),
                                                                       Terminal = x.Key.Terminal,
                                                                       Bank = x.Sum(i => i.Bank),
                                                                       Cash = x.Sum(i => i.Cash),
                                                                       Payout = x.Sum(i => i.Payout)
                                                                   });

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(groupedIncreases.AsQueryable(), parameters.Paging);

            PagedList<IncreaseModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }

        [HttpGet("months/split-terminals/count")]
        public async Task<IActionResult> GetByMonthsSplitTerminalsCount([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            IEnumerable<IncreaseModel> groupedIncreases = increases.GroupBy(i => new { i.DTime.Year, i.DTime.Month, i.Terminal })
                                                                   .Select(x => new IncreaseModel
                                                                   {
                                                                       DTime = new DateTime(x.Key.Year, x.Key.Month, 1),
                                                                       Terminal = x.Key.Terminal,
                                                                       Bank = x.Sum(i => i.Bank),
                                                                       Cash = x.Sum(i => i.Cash),
                                                                       Payout = x.Sum(i => i.Payout)
                                                                   });


            return Ok(groupedIncreases.Count());
        }
    }
}
