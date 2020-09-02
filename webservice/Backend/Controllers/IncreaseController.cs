using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncreaseController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

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

            return Ok(ssum.Value);
        }

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
                return StatusCode(500, e);
            }
        }
    }
}