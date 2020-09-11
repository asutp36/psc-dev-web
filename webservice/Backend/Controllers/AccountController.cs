using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Auth;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        private IActionResult Token(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error("Модель не прошла валидацию", "model"));

            var identity = GetIdentity(login);
            if (identity == null)
            {
                return BadRequest(new Error("Неверный логин или пароль", "login"));
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
                accessToken = encodedJwt,
                login = identity.Name,
                role = identity.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).FirstOrDefault().Value
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(LoginModel login)
        {
            Users user = _model.Users.FirstOrDefault(u => u.Login.Equals(login.login) && u.Password.Equals(login.password));
            if (user != null)
            {
                var claims = new List<Claim>();
                Claim c1 = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login);
                claims.Add(c1);

                List<UserRole> roles = _model.UserRole.Where(ur => ur.Iduser.Equals(_model.Users.Where(u => u.Login.Equals(user.Login)).FirstOrDefault().Iduser)).ToList();

                foreach(UserRole ur in roles)
                {
                    Claim c = new Claim(ClaimsIdentity.DefaultRoleClaimType, _model.Roles.Find(ur.Idrole).Code);
                    claims.Add(c);
                }

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [SwaggerResponse(200, Type = typeof(Token))]
        [SwaggerResponse(400, Type = typeof(Error))]
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error("Модель не прошла валидацию", "model"));
            return Token(login);
        }

        [SwaggerResponse(200, Type = typeof(DashboardFilters))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                UserInfo uInfo = new UserInfo(User.Claims.ToList());

                DashboardFilters filters = new DashboardFilters()
                {
                    washes = uInfo.GetWashes(),
                    posts = uInfo.GetPosts()
                };

                return Ok(filters);
            }
            catch(Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}