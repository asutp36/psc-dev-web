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

            var increases = _model.Database.ExecuteSqlRaw("GetIncreaseDurPeriod @p_DateBeg, @p_DateEnd, @p_Login, @p_WashCode", dateBeg, dateEnd, log, wash);
            return Ok(increases);
        }
    }
}