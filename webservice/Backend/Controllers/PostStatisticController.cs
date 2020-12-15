using Backend.Controllers.Supplies;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class PostStatisticController : ControllerBase
    {
        private ModelDbContext _model;
        private readonly ILogger<PostStatisticController> _logger;

        public PostStatisticController(ILogger<PostStatisticController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }


        [Authorize]
        [HttpGet]
        public IActionResult Get()
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
