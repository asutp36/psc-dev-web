using AuthenticationService.Models;
using AuthenticationService.Models.DTOs;
using AuthenticationService.Models.UserAuthenticationDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.Services
{
    public class AccountsService
    {
        private readonly ILogger<AccountsService> _logger;
        private readonly UserAuthenticationDbContext _model;
        private readonly RolesService _rolesService;
        private readonly WashesService _washesService;

        public AccountsService(ILogger<AccountsService> logger, UserAuthenticationDbContext model, RolesService rolesService, WashesService washesService)
        {
            _logger = logger;
            _model = model;
            _rolesService = rolesService;
            _washesService = washesService;
        }

        /// <summary>
        /// Получить всех пользователей
        /// </summary>
        /// <returns>Список пользователей</returns>
        public async Task<IEnumerable<AccountInfoDto>> GetAsync()
        {
            try
            {
                var accounts = await _model.Users.Include(o => o.UserWashes).ThenInclude(o => o.IdwashNavigation).ThenInclude(o => o.IdwashTypeNavigation)
                                    .Include(o => o.IdroleNavigation)
                                    .Select(o => new AccountInfoDto
                                    {
                                        id = o.Iduser,
                                        Login = o.Login,
                                        Name = o.Name,
                                        Phone = o.PhoneInt - 70000000000,
                                        Email = o.Email,
                                        Role = new RoleDTO(o.Idrole, o.IdroleNavigation.Code, o.IdroleNavigation.Name),
                                        Washes = o.UserWashes.Select(w => new WashInfo
                                        {
                                            Code = w.IdwashNavigation.Code,
                                            Name = w.IdwashNavigation.Name,
                                            TypeCode = w.IdwashNavigation.IdwashTypeNavigation.Code
                                        })
                                    }).ToListAsync();

                return accounts;
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace);
                return null;
            }
        }

        public async Task<AccountInfoDto> GetAsync(int id) 
        {
            return await _model.Users.Where(o => o.Iduser == id).Select(o => new AccountInfoDto
            {
                id = o.Iduser,
                Login = o.Login,
                Email = o.Email,
                Name = o.Name,
                Phone = o.PhoneInt - 70000000000,
                //Washes = o.UserWashes.Select(e => e.WashCode),
                Role = new RoleDTO() { Code = o.IdroleNavigation.Code, Name = o.IdroleNavigation.Name }
            }
            ).FirstOrDefaultAsync();
        }

        public async Task<AccountInfoDto> GetAsync(string login)
        {
            var user = await _model.Users.Include(o => o.UserWashes).Where(o => o.Login == login)
                .Select(o => new AccountInfoDto
                {
                    id = o.Iduser,
                    Login = o.Login,
                    Email = o.Email,
                    Name = o.Name,
                    Phone = o.PhoneInt,
                    Washes = o.UserWashes.Select(w => new WashInfo { Code = w.IdwashNavigation.Code, Name = w.IdwashNavigation.Name, TypeCode = w.IdwashNavigation.IdwashTypeNavigation.Code }),
                    Role = new RoleDTO {
                        Id = o.Idrole,
                        Code = o.IdroleNavigation.Code,
                        Name = o.IdroleNavigation.Name,
                        IsAdmin = o.IdroleNavigation.IsAdmin,
                        Eco = (AccessLevel)o.IdroleNavigation.Eco,
                        GateWash = (AccessLevel)o.IdroleNavigation.GateWash,
                        RefillGateWash = o.IdroleNavigation.RefillGateWash
                    }
                }).FirstOrDefaultAsync();

            if (user.Washes.Count() == 0)
            {
                _logger.LogError($"Не найдено ни одной мойки у пользователя {user.Login}");
                throw new CustomStatusCodeException(HttpStatusCode.PreconditionFailed, "Не найдено моек", $"У пользователя {user.Login} не найдено ни одной мойки");
            }

            return user;
        }

        public async Task<AccountInfoDto> UpdateAsync(UpdateAccountModel account) 
        {
            try
            {
                IEnumerable<WashDTO> washes = _washesService.GetRange(account.Washes);

                if (string.IsNullOrEmpty(account.Login))
                {
                    _logger.LogError("Не задан логин пользователя");
                    throw new CustomStatusCodeException(HttpStatusCode.BadRequest, "Не задан логин", "Логин пользователя не задан. Проверьте введённые данные и попробуйте снова");
                }

                if (string.IsNullOrEmpty(account.Name))
                {
                    _logger.LogError("Не задано имя пользователя");
                    throw new CustomStatusCodeException(HttpStatusCode.BadRequest, "Не задано имя", "Имя пользователя не задано. Проверьте введённые данные и попробуйте снова");
                }

                User user = _model.Users.FirstOrDefault(u => u.Iduser == account.id);

                if (user == null)
                {
                    _logger.LogError($"ПОльзователь с id={account.id} не найден");
                    throw new CustomStatusCodeException(HttpStatusCode.NotFound, "Пользователь не найден", "Пользователь по запрошенному id н найден");
                }

                //var role = await _rolesService.GetAsync(account.IdRole);
                var role = await _model.Roles.FindAsync(account.IdRole);

                var userWashesToRemove = _model.UserWashes.Where(uw => uw.Iduser == user.Iduser);
                _model.UserWashes.RemoveRange(userWashesToRemove);

                user.Login = account.Login;
                user.Name = account.Name;
                user.Email = account.Email;
                user.Phone = string.Format("{0:+#-###-###-##-##}", account.Phone + 70000000000);
                user.PhoneInt = account.Phone + 70000000000;
                //user.Idrole = role.Id;
                user.Idrole = role.Idrole;
                user.UserWashes = washes.Select(o => new UserWash { Iduser = user.Iduser, Idwash = o.IdWash }).ToList();

                //_model.Users.Update(user);
                await _model.SaveChangesAsync();

                return new AccountInfoDto
                {
                    id = user.Iduser,
                    Login = user.Login,
                    Name = user.Name,
                    Phone = user.PhoneInt - 70000000000,
                    Email = user.Email,
                    Role = await _rolesService.GetAsync(user.Idrole),
                    Washes = washes.Select(o => new WashInfo { Code = o.Code, Name = o.Name, TypeCode = o.Type.Code })
                };
            }
            catch(Exception e)
            {
                _logger.LogError($"Произошла ошибка: {e.GetType()}: {e.Message}");
                throw new CustomStatusCodeException(HttpStatusCode.InternalServerError, "Что-то случилось", "");
            }
        }

        public async Task DeleteAsync(string login) 
        {
            User user = await _model.Users.Where(o => o.Login == login).FirstOrDefaultAsync();

            _model.Remove(user);
            await _model.SaveChangesAsync();
        }

        public async Task<AccountInfoDto> CreateAsync(NewAccountInfoDto account) 
        {
            var role = await _rolesService.GetAsync(account.IdRole);

            List<WashDTO> washes = new List<WashDTO>();
            foreach(string w in account.Washes)
            {
                washes.Add(await _washesService.GetAsync(w));
            }

            User user = new User()
            {
                Login = account.Login,
                Name = account.Name,
                Password = account.Password,
                Email = account.Email,
                PhoneInt = 70000000000 + account.Phone,
                Idrole = role.Id
            };

            var transaction = await _model.Database.BeginTransactionAsync();

            try
            {
                await _model.Users.AddAsync(user);

                foreach (WashDTO w in washes)
                {
                    _model.UserWashes.Add(new UserWash { IduserNavigation = user, Idwash = w.IdWash });
                }

                await _model.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch(Exception e)
            {
                await transaction.RollbackAsync();
                throw e;
            }

            return new AccountInfoDto
            {
                id = user.Iduser,
                Login = user.Login,
                Name = user.Name,
                Phone = user.PhoneInt - 70000000000,
                Email = user.Email,
                Role = await _rolesService.GetAsync(user.Idrole),
                Washes = washes.Select(o => new WashInfo { Code = o.Code, Name = o.Name, TypeCode = o.Type.Code })
            };
        }

        public async Task<bool> IsLoginExistAsync(string login, int? currentId = null)
        {
            if (string.IsNullOrEmpty(login)) 
                return false;

            return await _model.Users.AnyAsync(e => e.Iduser != currentId && e.Login == login);
        }

        public async Task<Token> LoginAsync(LoginModel login)
        {
            if (string.IsNullOrEmpty(login.Login))
            {
                _logger.LogError("Логин пользователя пустой");
                throw new CustomStatusCodeException(HttpStatusCode.BadRequest, "Не удалось войти в систему", "Логин не задан");
            }

            if (string.IsNullOrEmpty(login.Password))
            {
                _logger.LogError("Пароль пустой");
                throw new CustomStatusCodeException(HttpStatusCode.BadRequest, "Не удалось войти в систему", "Пароль не задан");
            }

            if (!(await _model.Users.AnyAsync(o => o.Login == login.Login && o.Password == login.Password)))
            {
                _logger.LogInformation($"Не найдена пара логин-пароль: {login.Login} - {login.Password}");
                throw new CustomStatusCodeException(HttpStatusCode.NotFound, "Неверный логин или пароль", "Проверьте правильность введённых данных и попробуйте снова");
            }

            ClaimsIdentity identity = await GenerateIdenityAsync(login.Login);

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Token response = new Token()
            {
                AccessToken = encodedJwt,
                Login = identity.Name,
                Role = await _rolesService.GetAsync(identity.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType).Select(o => o.Value).FirstOrDefault()),
                Name = identity.Claims.Where(c => c.Type == "UserName").Select(o => o.Value).FirstOrDefault()
            };

            return response;
        }

        public async Task<Token> LoginAsync(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                _logger.LogError("Логин пользователя пустой");
                throw new CustomStatusCodeException(HttpStatusCode.BadRequest, "Не удалось войти в систему", "Логин не задан");
            }

            if (!(await _model.Users.AnyAsync(o => o.Login == login)))
            {
                _logger.LogInformation($"Не найден пользователь: {login}");
                throw new CustomStatusCodeException(HttpStatusCode.NotFound, "Не найден пользователь", $"Пользователь с логином {login} не найден");
            }

            ClaimsIdentity identity = await GenerateIdenityAsync(login);

            var now = DateTime.UtcNow;
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            Token response = new Token()
            {
                AccessToken = encodedJwt,
                Login = identity.Name,
                Role = await _rolesService.GetAsync(identity.Claims.Where(o => o.Type == ClaimsIdentity.DefaultRoleClaimType).Select(o => o.Value).FirstOrDefault()),
                Name = identity.Claims.Where(c => c.Type == "UserName").Select(o => o.Value).FirstOrDefault()
            };

            return response;
        }

        private async Task<ClaimsIdentity> GenerateIdenityAsync(string login)
        {
            AccountInfoDto user = await GetAsync(login);
            if (user != null)
            {
                var claims = new List<Claim>();
                Claim c1 = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login);
                claims.Add(c1);

                Claim c = new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.Code);
                claims.Add(c);

                foreach (WashInfo w in user.Washes)
                {
                    claims.Add(new Claim(w.TypeCode, w.Code));
                }

                claims.Add(new Claim("UserName", user.Name));

                claims.Add(new Claim("IsAdmin", user.Role.IsAdmin.ToString()));
                claims.Add(new Claim("GateWash", user.Role.GateWash.ToString()));
                claims.Add(new Claim("Eco", user.Role.Eco.ToString()));
                claims.Add(new Claim("RefillGateWash", user.Role.RefillGateWash.ToString()));

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);

                return claimsIdentity;
            }
            
            _logger.LogError($"user == null");
            throw new CustomStatusCodeException(HttpStatusCode.NotFound, "Неверный логин или пароль", "Проверьте правильность введённых данных и попробуйте снова");
        }
    }
}