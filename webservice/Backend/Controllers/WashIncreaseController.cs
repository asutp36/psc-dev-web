using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Backend.Controllers.Supplies.Stored_Procedures;
using Backend.Extentions;
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
        public IActionResult GetByWashs([FromQuery]PagingParameter paging, string startDate, string endDate, int regionCode = 0, string washCode = "")
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

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashs1 @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode);

                PagedList<GetIncreaseByWashs_Result> pagedResult = PagedList<GetIncreaseByWashs_Result>.ToPagedList(result, paging);

                PagedList<GetIncreaseByWashs_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/total_count")]
        public IActionResult GetByWashsTotalCount([FromQuery]PagingParameter paging, string startDate, string endDate, int regionCode = 0, string washCode = "")
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


                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashs1 @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode);

                return Ok(result.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам после последней инкассации")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/after_collect")]
        public IActionResult GetByWashsAfterLastCollect([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "")
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

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsAfterLastCollect1 @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode);

                PagedList<GetIncreaseByWashs_Result> pagedResult = PagedList<GetIncreaseByWashs_Result>.ToPagedList(result, paging);
                PagedList<GetIncreaseByWashs_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам после последней инкассации: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/after_collect/total_count")]
        public IActionResult GetByWashsAfterLastCollectTotalCount([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "")
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

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsAfterLastCollect1 @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode);

                return Ok(result.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам между двумя последними инкассациями")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/between2last")]
        public IActionResult GetByWashsBetweenTwoLastCollects([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "")
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

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsBetweenTwoLastCollects1 @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode);

                PagedList<GetIncreaseByWashs_Result> pagedResult = PagedList<GetIncreaseByWashs_Result>.ToPagedList(result, paging);
                PagedList<GetIncreaseByWashs_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по мойкам между двумя последними инкассациями: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByWashs_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("washs/between2last/total_count")]
        public IActionResult GetByWashsBetweenTwoLastCollectsTotalCount([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "")
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

                var result = _model.Set<GetIncreaseByWashs_Result>().FromSqlRaw("GetIncreaseByWashsBetweenTwoLastCollects1 @p_RegionCode, @p_WashCode", p_RegionCode, p_WashCode);

                return Ok(result.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts")]
        public IActionResult GetByPosts([FromQuery]PagingParameter paging, string startDate, string endDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPosts1 @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);

                PagedList<GetIncreaseByPosts_Result> pagedResult = PagedList<GetIncreaseByPosts_Result>.ToPagedList(procedureResult, paging);
                PagedList<GetIncreaseByPosts_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/total_count")]
        public IActionResult GetByPostsTotalCount([FromQuery]PagingParameter paging, string startDate, string endDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPosts1 @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);


                return Ok(procedureResult.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам после последней инкассации")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/after_collect")]
        public IActionResult GetByPostsAfterLastCollect([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPostsAfterLastCollect1 @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                PagedList<GetIncreaseByPosts_Result> pagedResult = PagedList<GetIncreaseByPosts_Result>.ToPagedList(procedureResult, paging);
                PagedList<GetIncreaseByPosts_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам после последней инкассации: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/after_collect/total_count")]
        public IActionResult GetByPostsAfterLastCollectTotalCount([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPostsAfterLastCollect1 @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(procedureResult.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам между двумя последними инкассациями")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/between2last")]
        public IActionResult GetByPostsBetweenTwoLastCollects([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPostsBetweenTwoLastCollects1 @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                PagedList<GetIncreaseByPosts_Result> pagedResult = PagedList<GetIncreaseByPosts_Result>.ToPagedList(procedureResult, paging);
                PagedList<GetIncreaseByPosts_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по постам между двумя последними инкассациями: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByPosts_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("posts/between2last/total_count")]
        public IActionResult GetByPostsBetweenTwoLastCollectsTotalCount([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByPosts_Result>().FromSqlRaw("GetIncreaseByPostsBetweenTwoLastCollects1 @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(procedureResult.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events")]
        public IActionResult GetByEvents([FromQuery]PagingParameter paging, string startDate, string endDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var result = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEvents @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);

                PagedList<GetIncreaseByEvents_Result> pagedResult = PagedList<GetIncreaseByEvents_Result>.ToPagedList(result, paging);
                PagedList<GetIncreaseByEvents_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events/total_count")]
        public IActionResult GetByEventsTotalCount([FromQuery]PagingParameter paging, string startDate, string endDate, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_dateBeg = new SqlParameter("@p_DateBeg", startDate);
                SqlParameter p_DateEnd = new SqlParameter("@p_DateEnd", endDate);
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var result = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEvents @p_DateBeg, @p_DateEnd, @p_RegionCode, @p_WashCode, @p_PostCode", p_dateBeg, p_DateEnd, p_RegionCode, p_WashCode, p_PostCode);

                return Ok(result.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям после последней инкассации")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events/after_collect")]
        public IActionResult GetByEventsAfterLastCollect([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEventsAfterLastCollect @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                PagedList<GetIncreaseByEvents_Result> pagedResult = PagedList<GetIncreaseByEvents_Result>.ToPagedList(procedureResult, paging);
                PagedList<GetIncreaseByEvents_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям после последней инкассации: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events/after_collect/total_count")]
        public IActionResult GetByEventsAfterLastCollectTotalCount([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEventsAfterLastCollect @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(procedureResult.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям между двумя последними инкассациями")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events/between2last")]
        public IActionResult GetByEventsBetweenTwoLastCollects([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEventsBetweenTwoLastCollects @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                PagedList<GetIncreaseByEvents_Result> pagedResult = PagedList<GetIncreaseByEvents_Result>.ToPagedList(procedureResult, paging);
                PagedList<GetIncreaseByEvents_Result>.PrepareHTTPResponseMetadata(Response, pagedResult);

                return Ok(pagedResult);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }

        #region Swagger description
        [SwaggerOperation(Summary = "Данные для страницы внесений по событиям между двумя последними инкассациями: количество строк")]
        [SwaggerResponse(200, Type = typeof(GetIncreaseByEvents_Result))]
        [SwaggerResponse(500, Type = typeof(Error))]
        #endregion
        [Authorize]
        [HttpGet("events/between2last/total_count")]
        public IActionResult GetByEventsBetweenTwoLastCollectsTotalCount([FromQuery]PagingParameter paging, int regionCode = 0, string washCode = "", string postCode = "")
        {
            try
            {
                SqlParameter p_RegionCode = new SqlParameter("@p_RegionCode", regionCode);
                SqlParameter p_WashCode = new SqlParameter("@p_WashCode", washCode);
                SqlParameter p_PostCode = new SqlParameter("@p_PostCode", postCode);

                var procedureResult = _model.Set<GetIncreaseByEvents_Result>().FromSqlRaw("GetIncreaseByEventsBetweenTwoLastCollects @p_RegionCode, @p_WashCode, @p_PostCode", p_RegionCode, p_WashCode, p_PostCode);

                return Ok(procedureResult.Count());
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace + Environment.NewLine);
                return StatusCode(500, new Error() { errorType = "unexpected", alert = "Что-то пошло не так в ходе работы сервера", errorCode = "Ошибка при обращении к серверу", errorMessage = "Попробуйте снова или обратитесь к специалисту" });
            }
        }
    }
}