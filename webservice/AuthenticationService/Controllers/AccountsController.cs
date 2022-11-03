using AuthenticationService.Models;
using AuthenticationService.Models.DTOs;
using AuthenticationService.Models.UserAuthenticationDb;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserAuthenticationDbContext _context;
        private readonly AccountsService _accountsService;

        public AccountsController(UserAuthenticationDbContext context, AccountsService accountsService)
        {
            _context = context;
            _accountsService = accountsService;
        }

        private async Task<ClaimsIdentity> GetIdentityAsync(LoginModel login)
        {
            AccountInfoDto user = await _accountsService.GetAsync(login.Login);
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

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Неверный логин или пароль", "Проверьте правильность введённых данных и повторите снова");
        }

        private async Task<IActionResult> TokenAsync(LoginModel login)
        {
            var identity = await GetIdentityAsync(login);
            if (identity == null)
            {
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.BadRequest, "Неверный логин или пароль", "Проверьте правильность введённых данных и повторите снова");
            }

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
                Role = identity.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).FirstOrDefault().Value,
                Name = identity.Claims.Where(c => c.Type == "UserName").FirstOrDefault().Value
            };

            return Ok(response);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Залогиниться")]
        [SwaggerResponse(200, Type = typeof(Token))]
        [SwaggerResponse(400, Type = typeof(ErrorModel))]
        #endregion
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            return await TokenAsync(login);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить всех пользователей")]
        [SwaggerResponse(200, Type = typeof(List<AccountInfoDto>))]
        [SwaggerResponse(500, Type = typeof(ErrorModel))]
        #endregion
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<AccountInfoDto> accounts = await _accountsService.GetAsync();

            return Ok(accounts);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Зарегистрировать нового")]
        [SwaggerResponse(200, Type = typeof(AccountInfoDto))]
        [SwaggerResponse(409, Type = typeof(ErrorModel))]
        #endregion
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] NewAccountInfoDto account)
        {
            if(await _accountsService.IsNameExistAsync(account.Login))
            {
                return Conflict(new ErrorModel() { ErrorType = "badvalue", Alert = "Некорректные входные параметры", ErrorCode = "Такой логин уже существует", ErrorMessage = "Попробуйте ввести другой логин" });
            }

            await _accountsService.CreateAsync(account);

            return Ok();
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Удалить по логину")]
        [SwaggerResponse(200)]
        #endregion
        [HttpDelete("{login}")]
        public async Task<IActionResult> Delete(string login)
        {
            await _accountsService.DeleteAsync(login);
            return Ok();
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить по логину")]
        [SwaggerResponse(200, Type = typeof(AccountInfoDto))]
        #endregion
        [HttpGet("{login}")]
        public async Task<IActionResult> GetByLogin([FromRoute]string login)
        {
            AccountInfoDto account = await _accountsService.GetAsync(login);

            return Ok(account);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Проверить логин")]
        [SwaggerResponse(200, Type = typeof(bool))]
        #endregion
        [HttpGet("check-login")]
        public async Task<IActionResult> CheckLogin([FromQuery] string login, int? id = null)
        {
            return Ok(await _accountsService.IsNameExistAsync(login, id));
        }

        [Authorize]
        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims()
        {
            return Ok(User.Claims.Select(o => new { o.Type, o.Value }));
        }
    }
}
