using GateWashDataService.Helpers;
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
        public async Task<IActionResult> GetAll(DateTime startDate, DateTime endDate, 
            string terminal = null, string program = null, bool onlyBank = false, bool onlyCash = false, bool onlyCheque = false, bool onlyNotes = false)
        {
            return Ok(SqlHelper.GetIncomes(_context, startDate, endDate, onlyBank, onlyCash, onlyNotes, terminal, program));
        }
    }
}
