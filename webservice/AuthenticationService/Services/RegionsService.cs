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
    public class RegionsService
    {
        private readonly ILogger<RegionsService> _logger;
        private readonly UserAuthenticationDbContext _model;

        public RegionsService(ILogger<RegionsService> logger, UserAuthenticationDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        /// <summary>
        /// Получить все регионы с мойками
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RegionDTO>> GetAsync()
        {
            var result = await _model.Regions.Include(o => o.Washes).ThenInclude(o => o.IdwashTypeNavigation)
                .Select(o => new RegionDTO
                {
                    IdRegion = o.Idregion,
                    Code = o.Code,
                    Name = o.Name,
                    Washes = o.Washes.Select(e => new WashDTO
                    {
                        IdWash = e.Idwash,
                        Code = e.Code,
                        Name = e.Name,
                        Type = new WashTypeDTO() { IdWashType = e.IdwashType, Code = e.IdwashTypeNavigation.Code, Name = e.IdwashTypeNavigation.Name }
                    })
                }).ToListAsync();
            return result;
        }
    }
}
