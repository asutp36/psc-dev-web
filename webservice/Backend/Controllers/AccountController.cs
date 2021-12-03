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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        private IActionResult Token(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error() { errorType = "badvalue", alert = "Некорректные входные параметры", errorCode = "Ошибка валидации", errorMessage = "Проверьте правильность введенных данных и попробуйте снова" });

            var identity = GetIdentity(login);
            if (identity == null)
            {
                return BadRequest(new Error() { errorType = "login", alert = "Неверный логин или пароль", errorCode = "Ошибка введённых данных", errorMessage = "Проверьте правильность введённых логина и пароля" });
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
                role = identity.Claims.Where(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).FirstOrDefault().Value,
                name = identity.Claims.Where(c => c.Type == "UserName").FirstOrDefault().Value
            };

            return Ok(response);
        }

        private ClaimsIdentity GetIdentity(LoginModel login)
        {
            Users user = _model.Users.FirstOrDefault(u => EF.Functions.Like(u.Login, login.login) && EF.Functions.Like(u.Password, login.password));
            if (user != null)
            {
                var claims = new List<Claim>();
                Claim c1 = new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login);
                claims.Add(c1);

                Claim c = new Claim(ClaimsIdentity.DefaultRoleClaimType, _model.Roles.Find(user.Idrole).Code);
                claims.Add(c);

                List<UserWash> washes = _model.UserWash.Where(uw => uw.Iduser == user.Iduser).ToList();
                foreach (UserWash w in washes)
                {
                    string washCode = _model.Wash.Find(w.Idwash).Code;
                    claims.Add(new Claim("Wash", washCode));
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

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Залогиниться")]
        [SwaggerResponse(200, Type = typeof(Token))]
        [SwaggerResponse(400, Type = typeof(Error))]
        #endregion
        [HttpPost("login")]
        public IActionResult Login(LoginModel login)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Error() { errorType = "badvalue", alert = "Некорректные входные параметры", errorCode = "Ошибка валидации", errorMessage = "Проверьте правильность введенных данных и попробуйте снова" });
            return Token(login);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить доступные значения фильтров")]
        [SwaggerResponse(200, Type = typeof(DashboardFilters))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("filters")]
        public IActionResult GetFilters()
        {
            try
            {
                UserInfo uInfo = new UserInfo(User.Claims.ToList());

                DashboardFilters filters = new DashboardFilters()
                {
                    regions = uInfo.GetRegions(),
                    washes = uInfo.GetWashes(),
                    posts = uInfo.GetPosts(),
                    changers = uInfo.GetChangers(),
                    operationTypes = uInfo.GetOperationTypes(),
                    eventChangerKinds = uInfo.GetEventChangerKinds()
                };

                return Ok(filters);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Записать нового пользователя")]
        [SwaggerResponse(201, Description = "Создан пользователь")]
        [SwaggerResponse(409, Type = typeof(Error), Description = "Пользователь с таким логином уже существует")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize(Roles = "dev")]
        [HttpPost]
        public IActionResult Register(AccountRequestModel account)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new Error() { errorType = "badvalue", alert = "Ошибка валидации", errorCode = "Некорректные входные параметры", errorMessage = "Проверьте правильность введенных данных и попробуйте снова" });

                if (_model.Users.Where(u => u.Login == account.login).FirstOrDefault() != null)
                    return Conflict(new Error() { errorType = "badvalue", alert = "Пользователь с таким логином уже существует", errorCode = "Конфликт введённых и имеющихся данных", errorMessage = "Введите другой логин и попробуйте снова" });

                _logger.LogInformation("запуск с параметрами: " + JsonConvert.SerializeObject(account));

                SqlHelper.WriteUser(account);

                return Created("", null);
            }
            catch (Exception e)
            {
                if (e.Message == "command") // ошибка в выполнении команды к бд
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error() { errorType = "db", alert = "Произошла ошибка во время обращения к базе данных", errorCode = "Ошибка базы данных", errorMessage = "Попробуйте снова или обратитесь к спецалисту" });
                }

                if (e.Message == "connection") // ошибка подключения к бд
                {
                    _logger.LogError("Не удалось подключиться к базе данных: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error() { errorType = "db", alert = "Не удалось подключиться к базе данных", errorCode = "Ошибка базы данных", errorMessage = "Попробуйте снова или обратитесь к спецалисту" });
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить всех пользователей")]
        [SwaggerResponse(200, Type = typeof(List<AccountViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize(Roles = "dev")]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<AccountViewModel> result = new List<AccountViewModel>();

                List<Users> users = _model.Users.ToList();
                foreach (Users u in users)
                {
                    List<string> washes = new List<string>();

                    List<UserWash> washIDs = _model.UserWash.Where(uw => uw.Iduser == u.Iduser).ToList();
                    foreach (UserWash w in washIDs)
                        washes.Add(_model.Wash.Find(w.Idwash).Code);

                    result.Add(new AccountViewModel
                    {
                        login = u.Login,
                        name = u.Name,
                        email = u.Email,
                        phone = u.Phone,
                        role = _model.Roles.Find(u.Idrole).Code,
                        washes = washes
                    });
                }

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Удалить пользователя")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error), Description = "Не найден пользователь")]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize(Roles = "dev")]
        [HttpDelete("{login}")]
        public IActionResult Delete(string login)
        {
            try
            {
                if (login == null)
                    return BadRequest(new Error() { errorType = "badvalue", alert = "Не задано имя пользователя", errorCode = "Ошибка валидации", errorMessage = "Проверьте, введено ли имя пользователя, и попробуйте снова" });

                if (_model.Users.Where(u => u.Login == login).FirstOrDefault() == null)
                    return NotFound(new Error() { errorType = "badvalue", alert = "Пользователя с таким логином не существует", errorCode = "Пользователь не найден", errorMessage = "Проверьте, правильно ли введён логин пользователя, и попробуйте снова" });

                SqlHelper.DeleteUser(login);
                _logger.LogInformation($"Удалён {login} пользователем {User.Identity.Name}");

                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message == "command") // ошибка в выполнении команды к бд
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error() { errorType = "db", alert = "Произошла ошибка во время обращения к базе данных", errorCode = "Ошибка базы данных", errorMessage = "Попробуйте снова или обратитесь к спецалисту" });
                }

                if (e.Message == "connection") // ошибка подключения к бд
                {
                    _logger.LogError("Не удалось подключиться к базе данных: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error() { errorType = "db", alert = "Не удалось подключиться к базе данных", errorCode = "Ошибка базы данных", errorMessage = "Попробуйте снова или обратитесь к спецалисту" });
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Изменить данные пользоателя")]
        [SwaggerResponse(200)]
        [SwaggerResponse(404, Type = typeof(Error))]
        [SwaggerResponse(409, Type = typeof(Error))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize(Roles = "dev")]
        [HttpPut]
        public IActionResult Update(UpdateAccountRequestModel account)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new Error() { errorType = "badvalue", alert = "Некорректные входные параметры", errorCode = "Ошибка валидации", errorMessage = "Проверьте правильность введенных данных и попробуйте снова" });
                }

                if (_model.Users.Where(u => u.Login == account.oldLogin).FirstOrDefault() == null)
                {
                    return NotFound(new Error() { errorType = "badvalue", alert = "Пользователя с таким логином не существует", errorCode = "Пользователь не найден", errorMessage = "Проверьте, правильно ли введён логин пользователя, и попробуйте снова" });
                }

                SqlHelper.UpdateUser(account);
                _logger.LogInformation($"пользователь {User.Identity.Name} изменил пользователя: " + JsonConvert.SerializeObject(account));
            }
            catch (Exception e)
            {
                if (e.Message == "command") // ошибка в выполнении команды к бд
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error() { errorType = "db", alert = "Произошла ошибка во время обращения к базе данных", errorCode = "Ошибка базы данных", errorMessage = "Попробуйте снова или обратитесь к спецалисту" });
                }

                if (e.Message == "connection") // ошибка подключения к бд
                {
                    _logger.LogError("Не удалось подключиться к базе данных: " + e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error() { errorType = "db", alert = "Не удалось подключиться к базе данных", errorCode = "Ошибка базы данных", errorMessage = "Попробуйте снова или обратитесь к спецалисту" });
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
            return Ok();
        }
    }
}