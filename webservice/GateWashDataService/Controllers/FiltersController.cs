using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GateWashDataService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FiltersController : ControllerBase
    {
        private readonly GateWashDbContext _context;
        private readonly ILogger<FiltersController> _logger;

        public FiltersController(GateWashDbContext context, ILogger<FiltersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Get()
        {
            try
            {
                FiltersModel filters = new FiltersModel();
                filters.PayTerminals = new List<PayTerminalModel>();
                foreach (Claim c in User.Claims.Where(cl => cl.Type == "Wash").ToList())
                {
                    var terminals = _context.Posts.Where(p => p.IddeviceNavigation.IddeviceTypeNavigation.Code == "pay"
                    && p.IdwashNavigation.Code == c.Value).Select(t => new PayTerminalModel
                    {
                        IdDevice = t.Iddevice,
                        Code = t.IddeviceNavigation.Code,
                        Name = t.IddeviceNavigation.Name,
                        IdWash = t.Idwash
                    }).ToList();
 
                    filters.PayTerminals.AddRange(terminals);
                }
                

                filters.Regions = await SqlHelper.GetRegions(_context);
                filters.Washes = await SqlHelper.GetWashes(_context);
                //filters.PayTerminals = await SqlHelper.GetPayTerminals(_context);
                filters.Programs = await SqlHelper.GetPrograms(_context);

                return Ok(filters);
            }
            catch (Exception e)
            {
                _logger.LogError("Непредвиденная ошибка: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error()
                {
                    errorType = "unexpected",
                    alert = "Что-то пошло не так в ходе работы сервера",
                    errorCode = "Ошибка при обращении к серверу",
                    errorMessage = "Попробуйте снова или обратитесь к специалисту"
                });
            }
        }

    }
}
