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
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Annotations;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangerOperationsController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        [SwaggerOperation(Summary = "Данные для страницы операций на разменнике")]
        [SwaggerResponse(200, Type = typeof(List<GetSumsByChangerViewModel>))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet("sums")]
        public IActionResult GetSumsByChanger(string startDate, string endDate, int regionCode = 0, string changerCode = "")
        {
            try
            {
                SqlParameter p_DateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_ChangerCode = new SqlParameter("@p_ChangerCode", changerCode);

                List<GetSumsByChanger_Result> sums = _model.Set<GetSumsByChanger_Result>().FromSqlRaw("GetSumsByChanger @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_ChangerCode", p_DateBeg, p_DateEnd, p_RegionCode, p_ChangerCode).ToList();

                List<GetSumsByChangerViewModel> result = new List<GetSumsByChangerViewModel>();

                foreach(GetSumsByChanger_Result s in sums)
                {
                    Supplies.HttpResponse response = HttpSender.SendGet("http://194.87.98.177/postrc/api/changer/state/" + s.ChangerCode);

                    if(response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        result.Add(new GetSumsByChangerViewModel(s, JsonConvert.DeserializeObject<ChangerInfo>(response.ResultMessage)));
                    }
                    else
                    {
                        result.Add(new GetSumsByChangerViewModel(s));
                    }
                }

                return Ok(result);
            } 
            catch(Exception e)
            {
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}