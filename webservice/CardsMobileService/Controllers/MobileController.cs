using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CardsMobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MobileController : ControllerBase
    {
        public IActionResult PostIncrease()
        {
            return Ok();
        }

        public IActionResult GetBalance()
        {
            return Ok();
        }

        public IActionResult GetCards()
        {
            return Ok();
        }

        public IActionResult UpdatePhone()
        {
            return Ok();
        }

        public IActionResult StartPost()
        {
            return Ok();
        }

        public IActionResult PostCard()
        {
            return Ok();
        }
    }
}