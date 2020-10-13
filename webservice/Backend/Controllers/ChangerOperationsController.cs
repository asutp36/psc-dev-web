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
    public class ChangerOperationsController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        [SwaggerResponse(200, Type = typeof(GetSumsByChanger_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet("sums")]
        public IActionResult GetSumsByChanger(string dateBeg, string dateEnd, int regionCode = 0, string changerCode = "")
        {
            try
            {
                SqlParameter p_DateBeg = new SqlParameter("@p_DateBeg", dateBeg);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", dateEnd);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_ChangerCode = new SqlParameter("@p_ChangerCode", changerCode);

                var result = _model.Set<GetSumsByChanger_Result>().FromSqlRaw("GetSumsByChanger @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_ChangerCode", p_DateBeg, p_DateEnd, p_RegionCode, p_ChangerCode);

                return Ok(result);
            } 
            catch(Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}