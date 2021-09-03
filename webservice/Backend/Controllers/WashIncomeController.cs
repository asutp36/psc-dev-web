using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Stored_Procedures;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashIncomeController : ControllerBase
    {
        private readonly ILogger<WashIncomeController> _logger;
        private ModelDbContext _model;

        public WashIncomeController(ILogger<WashIncomeController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы поступлений по постам")]
        [SwaggerResponse(200, Type = typeof(GetIncomeByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts")]
        public IActionResult GetByPosts(string startDate, string endDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var pocedureResult = _model.Set<GetIncomeByPosts_Result>().FromSqlRaw("GetIncomeByPosts @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);

                return Ok(pocedureResult);
            }
            catch (Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы поступлений по постам после последней инкассации")]
        [SwaggerResponse(200, Type = typeof(GetIncomeByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/after_collect")]
        public IActionResult GetByPostsAfterLastCollect(int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var pocedureResult = _model.Set<GetIncomeByPosts_Result>().FromSqlRaw("GetIncomeByPostsAfterLastCollect @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(pocedureResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы поступлений по постам между двумя последними инкассациями")]
        [SwaggerResponse(200, Type = typeof(GetIncomeByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/between2last")]
        public IActionResult GetByPostsBetweenTwoLastCollects(int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var pocedureResult = _model.Set<GetIncomeByPosts_Result>().FromSqlRaw("GetIncomeByPostsBetweenTwoLastCollects @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(pocedureResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
