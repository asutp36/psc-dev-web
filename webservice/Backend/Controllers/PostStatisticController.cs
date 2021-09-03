using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Stored_Procedures;
using Backend.Controllers.Supplies.ViewModels;
using Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class PostStatisticController : ControllerBase
    {
        private ModelDbContext _model;
        private readonly ILogger<PostStatisticController> _logger;

        public PostStatisticController(ILogger<PostStatisticController> logger)
        {
            _logger = logger;
            _model = new ModelDbContext();
        }

        #region Swagger Annotations
        [SwaggerOperation(Summary = "Данные для страницы статистики поста")]
        [SwaggerResponse(200, Type = typeof(PostStatisticViewModel))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet]
        public IActionResult Get(int regionCode = 0, string washCode = "", string postCode = "")
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

                List<PostStatisticViewModel> result = new List<PostStatisticViewModel>();

                List<GetBoxByPosts_Result> getBoxByPosts_Result = SqlHelper.GetBoxByPosts(DateTime.Now.Date.ToString("yyyy-MM-dd"), regionCode, washCode, postCode);

                //foreach(GetBoxByPosts_Result box in getBoxByPosts_Result)
                //{
                //    result.Add(new PostStatisticViewModel
                //    {
                //        postCode = box.PostCode,
                //        washCode = box.WashCode,
                //        sumall = box.sumall,
                //        sumofm = box.sumofm,
                //        sumofb = box.sumofb,
                //    });
                //}
                //result.postCode = getBoxByPosts_Result.PostCode;
                //result.washCode = getBoxByPosts_Result.WashCode;
                //result.sumall = getBoxByPosts_Result.sumall;
                //result.sumofm = getBoxByPosts_Result.sumofm;
                //result.sumofb = getBoxByPosts_Result.sumofb;

                List<GetIncreaseByPosts_Result> getIncreaseByPosts_Result = SqlHelper.GetIncreaseByPosts(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), regionCode, washCode, postCode);
                //if(getIncreaseByPosts_Result == null)
                //{
                //    result.sumIncrease = 0;
                //}
                //else
                //{
                //    result.sumIncrease = getIncreaseByPosts_Result.sumall;
                //} 

                //// реализовать запрос к последнему пингу
                //result.lastPing = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                //// узнать как брать минуты мойки
                //result.washMins = 100;

                for(int i = 0; i < getBoxByPosts_Result.Count; i++)
                {
                    result.Add(new PostStatisticViewModel
                    {
                        postCode = getBoxByPosts_Result[i].PostCode,
                        washCode = getBoxByPosts_Result[i].WashCode,
                        sumall = getBoxByPosts_Result[i].sumall,
                        sumofm = getBoxByPosts_Result[i].sumofm,
                        sumofb = getBoxByPosts_Result[i].sumofb,
                        sumIncrease = getIncreaseByPosts_Result[i] == null ? 0 : getIncreaseByPosts_Result[i].sumall,
                        lastPing = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        washMins = 777
                    });
                }

                return Ok(result);
            }
            catch(Exception e)
            {
                if(e.Message == "command")
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error(e.InnerException.Message, "command"));
                }

                if(e.Message == "connection")
                {
                    _logger.LogError("Не удалось подключиться с бд" + Environment.NewLine);
                    return StatusCode(500, new Error(e.Message, "connection"));
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }

        [HttpGet("get1")]
        public IActionResult Get1(int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                PostStatisticViewModel result = new PostStatisticViewModel();

                GetBoxByPosts_Result getBoxByPosts_Result = SqlHelper.GetBoxByPosts(DateTime.Now.Date.ToString("yyyy-MM-dd"), regionCode, washCode, postCode).FirstOrDefault();
                result.postCode = getBoxByPosts_Result.PostCode;
                result.washCode = getBoxByPosts_Result.WashCode;
                result.sumall = getBoxByPosts_Result.sumall;
                result.sumofm = getBoxByPosts_Result.sumofm;
                result.sumofb = getBoxByPosts_Result.sumofb;

                GetIncreaseByPosts_Result getIncreaseByPosts_Result = SqlHelper.GetIncreaseByPosts(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), regionCode, washCode, postCode).FirstOrDefault();
                if (getIncreaseByPosts_Result == null)
                {
                    result.sumIncrease = 0;
                }
                else
                {
                    result.sumIncrease = getIncreaseByPosts_Result.sumall;
                }

                // реализовать запрос к последнему пингу
                result.lastPing = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                // узнать как брать минуты мойки
                result.washMins = 100;

                return Ok(result);
            }
            catch (Exception e)
            {
                if (e.Message == "command")
                {
                    _logger.LogError(e.InnerException.Message + Environment.NewLine + e.InnerException.StackTrace + Environment.NewLine);
                    return StatusCode(500, new Error(e.InnerException.Message, "command"));
                }

                if (e.Message == "connection")
                {
                    _logger.LogError("Не удалось подключиться с бд" + Environment.NewLine);
                    return StatusCode(500, new Error(e.Message, "connection"));
                }

                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error(e.Message, "unexpected"));
            }
        }
    }
}
