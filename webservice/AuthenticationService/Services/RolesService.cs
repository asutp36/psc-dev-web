using AuthenticationService.Models;
using AuthenticationService.Models.UserAuthenticationDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationService.Services
{
    public class RolesService
    {
        private readonly ILogger<RolesService> _logger;
        private readonly UserAuthenticationDbContext _model;

        public RolesService(ILogger<RolesService> logger, UserAuthenticationDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        public async Task<IEnumerable<RoleDto>> GatAll()
        {
            var roles = await _model.Roles.Select(o => new RoleDto
            {
                Code = o.Code,
                Name = o.Name
            }).ToListAsync();

            return roles;
        }
    }
}
