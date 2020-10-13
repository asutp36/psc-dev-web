using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangerOperationsController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet("sums")]
        public IActionResult GetSumsByChanger()
        {
            try
            {
                return Ok();
            } 
            catch(Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}