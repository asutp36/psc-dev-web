using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Auth;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

            var response = new
            {
                access_token = encodedJwt,
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

                List<UsersAvailableWash> washes = _model.UsersAvailableWash.Where(uaw => uaw.Iduser == user.Iduser).ToList();

                foreach (UsersAvailableWash uaw in washes)
                {
                    Claim c2 = new Claim(ClaimsIdentity.DefaultRoleClaimType, _model.Wash.Find(uaw.Idwash).Name);
                    claims.Add(c2);
                }

                ClaimsIdentity claimsIdentity =
                new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                    ClaimsIdentity.DefaultRoleClaimType);
                return claimsIdentity;
            }

            // если пользователя не найдено
            return null;
        }

        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error("Модель не прошла валидацию", "model"));
            return Token(login);
        }


        [Authorize]
        [HttpGet]
        public IActionResult GetWashes()
        {
            List<Claim> claims = User.Claims.ToList();
            List<string> result = new List<string>();

            foreach(Claim c in claims)
            {
                if(c.Type == ClaimsIdentity.DefaultRoleClaimType)
                    result.Add(c.Value);
            }
            return Ok(result);
        }
    }
}