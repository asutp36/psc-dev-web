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
    public class AccountsService
    {
        private readonly ILogger<AccountsService> _logger;
        private readonly UserAuthenticationDbContext _model;

        public AccountsService(ILogger<AccountsService> logger, UserAuthenticationDbContext model)
        {
            _logger = logger;
            _model = model;
        }

        public async Task<IEnumerable<AccountViewModel>> Get()
        {
            var accounts = await _model.Users.Include(o => o.IdroleNavigation)
                .GroupJoin(_model.UserWashes,
                           u => u.Iduser,
                           uw => uw.Iduser,
                           (u, washes) => new AccountViewModel
                           {
                                Login = u.Login,
                                Name = u.Name,
                                Email = u.Email,
                                Phone = u.PhoneInt,
                                Role = u.IdroleNavigation.Code,
                                Washes = washes.Select(e => e.WashCode)
                           }).ToListAsync();

            return accounts;
        }

        public void Get(int id) { }

        public void Update() { }

        public void Delete() { }

        public void Create() { }
    }
}
