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
    public class WashingsController : ControllerBase
    {
        private readonly IAsyncDiscountManager _washDiscountService;
        public WashingsController(IAsyncDiscountManager washDiscountService)
        {
            _washDiscountService = washDiscountService;
        }

        [HttpPost]
        public async Task<IActionResult> WriteWashing([FromBody]WashingModel washing)
        {
            // проверить, что девайс существует
            if(!(await _washDiscountService.IsDeviceExistsAsync(washing.Device)))
            {
                return NotFound($"Девайс {washing.Device} не найден");
            }

            // проверить, что программа существует
            if (!(await _washDiscountService.IsProgramExistsAsync(washing.Program)))
            {
                return NotFound($"Программа {washing.Program} не найдена");
            }

            // ServerID добавленного запуска
            int idWashing = 0;
            try
            {
                idWashing = await _washDiscountService.WriteWashingAsync(washing);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(500, e.Message);
            }

            return Created("", idWashing);
        }
    }
}
