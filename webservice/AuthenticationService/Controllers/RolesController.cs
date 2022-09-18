using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<RoleDto> roles = await _rolesService.GatAll();

            return Ok(roles);
        }
    }
}
