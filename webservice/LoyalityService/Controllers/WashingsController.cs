using LoyalityService.Models;
using LoyalityService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<WashingsController> _logger;
        public WashingsController(IAsyncDiscountManager washDiscountService, ILogger<WashingsController> logger)
        {
            _washDiscountService = washDiscountService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> WriteWashing([FromBody]WashingModel washing)
        {
            _logger.LogInformation($"параметры запроса: {JsonConvert.SerializeObject(washing)}");
            //if(!DateTime.TryParse(washing.DTime, out DateTime dtime))
            //{
            //    return BadRequest("Не удалось распарсить дату");
            //}

            //// проверить, что девайс существует
            //if(!(await _washDiscountService.IsDeviceExistsAsync(washing.Device)))
            //{
            //    _logger.LogError($"Девайс {washing.Device} не найден");
            //    return NotFound($"Девайс {washing.Device} не найден");
            //}

            //// проверить, что программа существует
            //if (!(await _washDiscountService.IsProgramExistsAsync(washing.Program)))
            //{
            //    _logger.LogError($"Программа {washing.Program} не найдена");
            //    return NotFound($"Программа {washing.Program} не найдена");
            //}

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

        [HttpPost("test")]
        public async Task<IActionResult> TestPost()
        {
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> TestGet()
        {
            return Ok();
        }
    }
}
