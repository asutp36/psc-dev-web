using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpResponse = Backend.Controllers.Supplies.HttpResponse;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostStateController : ControllerBase
    {
        private ModelDbContext _model;
        private readonly ILogger<PostStateController> _logger;

        public PostStateController(ILogger<PostStateController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Данные для страницы статистики поста")]
        [SwaggerResponse(200, Type = typeof(PostStateViewModel))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("{postCode}")]
        public IActionResult Get(string postCode)
        {
            try
            {
                PostStateViewModel result = new PostStateViewModel();
                result.washCode = SqlHelper.GetWashCode(postCode);
                result.lastSync = SqlHelper.GetLastPostSync(postCode).ToString("yyyy-MM-dd HH:mm:ss");

                HttpResponse response = HttpSender.SendGet("");

                // сервис пинга
                result.lastPing = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


                return Ok(result);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }
    }
}
