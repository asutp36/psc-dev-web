using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Filters;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        [SwaggerOperation(Summary = "Получить все роли")]
        [SwaggerResponse(200, Type = typeof(List<RoleViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                List<RoleViewModel> result = new List<RoleViewModel>();

                List<Roles> roles = _model.Roles.ToList();
                foreach(Roles r in roles)
                {
                    result.Add(new RoleViewModel { code = r.Code, name = r.Name });
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }
    }
}
