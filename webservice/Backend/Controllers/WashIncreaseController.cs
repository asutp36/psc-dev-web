﻿using System;
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
using Swashbuckle.Swagger.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WashIncreaseController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
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
                return StatusCode(500, new Error("Внутренняя ошибка сервера", "unexpected"));
            }
        }
    }
}