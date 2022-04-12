using GateWashDataService.Helpers;
using GateWashDataService.Models;
using GateWashDataService.Models.Filters;
using GateWashDataService.Models.GateWashContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult> Get()
        {
            try
            {
                FiltersModel filters = new FiltersModel();

                filters.Regions = await SqlHelper.GetRegions(_context);
                filters.Washes = await SqlHelper.GetWashes(_context);
                filters.PayTerminals = await SqlHelper.GetPayTerminals(_context);

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
