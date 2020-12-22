using Backend.Controllers.Supplies;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostMonitoringController : ControllerBase
    {
        private ModelDbContext _model;
        private readonly ILogger<PostMonitoringController> _logger;

        public PostMonitoringController(ILogger<PostMonitoringController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }

        #region Swagger Annotations
        #endregion
        [HttpGet("{postCode}")]
        public IActionResult Get(string postCode)
        {
            try
            {
                return Ok();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
