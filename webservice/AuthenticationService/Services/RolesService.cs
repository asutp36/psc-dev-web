using AuthenticationService.Models;
using AuthenticationService.Models.DTOs;
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
        /// <returns>Информацию о роли</returns>
        public async Task<IEnumerable<RoleInfoDto>> GetAsync()
        {
            var roles = await _model.Roles.Select(o => new RoleInfoDto
            {
                Code = o.Code,
                Name = o.Name
            }).ToListAsync();

            return roles;
        }

        /// <summary>
        /// Получить информацио о роли по её коду
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <returns>Информация о роли или null</returns>
        public async Task<RoleInfoDto> GetAsync(string code)
        {
            return await _model.Roles.Where(o => o.Code == code)
                .Select(o => new RoleInfoDto 
                { 
                    Code = o.Code, 
                    Name = o.Name 
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Получить информацию о роли по её id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>информац о роли или null</returns>
        public async Task<RoleInfoDto> GetAsync(int id)
        {
            return await _model.Roles.Where(o => o.Idrole == id)
                .Select(o => new RoleInfoDto
                {
                    Code = o.Code,
                    Name = o.Name
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Получить информацию о роли по её id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>информац о роли или null</returns>
        public RoleInfoDto Get(int id)
        {
            return _model.Roles.Where(o => o.Idrole == id)
                .Select(o => new RoleInfoDto
                {
                    Code = o.Code,
                    Name = o.Name
                })
                .FirstOrDefault();
        }
    }
}
