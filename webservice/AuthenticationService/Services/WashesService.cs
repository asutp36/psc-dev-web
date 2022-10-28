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
    public class WashesService
    {
        private readonly ILogger<WashesService> _logger;
        private readonly UserAuthenticationDbContext _model;

        public WashesService(ILogger<WashesService> logger, UserAuthenticationDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        /// <summary>
        /// Получить все мойки списком
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<WashDTO>> GetAsync()
        {
            var result = await _model.Washes.Include(o => o.IdwashTypeNavigation)
                .Select(o => new WashDTO
                {
                    IdWash = o.Idwash,
                    Code = o.Code,
                    Name = o.Name,
                    Type = new WashTypeDTO() { IdWashType = o.IdwashType, Code = o.IdwashTypeNavigation.Code, Name = o.IdwashTypeNavigation.Name }
                }).ToListAsync();

            return result;
        }

        /// <summary>
        /// Получить мойку по её коду
        /// </summary>
        /// <param name="code">Код мойки</param>
        /// <returns></returns>
        public async Task<WashDTO> GetByCodeAsync(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                _logger.LogError("Код мойки пустой");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Неверные входные параметры", "Не задан код мойки");
            }

            if (!await _model.Washes.AnyAsync(o => o.Code == code))
            {
                _logger.LogError($"Не удалось найти мойку по коду {code}");
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.NotFound, "Мойка не найдена", "Не удалось найти мойку по запрошенному коду");
            }

            var wash = await _model.Washes.Where(o => o.Code == code).Include(o => o.IdwashTypeNavigation)
                .Select(o => new WashDTO
                {
                    IdWash = o.Idwash,
                    Code = o.Code,
                    Name = o.Name,
                    Type = new WashTypeDTO() { IdWashType = o.IdwashType, Code = o.IdwashTypeNavigation.Code, Name = o.IdwashTypeNavigation.Name }
                })
                .FirstOrDefaultAsync();

            return wash;
        }
    }
}
