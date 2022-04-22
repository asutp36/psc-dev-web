using GateWashDataService.Extentions;
using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.GateWashContext;
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
        private readonly GateWashDbContext _context;

        public CollectsController(GateWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] GetCollectsParameters parameters)
        {
            List<CollectModel> collects = SqlHelper.GetCollects(_context, parameters);

            PagedList<CollectModel> result = PagedList<CollectModel>.ToPagedList(collects.AsQueryable(), parameters.Paging);

            PagedList<CollectModel>.PrepareHTTPResponseMetadata(Response, result);

            return Ok(result);
        }
    }
}
