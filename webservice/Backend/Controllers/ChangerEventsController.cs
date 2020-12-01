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
    public class ChangerEventsController : ControllerBase
    {
        ModelDbContext _model = new ModelDbContext();

        [SwaggerOperation(Summary = "Данные для страницы событий на разменнике")]
        [SwaggerResponse(200, Type = typeof(GetEventsByChangerViewModel))]
        [SwaggerResponse(500, Type = typeof(Error))]
        [Authorize]
        [HttpGet]
        public IActionResult Get(string startDate, string endDate, string changerCode = "", string kindEventCode = "")
        {
            try
            {
                SqlParameter p_DateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_ChangerCode = new SqlParameter("@p_ChangerCode", changerCode);
                SqlParameter p_KindEventCode = new SqlParameter("@p_KindEventCode", kindEventCode);
                SqlParameter IDEventChanger = new SqlParameter("@p_IDEventChanger", DBNull.Value);

                List<GetEventsByChanger_Result> events = _model.Set<GetEventsByChanger_Result>().FromSqlRaw("GetEventsByChanger @p_DateBeg, @p_DateEnd, @p_KindEventCode, @p_ChangerCode", p_DateBeg, p_DateEnd, p_KindEventCode, p_ChangerCode).ToList();

                List<GetDataEventsByChanger_Result> details = _model.Set<GetDataEventsByChanger_Result>().FromSqlRaw("GetDataEventsByChanger @p_DateBeg, @p_DateEnd, @p_IDEventChanger, @p_KindEventCode, @p_ChangerCode", p_DateBeg, p_DateEnd, IDEventChanger, p_KindEventCode, p_ChangerCode).ToList();

                List<GetEventsByChangerViewModel> result = new List<GetEventsByChangerViewModel>();

                foreach(GetEventsByChanger_Result e in events)
                {
                    GetEventsByChangerViewModel model = new GetEventsByChangerViewModel(e.ChangerName, e.DTime, e.KindEvent);
                    model.Details.AddRange(details.Where(d => d.IDEventChanger == e.IDEventChanger).ToList());

                    result.Add(model);
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