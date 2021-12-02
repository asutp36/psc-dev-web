using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        [Authorize]
        [HttpGet("filters")]
        public IActionResult GetFilters()
        {
            try
            {

                return Ok();
            }
            catch(Exception e)
            {
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }
        
    }
}