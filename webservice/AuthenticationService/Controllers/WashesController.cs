using AuthenticationService.Models.Test;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashesController : ControllerBase
    {
        private readonly ILogger<WashesController> _logger;
        private readonly WashesService _washesService;
        private readonly RegionsService _regionsService;

        public WashesController(ILogger<WashesController> logger, WashesService washesService)
        {
            _logger = logger;
            _washesService = washesService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok();
        }



        [HttpGet("fake")]
        public IActionResult Get()
        {
            WashModel m1 = new WashModel()
            {
                Id = 1,
                Code = "M1",
                Name = "Первая",
                Address = "ул. Фыва, д.2",
                IdRegion = 1
            };

            WashModel m2 = new WashModel()
            {
                Id = 2,
                Code = "M2",
                Name = "Вторая",
                Address = "ул. ТЛРИ, д.123",
                IdRegion = 1
            };

            WashModel m3 = new WashModel()
            {
                Id = 3,
                Code = "M3",
                Name = "Три",
                Address = "ул. HGeivuep, д.rej",
                IdRegion = 2
            };

            WashModel m4 = new WashModel()
            {
                Id = 4,
                Code = "M4",
                Name = "Мойка номер 4",
                Address = "ул. коаз39г4, д.333",
                IdRegion = 3
            };

            WashModel m5 = new WashModel()
            {
                Id = 5,
                Code = "M5",
                Name = "№5",
                Address = "ул. Карла Маркса, д.24",
                IdRegion = 4
            };

            WashModel m6 = new WashModel()
            {
                Id = 6,
                Code = "M6",
                Name = "666",
                Address = "4орт, 666",
                IdRegion = 4
            };

            RegionModel vrn = new RegionModel()
            {
                Id = 1,
                Code = 36,
                Name = "Воронежская область",
                Washes = new List<WashModel>() { m1, m2 }
            };

            RegionModel msk = new RegionModel()
            {
                Id = 2,
                Code = 77,
                Name = "Москва.Север",
                Washes = new List<WashModel>() { m3 }
            };

            RegionModel spb = new RegionModel()
            {
                Id = 3,
                Code = 97,
                Name = "Питер",
                Washes = new List<WashModel>() { m4 }
            };

            RegionModel ekb = new RegionModel()
            {
                Id = 4,
                Code = 66,
                Name = "Екатеринбург",
                Washes = new List<WashModel>() { m5, m6 }
            };

            return Ok(new List<RegionModel>() { vrn, msk, spb, ekb });
        }

    }
}
