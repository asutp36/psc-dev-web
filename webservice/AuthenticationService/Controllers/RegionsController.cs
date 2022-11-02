using AuthenticationService.Models;
using AuthenticationService.Models.DTOs;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly ILogger<RegionsController> _logger;
        private readonly RegionsService _regionsService;

        public RegionsController(ILogger<RegionsController> logger, RegionsService regionsService)
        {
            _logger = logger;
            _regionsService = regionsService;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить все регионы")]
        [SwaggerResponse(200, Type = typeof(List<RegionDTO>))]
        [SwaggerResponse(500, Description = "Что-то пошло не так")]
        #endregion
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _regionsService.GetAsync());
        }
    }
}
