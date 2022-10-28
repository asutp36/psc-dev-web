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
    }
}
