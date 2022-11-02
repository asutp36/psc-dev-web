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
        public async Task<IEnumerable<RoleDTO>> GetAsync()
        {
            var roles = await _model.Roles.Select(o => new RoleDTO
            {
                IdRole = o.Idrole,
                Code = o.Code,
                Name = o.Name
            }).ToListAsync();

            return roles;
        }

        public async Task<RoleDTO> GetAsync(string code, int id)
        {
            if(id > 0)
            {
                return await GetAsync(id);
            }

            if (!string.IsNullOrEmpty(code))
            {
                return await GetAsync(code);
            }

            throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Некорректные входные параметры", "Входные параметры заданы некорректно, проверьте правильность ввода");
        }

        /// <summary>
        /// Получить информацио о роли по её коду
        /// </summary>
        /// <param name="code">Код роли</param>
        /// <returns>Информация о роли или null</returns>
        public async Task<RoleDTO> GetAsync(string code)
        {
            return await _model.Roles.Where(o => o.Code == code)
                .Select(o => new RoleDTO 
                { 
                    IdRole = o.Idrole,
                    Code = o.Code, 
                    Name = o.Name 
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Получить информацию о роли по её id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Информация о роли или null</returns>
        public async Task<RoleDTO> GetAsync(int id)
        {
            if(! await _model.Roles.AnyAsync(o => o.Idrole == id))
            {
                _logger.LogError($"Не найдена мойка с id={id}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Роль не найдена", "Не удалось найти роль по запрошенному id");
            }

            return await _model.Roles.Where(o => o.Idrole == id)
                .Select(o => new RoleDTO
                {
                    IdRole = o.Idrole,
                    Code = o.Code,
                    Name = o.Name
                })
                .FirstOrDefaultAsync();
        }
    }
}
