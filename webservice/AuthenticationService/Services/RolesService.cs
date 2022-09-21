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

        /// <summary>
        /// Получить все роли
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RoleDto>> GetAsync()
        {
            var roles = await _model.Roles.Select(o => new RoleDto
            {
                Id = o.Idrole,
                Code = o.Code,
                Name = o.Name
            }).ToListAsync();

            return roles;
        }

        /// <summary>
        /// Получить роль по её коду
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <returns>Роль ил null</returns>
        public async Task<RoleDto> GetAsync(string code)
        {
            return await _model.Roles.Where(o => o.Code == code)
                .Select(o => new RoleDto 
                { 
                    Id = o.Idrole, 
                    Code = o.Code, 
                    Name = o.Name 
                })
                .FirstOrDefaultAsync();
        }
    }
}
