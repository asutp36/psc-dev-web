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
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashIncreaseController : ControllerBase
    {
        private readonly ILogger<WashIncreaseController> _logger;
        private ModelDbContext _model;

        public WashIncreaseController(ILogger<WashIncreaseController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }
        

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs")]
        public IActionResult GetByWashs(string startDate, string endDate, int regionCode = 0, string washCode = "")
        {
            try
            {
                if (washCode == "")
                {
                    UserInfo uInfo = new UserInfo(User.Claims.ToList());
                    List<WashViewModel> washes = uInfo.GetWashes();

                    for(int i = 0; i < washes.Count; i++)
                    {
                        if (i == washes.Count - 1)
                        {
                            washCode += washes.ElementAt(i).code;
                            continue;
                        }

                        washCode += washes.ElementAt(i).code + ", ";                        
                    }
                }
                

                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashs @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам после последней инкассации")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/after_collect")]
        public IActionResult GetByWashsAfterLastCollect(int regionCode = 0, string washCode = "")
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

                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsAfterLastCollect @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам между двумя последними инкассациями")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/between2last")]
        public IActionResult GetByWashsBetweenTwoLastCollects(int regionCode = 0, string washCode = "")
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

                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsBetweenTwoLastCollects @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
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

                var pocedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPosts @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);

                return Ok(pocedureResult);
            }
            catch(Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам после последней инкассации")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
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

                var pocedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPostsAfterLastCollect @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(pocedureResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам между двумя последними инкассациями")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
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

                var pocedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPostsBetweenTwoLastCollects @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(pocedureResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events")]
        public IActionResult GetByEvents(string startDate, string endDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var result = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEvents @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);

                return Ok(result);
            }
            catch(Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}