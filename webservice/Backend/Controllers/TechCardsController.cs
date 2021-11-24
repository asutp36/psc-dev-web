using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechCardsController : ControllerBase
    {
        ILogger<TechCardsController> _logger;
        IConfiguration _config;

        public TechCardsController(ILogger<TechCardsController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить технические карты по коду мойки")]
        [SwaggerResponse(200, Type = typeof(List<GroupViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("wash/{washCode}")]
        public IActionResult GetCardsByWash(string washCode)
        {
            try
            {
                if (!SqlHelper.IsWashExists(washCode))
                    return NotFound(new Error("Мойка не найдена", "badvalue"));

                var cards = SqlHelper.GetGroupsTechCardsByWash(washCode);
                return Ok(cards);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить технические карты по коду группы")]
        [SwaggerResponse(200, Type = typeof(GroupViewModel))]
        [SwaggerResponse(404, Type = typeof(Error))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("group/{groupCode}")]
        public IActionResult GetCardsByGroup(string groupCode)
        {
            try
            {
                var cards = SqlHelper.GetTechCardsByGroup(groupCode);
                return Ok(cards);
            }
            catch (Exception e)
            {
                if (e.Message == "404")
                    return NotFound(new Error("Не найдена группа", "badvalue"));

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить типы технических карт")]
        [SwaggerResponse(200, Type = typeof(List<CardTypeViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpGet("types")]
        public IActionResult GetCardTypes()
        {
            try
            {
                return Ok(SqlHelper.GetTechCardTypes());
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Создать новую техническую карту")]
        [SwaggerResponse(201)]
        [SwaggerResponse(400, Type = typeof(Error), Description = "Некорректные входные параметры")]
        [SwaggerResponse(409, Type = typeof(Error), Description = "Карта с таким номером уже существует")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpPost]
        public IActionResult PostTechCard(TechCardModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Model not valid: " + JsonConvert.SerializeObject(model) + Environment.NewLine);
                    return BadRequest(new Error("Некорректно заданы значения", "badvalue"));
                }

                    if (SqlHelper.IsCardExists(model.cardNum))
                    return UpdateTechCardGroups(model.cardNum, model.groupCode);

                SqlHelper.WriteTechCard(model);

                return StatusCode(201);
            }
            catch(Exception e)
            {
                if(e.Message == "command")
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error("Произошла ошибка базы данных", "db"));
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Добавить группы в техническую карту")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Карта с таким номером или группа не существует")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpPatch("{cardNum}/group")]
        public IActionResult UpdateTechCardGroups(string cardNum, string groupCode)
        {
            try
            {
                if (!SqlHelper.IsCardExists(cardNum))
                    return NotFound(new Error("Карта с таким номером не найдена", "badvalue"));

                if (!SqlHelper.IsGroupExists(groupCode))
                    return NotFound(new Error("Группа не найдена", "badvalue"));

                SqlHelper.AddTechCardGroup(cardNum, groupCode);

                return Ok();
            }
            catch(Exception e)
            {
                if (e.Message == "constraint")
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return Conflict(new Error($"Карта {cardNum} уже находится в группе {groupCode}", "badvalue"));
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Удалить техническую карту")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Карта с таким номером не существует")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [HttpDelete("{cardNum}")]
        public IActionResult DeleteTechCard(string cardNum)
        {
            try
            {
                if (!SqlHelper.IsCardExists(cardNum))
                    return NotFound(new Error("Карта с таким номером не найдена", "badvalue"));

                SqlHelper.DeleteCard(cardNum);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
