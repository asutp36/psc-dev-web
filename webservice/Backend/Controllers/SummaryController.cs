using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
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
    public class SummaryController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();
        //ILogger<IncreaseController> _logger;

        //public IncreaseController(ILogger<IncreaseController> logger)
        //{
        //    _logger = logger;
        //}

        [HttpGet("durperiod")]
        public IActionResult GetDurPeriod(string startDate, string endDate, string login, string washCode)
        {
            SqlParameter dateBeg = new SqlParameter("@p_DateBeg", startDate);
            SqlParameter dateEnd = new SqlParameter("@p_DateEnd", endDate);
            SqlParameter log = new SqlParameter("@p_Login", login);
            SqlParameter wash = new SqlParameter("@p_WashCode", washCode);

            SqlParameter ssum = new SqlParameter()
            {
                ParameterName = "@ssum",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.Output
            };

            _model.Database.ExecuteSqlRaw("GetIncreaseDurPeriod @p_DateBeg, @p_DateEnd, @p_Login, @p_WashCode, @ssum OUT", dateBeg, dateEnd, log, wash, ssum);
            if (ssum.Value is System.DBNull)
                return Ok(0);
            else
                return Ok((int)ssum.Value);
        }

        [Authorize]
        [HttpGet("bywashs")]
        public IActionResult GetByWashs(string startDate, string endDate, int regionCode = 0, string washCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashs @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode);

                return Ok(result);
            }
            catch (Exception e)
            {
                //_logger.LogError("GetByWashs: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500);
            }
        }

        [SwaggerResponse(200, Type = typeof(Summary))]
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            //_logger.LogInformation("Svodka: запуск с парамерами ");
            try
            {
                UserInfo uInfo = new UserInfo(User.Claims.ToList());
                List<WashViewModel> washes = uInfo.GetWashes();

                Summary result = new Summary();
                string begdate = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
                string enddate = DateTime.Today.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss");

                SqlParameter p_DateBeg = new SqlParameter("@p_DateBeg", DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", DateTime.Today.AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));
                SqlParameter p_Login = new SqlParameter("@p_Login", "");
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washes.FirstOrDefault().code);

                SqlParameter ssum = new SqlParameter()
                {
                    ParameterName = "@ssum",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                _model.Database.ExecuteSqlRaw("GetIncreaseBefDate @p_DateEnd, @p_Login, @p_WashCode, @ssum OUT", p_DateEnd, p_Login, p_WashCode, ssum);
                if(ssum.Value is System.DBNull)
                    result.increaseAllTime = 0;
                else
                    result.increaseAllTime = (int)ssum.Value;

                _model.Database.ExecuteSqlRaw("GetIncreaseDurPeriod @p_DateBeg, @p_DateEnd, @p_Login, @p_WashCode, @ssum OUT", p_DateBeg, p_DateEnd, p_Login, p_WashCode, ssum);
                if (ssum.Value is System.DBNull)
                    result.increaseYesterday = 0;
                else
                    result.increaseYesterday = (int)ssum.Value;

                _model.Database.ExecuteSqlRaw("GetCollectDurPeriod @p_DateBeg, @p_DateEnd, @p_Login, @p_WashCode, @ssum OUT", p_DateBeg, p_DateEnd, p_Login, p_WashCode, ssum);
                if (ssum.Value is System.DBNull)
                    result.collectLastMonth = 0;
                else
                    result.collectLastMonth = (int)ssum.Value;

                return Ok(result);
            }
            catch (Exception e)
            {
                //_logger.LogError("Svodka: " + e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, e.Message);
            }
        }
    }
}