using AuthenticationService.Extentions;
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

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Залогиниться")]
        [SwaggerResponse(200, Type = typeof(Token))]
        [SwaggerResponse(400, Type = typeof(ErrorModel))]
        #endregion
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            var token = await _accountsService.LoginAsync(login);
            return Ok(token);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить всех пользователей")]
        [SwaggerResponse(200, Type = typeof(List<AccountInfoDto>))]
        [SwaggerResponse(500, Type = typeof(ErrorModel))]
        #endregion
        [Authorize(Policy = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PagingParameter paging, [FromQuery] string query = null)
        {
            IQueryable<AccountInfoDto> accounts = await _accountsService.GetByQueryAsync(query);
            PagedList<AccountInfoDto> result = PagedList<AccountInfoDto>.ToPagedList(accounts, paging);

            return Ok(result);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Получить всех пользователей (количество записей)")]
        [SwaggerResponse(200, Type = typeof(int))]
        [SwaggerResponse(500, Type = typeof(ErrorModel))]
        #endregion
        [Authorize(Policy = "Admin")]
        [HttpGet("total_count")]
        public async Task<IActionResult> GetTotalCount([FromQuery] PagingParameter paging, [FromQuery] string query = null)
        {
            IQueryable<AccountInfoDto> accounts = await _accountsService.GetByQueryAsync(query);

            return Ok(accounts.Count());
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Зарегистрировать нового")]
        [SwaggerResponse(200, Type = typeof(AccountInfoDto))]
        [SwaggerResponse(409, Type = typeof(ErrorModel))]
        #endregion
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] NewAccountInfoDto account)
        {
            if (await _accountsService.IsLoginExistAsync(account.Login))
            {
                return Conflict(new ErrorModel() { ErrorType = "badvalue", Alert = "Некорректные входные параметры", ErrorCode = "Такой логин уже существует", ErrorMessage = "Попробуйте ввести другой логин" });
            }

            var user = await _accountsService.CreateAsync(account);

            return Created("", user);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Удалить по логину")]
        [SwaggerResponse(200)]
        #endregion
        [Authorize(Policy = "Admin")]
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
        public async Task<IActionResult> GetByLogin([FromRoute] string login)
        {
            AccountInfoDto account = await _accountsService.GetAsync(login);

            return Ok(account);
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Проверить логин")]
        [SwaggerResponse(200, Type = typeof(bool))]
        #endregion
        [HttpGet("isLoginForbidden")]
        public async Task<IActionResult> CheckLogin([FromQuery] string login, int? id = null)
        {
            return Ok(await _accountsService.IsLoginExistAsync(login, id));
        }

        [Authorize]
        [HttpGet("claims")]
        public async Task<IActionResult> GetClaims()
        {
            return Ok(User.Claims.Select(o => new { o.Type, o.Value }));
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("loginas/{login}")]
        public async Task<IActionResult> LoginAs(string login)
        {
            return Ok(await _accountsService.LoginAsync(login));
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateAccountModel account)
        {
            var login = await _accountsService.GetAsync(account.id);

            if (!User.HasClaim(c => c.Type == "IsAdmin" && c.Value == true.ToString()) && User.Identity.Name != login.Login)
            {
                throw new CustomStatusCodeException(System.Net.HttpStatusCode.Forbidden, "Изменение пользователя недоступно", "Можно менять данные своего аккаунта, либо необходимо обладать правами администратора");
            }

            var user = await _accountsService.UpdateAsync(account);
            return Ok(user);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut("password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            var account = await _accountsService.ChangePassword(model);
            return Ok(account);
        }
    }
}
