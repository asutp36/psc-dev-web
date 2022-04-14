using GateWashDataService.Extentions;
using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] GetIncreaseParameters parameters)
        {
            List<IncreaseModel> increases = SqlHelper.GetIncreases(_context, parameters);

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            var metadata = new
            {
                result.CurrentPage,
                result.HasNext,
                result.HasPrevious,
                result.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(result);
        }

        [HttpGet("days")]
        public async Task<IActionResult> GetByDays([FromQuery] GetIncreaseParameters parameters)
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

            PagedList<IncreaseModel> result = PagedList<IncreaseModel>.ToPagedList(increases.AsQueryable(), parameters.Paging);

            var metadata = new
            {
                result.CurrentPage,
                result.HasNext,
                result.HasPrevious,
                result.TotalPages
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(result);
        }
    }
}
