using LoyalityService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyalityService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IDiscountManager _washDiscount;

        public ClientsController(IDiscountManager washDiscount)
        {
            _washDiscount = washDiscount;
        }

        [HttpGet("{clientPhone}")]
        public async Task<IActionResult> GetLastWashing(long clientPhone)
        {
            return Ok();
        }
    }
}
