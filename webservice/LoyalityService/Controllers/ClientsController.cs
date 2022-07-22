using LoyalityService.Models;
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
            if(clientPhone <= 0)
            {
                BadRequest("Телефон клиента не может быть <= 0");
            }

            int digitsCount = (int)Math.Log10(clientPhone) + 1;
            if(digitsCount != 11)
            {
                return BadRequest($"Неверное количество цифр в номере {clientPhone}. Необходимо 11, а введено {digitsCount}");
            }

            WashingModel washing = await _washDiscount.GetClientLastWashing(clientPhone);
            if(washing == null)
            {
                return NotFound("Похоже, что вы у нас ещё ни разу не мылись");
            }

            return Ok(washing);
        }
    }
}
