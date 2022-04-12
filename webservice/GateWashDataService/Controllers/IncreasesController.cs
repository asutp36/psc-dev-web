using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
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
            return Ok(SqlHelper.GetIncreases(_context, parameters.startDate, parameters.endDate, 
                parameters.onlyBank, parameters.onlyCash, parameters.onlyNotes, parameters.terminal, parameters.program));
        }

        [HttpGet("days")]
        public async Task<IActionResult> GetByDays([FromQuery] GetIncreaseParameters parameters)
        {
            var result = SqlHelper.GetIncreases(_context, parameters.startDate, parameters.endDate, parameters.onlyBank, 
                parameters.onlyCash, parameters.onlyNotes, parameters.terminal, parameters.program)
                .GroupBy(i => new { i.DTime.Date, i.Terminal })
                .Select(x => new IncreaseModel
                {
                    DTime = x.Key.Date,
                    Terminal = x.Key.Terminal,
                    Bank = x.Sum(i => i.Bank),
                    Cash = x.Sum(i => i.Cash),
                    Amount = x.Sum(i => i.Amount),
                    Payout = x.Sum(i => i.Payout)
                });

            return Ok(result);
        }

        [HttpGet("test")]
        public async Task<IActionResult> GetAllTest(DateTime startDate, DateTime endDate,
           string terminal = null, string program = null, bool onlyBank = false, bool onlyCash = false, bool onlyCheque = false, bool onlyNotes = false)
        {
            Random rnd = new Random();
            List<IncreaseModel> result = new List<IncreaseModel>();
            for(int i = 0; i < 20; i++)
            {
                IncreaseModel m = new IncreaseModel()
                {
                    DTime = DateTime.Now.AddDays(rnd.Next(-1, 1)),
                    Terminal = "test-post",
                    Program = "express",
                    Bank = rnd.Next(0, 300),
                    Cash = rnd.Next(0, 200)
                };

                if (rnd.Next(10) > 5)
                    m.Note = "Произошла какая-то ошибка";
                m.Cheque = rnd.Next(10) < 5;

                if (rnd.Next(10) > 8)
                    m.Bank = 0;
                if(rnd.Next(10) > 5 && m.Bank != 0)
                    m.Cash = 0;
                m.Payout = rnd.Next(0, m.Amount);
                result.Add(m);
            }

            return Ok(result.Where(r => r.DTime >= startDate && r.DTime <= endDate && (!onlyNotes || r.Note != null)
                                    && (terminal == null || r.Terminal == terminal) && (program == null || r.Program == program)
                                    && (!onlyBank || r.Bank != 0) && (!onlyCash || r.Cash != 0))
                .OrderBy(r => r.DTime));
        }
    }
}
