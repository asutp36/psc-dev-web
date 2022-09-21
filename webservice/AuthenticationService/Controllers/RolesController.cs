using AuthenticationService.Models;
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
        [SwaggerOperation(Summary = "Получить все роли")]
        [SwaggerResponse(200, Type = typeof(List<RoleDto>))]
        #endregion
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<RoleDto> roles = await _rolesService.GetAsync();

            return Ok(roles);
        }
    }
}
