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
                Id = o.Idrole,
                Code = o.Code,
                Name = o.Name,
                IsAdmin = o.IsAdmin,
                Eco = (AccessLevel)o.Eco,
                GateWash = (AccessLevel)o.GateWash,
                RefillGateWash = o.RefillGateWash
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
                    Id = o.Idrole,
                    Code = o.Code, 
                    Name = o.Name,
                    IsAdmin = o.IsAdmin,
                    Eco = (AccessLevel)o.Eco,
                    GateWash = (AccessLevel)o.GateWash,
                    RefillGateWash = o.RefillGateWash
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
                _logger.LogError($"Не найдена роль с id={id}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Роль не найдена", "Не удалось найти роль по запрошенному id");
            }

            return await _model.Roles.Where(o => o.Idrole == id)
                .Select(o => new RoleDTO
                {
                    Id = o.Idrole,
                    Code = o.Code,
                    Name = o.Name,
                    Eco = (AccessLevel)o.Eco,
                    GateWash = (AccessLevel)o.GateWash,
                    RefillGateWash = o.RefillGateWash
                })
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Создать новую роль
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<int> CreateRoleAsync(RoleDTO role)
        {
            try
            {
                if (string.IsNullOrEmpty(role.Code))
                {
                    _logger.LogError($"Не задан код роли");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Невозможно создать роль", "Не задан код роли");
                }

                if(await _model.Roles.AnyAsync(o => o.Code == role.Code))
                {
                    _logger.LogError($"Роль с кодом {role.Code} уже существует");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.Conflict, "Невозможно создать роль", $"Роль с кодом {role.Code} уже существует");
                }

                if (string.IsNullOrEmpty(role.Name))
                {
                    _logger.LogError("Не задано имя роли");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Невозможно создать роль", "Не задано имя роли");
                }

                Role r = new Role()
                {
                    Code = role.Code,
                    Name = role.Name,
                    IsAdmin = role.IsAdmin,
                    Eco = (int)role.Eco,
                    GateWash = (int)role.GateWash,
                    RefillGateWash = role.RefillGateWash
                };

                await _model.Roles.AddAsync(r);
                await _model.SaveChangesAsync();

                return r.Idrole;
            }
            catch(DbUpdateException e)
            {
                _logger.LogError($"Ошибка при добавлении роли в базу: {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.InternalServerError, "Не удалось создать роль", 
                    "В данный момент не удалось записать роль в базу, попробуйте позже и сообщите об этой ошибке специалистам");
            }
        }

        /// <summary>
        /// Изменить роль
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public async Task<int> UpdateRoleAsync(RoleDTO role)
        {
            try
            {
                if (string.IsNullOrEmpty(role.Code))
                {
                    _logger.LogError("Не задан код изменяемой роли");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не удалось изменить роль", "Не задан код роли");
                }

                if (string.IsNullOrEmpty(role.Name))
                {
                    _logger.LogError("Не задано имя изменяемой роли");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не удалось изменить роль", "Не задано имя роли");
                }

                Role r = await _model.Roles.FindAsync(role.Id);

                if (r == null)
                {
                    _logger.LogError($"Роль с id={role.Id} не найдена");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Невозможно изменить роль", "Роль, которую хотите изменить, не найдена");
                }

                r.Code = role.Code;
                r.Name = role.Name;
                r.IsAdmin = role.IsAdmin;
                r.Eco = (int)role.Eco;
                r.GateWash = (int)role.GateWash;
                r.RefillGateWash = role.RefillGateWash;

                _model.Roles.Update(r);
                await _model.SaveChangesAsync();

                return r.Idrole;
            }
            catch (DbUpdateException e)
            {
                _logger.LogError($"Ошибка при изменении роли в базе: {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.InternalServerError, "Не удалось изменить роль",
                    "В данный момент не удалось изменить роль, попробуйте позже и сообщите об этой ошибке специалистам");
            }
        }

        public async Task DeleteRoleAsync(int id)
        {
            try
            {
                if (id < 0)
                {
                    _logger.LogError($"Не удалось удалить роль, id<0");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не удалось удалить роль", "Некорректное значение id роли");
                }

                Role r = await _model.Roles.FindAsync(id);
                if(r == null)
                {
                    _logger.LogError($"Роль с id={id} не найдена");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Не удалось удалить роль", "Роль с таким id не найдена");
                }

                _model.Roles.Remove(r);
                await _model.SaveChangesAsync();
            }
            catch(DbUpdateException e)
            {
                _logger.LogError($"Ошибка при удалении роли в базе: {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.InternalServerError, "Не удалось удалить роль",
                    "В данный момент не удалось удалить роль, попробуйте позже и сообщите об этой ошибке специалистам");
            }
        }

        public async Task DeleteRoleAsync(string code)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    _logger.LogError($"Не удалось удалить роль, code не задан");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Не удалось удалить роль", "Некорректное значение кода роли");
                }

                Role r = await _model.Roles.Where(o => o.Code == code).FirstOrDefaultAsync();
                if (r == null)
                {
                    _logger.LogError($"Роль {code} не найдена");
                    throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Не удалось удалить роль", "Роль с таким кодом не найдена");
                }

                _model.Roles.Remove(r);
                await _model.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError($"Ошибка при удалении роли в базе: {e.Message}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.InternalServerError, "Не удалось удалить роль",
                    "В данный момент не удалось удалить роль, попробуйте позже и сообщите об этой ошибке специалистам");
            }
        }

        public async Task<IEnumerable<string>> GetAssociatedUsers(string code = null, int id = 0)
        {
            var result = await _model.Roles.Where(o => o.Idrole == id || o.Code == code).Include(o => o.Users).Select(o => o.Users.Select(u => u.Login)).FirstOrDefaultAsync();
            return result;
        }

        public async Task<bool> IsCodeExistAsync(string code, int? currentId = null)
        {
            if (string.IsNullOrEmpty(code))
                return false;

            return await _model.Roles.AnyAsync(e => e.Idrole != currentId && e.Code == code);
        }
    }
}
