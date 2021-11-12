using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechCardsController : ControllerBase
    {
        ILogger<TechCardsController> _logger;
        IConfiguration _config;

        public TechCardsController(ILogger<TechCardsController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить технические карты по коду мойки")]
        [SwaggerResponse(200, Type = typeof(List<GroupViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("wash/{washCode}")]
        public IActionResult GetCardsByWash(string washCode)
        {
            try
            {
                var cards = SqlHelper.GetGroupsTechCardsByWash(washCode);
                return Ok(cards);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }


        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить типы технических карт")]
        [SwaggerResponse(200, Type = typeof(List<CardTypeViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("types")]
        public IActionResult GetCardTypes()
        {
            try
            {
                return Ok(SqlHelper.GetTechCardTypes());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
