using AuthenticationService.Models;
using AuthenticationService.Models.DTOs;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly RolesService _rolesService;

        public RolesController(RolesService rolesService)
        {
            _rolesService = rolesService;
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить роль по коду или id")]
        [SwaggerResponse(200, Type = typeof(RoleDTO))]
        [SwaggerResponse(400, Description = "Некорректные входные параметры")]
        [SwaggerResponse(404, Description = "Роль не найдена")]
        [SwaggerResponse(500, Description = "Что-то пошло не так")]
        #endregion
        [HttpGet]
        public async Task<IActionResult> Get(string code = null, int id = 0)
        {
            if (code == null && id == 0)
            {
                return Ok(await _rolesService.GetAsync());
            }

            return Ok(await _rolesService.GetAsync(code, id));
        }

        [HttpPost]
        public async Task<IActionResult> PostRole(RoleDTO role)
        {
            var r = await _rolesService.CreateRoleAsync(role);

            return Created("", r);
        }

        [HttpPut]
        public async Task<IActionResult> PutRole(RoleDTO role)
        {
            var r = await _rolesService.UpdateRoleAsync(role);

            return Ok(r);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string code = null, int id = 0)
        {
            if(id != 0)
            {
                await _rolesService.DeleteRoleAsync(id);
                return Ok();
            }

            if (code != null)
            {
                await _rolesService.DeleteRoleAsync(code);
                return Ok();
            }

            return BadRequest(new { Message = "Не удалось удалить роль", Description = "Для удаления роли необходимо передать либо её id, либо её код" });
        }

        [HttpGet("DeleteEffect")]
        public async Task<IActionResult> GetDeleteEffect(string code = null, int id = 0)
        {
            return Ok(await _rolesService.GetAssociatedUsers(code, id));
        }

        [HttpGet("isCodeForbidden")]
        public async Task<IActionResult> CheckCode([FromQuery] string code, int? id = null)
        {
            return Ok(await _rolesService.IsCodeExistAsync(code, id));
        }
    }
}
