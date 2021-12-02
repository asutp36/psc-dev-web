using Backend.Controllers.Supplies;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashController : ControllerBase
    {
        private ModelDbContext _model;
        private readonly ILogger<WashController> _logger;

        public WashController(ILogger<WashController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }

        [SwaggerOperation(Summary = "Данные для страницы наличия в боксах по мойкам")]
        [SwaggerResponse(200, Type = typeof(List<Wash>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<Wash> washes = new List<Wash>();

            }
            catch(Exception e)
            {
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }

            return null;
        }
    }
}
