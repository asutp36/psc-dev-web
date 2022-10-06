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
    public class AccountsService
    {
        private readonly ILogger<AccountsService> _logger;
        private readonly UserAuthenticationDbContext _model;
        private readonly RolesService _rolesService;

        public AccountsService(ILogger<AccountsService> logger, UserAuthenticationDbContext model, RolesService rolesService)
        {
            _logger = logger;
            _model = model;
            _rolesService = rolesService;
        }

        public async Task<IEnumerable<AccountInfoDto>> Get()
        {
            var accounts = await _model.Users.Include(o => o.IdroleNavigation)
                .GroupJoin(_model.UserWashes,
                           u => u.Iduser,
                           uw => uw.Iduser,
                           (u, washes) => new AccountInfoDto
                           {
                                Login = u.Login,
                                Name = u.Name,
                                Email = u.Email,
                                Phone = u.PhoneInt,
                                Role = new RoleInfoDto() { Code = u.IdroleNavigation.Code, Name = u.IdroleNavigation.Name},
                                Washes = washes.Select(e => e.WashCode)
                           }).ToListAsync();

            return accounts;
        }

        public async Task<AccountInfoDto> Get(int id) 
        {
            return await _model.Users.Where(o => o.Iduser == id).Select(o => new AccountInfoDto
            {
                id = o.Iduser,
                Login = o.Login,
                Email = o.Email,
                Name = o.Name,
                Phone = o.PhoneInt,
                Washes = o.UserWashes.Select(e => e.WashCode),
                Role = new RoleInfoDto() { Code = o.IdroleNavigation.Code, Name = o.IdroleNavigation.Name }
            }
            ).FirstOrDefaultAsync();
        }

        public async Task<AccountInfoDto> GetAsync(string login) 
        {
            return await _model.Users.Where(o => o.Login == login).Select(o => new AccountInfoDto
            {
                id = o.Iduser,
                Login = o.Login,
                Email = o.Email,
                Name = o.Name,
                Phone = o.PhoneInt,
                Washes = o.UserWashes.Select(e => e.WashCode),
                Role = _rolesService.Get(o.Idrole)
            }).FirstOrDefaultAsync();
        }

        public async Task Update() { }

        public async Task DeleteAsync(string login) 
        {
            User user = await _model.Users.Where(o => o.Login == login).FirstOrDefaultAsync();

            _model.Remove(user);
            await _model.SaveChangesAsync();
        }

        public async Task CreateAsync(NewAccountInfoDto account) 
        {
            var role = await _rolesService.GetAsync(account.Role.Code);

            User user = new User()
            {
                Login = account.Login,
                Name = account.Name,
                Password = account.Password,
                Email = account.Email,
                PhoneInt = account.Phone,
                Idrole = role.Id
            };

            try
            {
                await _model.Database.BeginTransactionAsync();
                await _model.Users.AddAsync(user);

                await _model.SaveChangesAsync();

                IEnumerable<UserWash> uw = account.Washes.Select(o => new UserWash { Iduser = user.Iduser, WashCode = o });
                await _model.UserWashes.AddRangeAsync(uw);
                await _model.SaveChangesAsync();

                await _model.Database.CommitTransactionAsync();
            }
            catch(Exception e)
            {
                await _model.Database.RollbackTransactionAsync();
            }
        }

        public async Task<bool> IsNameExistAsync(string login, int? currentId = null)
        {
            if (string.IsNullOrEmpty(login)) 
                return false;

            return await _model.Users.AnyAsync(e => e.Iduser != currentId && e.Name == login);
        }
    }
}