using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestConnectionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashesController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetConnection()
        {
            try
            {
                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500);
            }
        }
    }
}
