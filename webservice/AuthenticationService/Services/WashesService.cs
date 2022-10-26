using AuthenticationService.Models.UserAuthenticationDb;
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
    }
}
