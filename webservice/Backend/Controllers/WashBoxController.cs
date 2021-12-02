using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Stored_Procedures;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashBoxController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы наличия в боксах по мойкам")]
        [SwaggerResponse(200, Type = typeof(GetBoxByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs")]
        public IActionResult ByWashs(string reportDate, int regionCode = 0, string washCode = "")
        {
            try
            {
                if (washCode == "")
                {
                    UserInfo uInfo = new UserInfo(User.Claims.ToList());
                    List<WashViewModel> washes = uInfo.GetWashes();

                    for (int i = 0; i < washes.Count; i++)
                    {
                        if (i == washes.Count - 1)
                        {
                            washCode += washes.ElementAt(i).code;
                            continue;
                        }

                        washCode += washes.ElementAt(i).code + ", ";
                    }
                }

                SqlParameter p_ReportDate = new SqlParameter("@p_ReportDate", reportDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                var result = _model.Set<GetBoxByWashs_Result>().FromSqlRaw("GetBoxByWashs @p_ReportDate, @p_RegionCode, @p_WashCode", p_ReportDate, p_RegionCode, p_WashCode);

                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы наличия в боксах по постам")]
        [SwaggerResponse(200, Type = typeof(GetBoxByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts")]
        public IActionResult ByPosts(string reportDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_ReportDate = new SqlParameter("@p_ReportDate", reportDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var result = _model.Set<GetBoxByPosts_Result>().FromSqlRaw("GetBoxByPosts @p_ReportDate, @p_RegionCode, @p_WashCode, @p_PostCode", p_ReportDate, p_RegionCode, p_WashCode, p_PostCode);

                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"));
            }
        }
    }
}