﻿using AuthenticationService.Models;
using AuthenticationService.Models.UserAuthenticationDb;
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

        public AccountsController(UserAuthenticationDbContext context)
        {
            _context = context;
        }

        private ClaimsIdentity GetIdentity(LoginModel login)
        {
            User user = _context.Users.FirstOrDefault(u => u.Login == login.Login && u.Password == login.Password);
            if (user != null)
            {
                var claims = new List<Claim>();
                Claim c1 = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login);
                claims.Add(c1);

                Claim c = new Claim(ClaimsIdentity.DefaultRoleClaimType, _context.Roles.Find(user.Idrole).Code);
                claims.Add(c);

                List<UserWash> washes = _context.UserWashes.Where(uw => uw.Iduser == user.Iduser).ToList();
                foreach (UserWash w in washes)
                {
                    claims.Add(new Claim("Wash", w.WashCode));
                }

                claims.Add(new Claim("UserName", user.Name));

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        private IActionResult Token(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel() { ErrorType = "badvalue", Alert = "Некорректные входные параметры", ErrorCode = "Ошибка валидации", ErrorMessage = "Проверьте правильность введенных данных и попробуйте снова" });

            var identity = GetIdentity(login);
            if (identity == null)
            {
                return BadRequest(new ErrorModel() { ErrorType = "login", Alert = "Неверный логин или пароль", ErrorCode = "Ошибка введённых данных", ErrorMessage = "Проверьте правильность введённых логина и пароля" });
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
        public IActionResult Login(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorModel() { ErrorType = "badvalue", Alert = "Некорректные входные параметры", ErrorCode = "Ошибка валидации", ErrorMessage = "Проверьте правильность введенных данных и попробуйте снова" });
            return Token(login);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить всех пользователей")]
        [SwaggerResponse(200, Type = typeof(List<AccountViewModel>))]
        [SwaggerResponse(500, Type = typeof(ErrorModel))]
        #endregion
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<AccountViewModel> result = new List<AccountViewModel>();

                List<User> users = _context.Users.ToList();
                foreach (User u in users)
                {
                    List<string> washes = new List<string>();

                    List<UserWash> washIDs = _context.UserWashes.Where(uw => uw.Iduser == u.Iduser).ToList();
                    foreach (UserWash w in washIDs)
                        washes.Add(w.WashCode);

                    result.Add(new AccountViewModel
                    {
                        Login = u.Login,
                        Name = u.Name,
                        Email = u.Email,
                        Phone = u.Phone,
                        Role = _context.Roles.Find(u.Idrole).Code,
                        Washes = washes
                    });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                //_logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new ErrorModel() { ErrorType = "unexpected", Alert = "Что-то пошло не так в ходе работы сервера", ErrorCode = "Ошибка при обращении к серверу", ErrorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }
    }
}