using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsMobileService.Controllers.Supplies;
using CardsMobileService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CardsMobileService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        ILogger<PostController> _logger;
        CardsApi _cardsApi;
        PostApi _postApi;

        public PostController(ILogger<PostController> logger, CardsApi cardsApi, PostApi postApi)
        {
            _logger = logger;
            _cardsApi = cardsApi;
            _postApi = postApi;
        }

        [HttpPost("start")]
        public IActionResult Start(StartPostModel model)
        {
            _logger.LogInformation("StartPost: запуск с параметрами\n" + JsonConvert.SerializeObject(model));

            if (!ModelState.IsValid)
            {
                _logger.LogError("StartPost: модель не прошла валидацию" + Environment.NewLine);
                return BadRequest();
            }

            if (!_cardsApi.IsExist(model.cardNum))
            {
                _logger.LogError("StartPost: карта с номером {0} не существует" + Environment.NewLine);
                return NotFound();
            }

            if (!_postApi.IsExist(model.post))
            {
                _logger.LogError("StartPost: не найден код поста" + Environment.NewLine);
                return StatusCode(405);
            }

            return Ok();
        }
    }
}