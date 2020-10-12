using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Stored_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashBoxController : ControllerBase
    {
        [SwaggerResponse(200, Type = typeof(GetBoxByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet("washs")]
        public IActionResult ByWashs()
        {
            return Ok();
        }

        [SwaggerResponse(200, Type = typeof(GetBoxByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet("posts")]
        public IActionResult ByPosts()
        {
            return Ok();
        }
    }
}