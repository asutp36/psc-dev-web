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
        private readonly IAsyncDiscountManager _washDiscount;

        public ClientsController(IAsyncDiscountManager washDiscount)
        {
            _washDiscount = washDiscount;
        }

        [HttpGet("{clientPhone}")]
        public async Task<IActionResult> CheckPhone(long clientPhone)
        {
            if (clientPhone <= 0)
            {
                BadRequest("Телефон клиента не может быть <= 0");
            }

            int digitsCount = (int)Math.Log10(clientPhone) + 1;
            if (digitsCount != 10)
            {
                return BadRequest($"Неверное количество цифр в номере {clientPhone}. Необходимо 10, а введено {digitsCount}. Номер вводится без 7");
            }

            // к номеру надо прибавить 70000000000, потому что от фронта приходит номер без семёрки
            clientPhone += 70000000000;

            try
            {
                await _washDiscount.GetClientByPhoneAsync(clientPhone);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound("Похоже, что вы у нас ещё ни разу не мылись");
            }

            return Ok();
        }

        [HttpGet("{clientPhone}/washings")]
        public async Task<IActionResult> GetLastWashing(long clientPhone)
        {
            if(clientPhone <= 0)
            {
                BadRequest("Телефон клиента не может быть <= 0");
            }

            int digitsCount = (int)Math.Log10(clientPhone) + 1;
            if(digitsCount != 10)
            {
                return BadRequest($"Неверное количество цифр в номере {clientPhone}. Необходимо 10, а введено {digitsCount}. Номер вводится без 7");
            }

            // к номеру надо прибавить 70000000000, потому что от фронта приходит номер без семёрки
            clientPhone += 70000000000;

            IEnumerable<WashingModel> washings = await _washDiscount.GetClientLast10WashingsAsync(clientPhone);
            if(washings == null || washings.Count() == 0)
            {
                return NotFound("Похоже, что вы у нас ещё ни разу не мылись");
            }

            return Ok(washings);
        }

        [HttpGet("{clientPhone}/promotions")]
        public async Task<IActionResult> GetCurrentPromotions(long clientPhone)
        {
            if (clientPhone <= 0)
            {
                BadRequest("Телефон клиента не может быть <= 0");
            }

            int digitsCount = (int)Math.Log10(clientPhone) + 1;
            if (digitsCount != 10)
            {
                return BadRequest($"Неверное количество цифр в номере {clientPhone}. Необходимо 10, а введено {digitsCount}. Номер вводится без 7");
            }

            // к номеру надо прибавить 70000000000, потому что от фронта приходит номер без семёрки
            clientPhone += 70000000000;

            try
            {
                await _washDiscount.GetClientByPhoneAsync(clientPhone);
            }
            catch(KeyNotFoundException e)
            {
                return NotFound("Похоже, что вы у нас ещё ни разу не мылись");
            }

            ClientPromotions promotions = await _washDiscount.GetCurrentPromotions(clientPhone);
            return Ok(promotions);
        }
    }
}
