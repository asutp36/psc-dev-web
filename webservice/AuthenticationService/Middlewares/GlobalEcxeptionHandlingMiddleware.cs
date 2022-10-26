using AuthenticationService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AuthenticationService.Middlewares
{
    public class GlobalEcxeptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalEcxeptionHandlingMiddleware> _logger;

        public GlobalEcxeptionHandlingMiddleware(ILogger<GlobalEcxeptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try 
            {
                await next(context);
            }
            catch(CustomStatusCodeException e)
            {
                context.Response.StatusCode = (int)e.StatusCode;

                string json = JsonConvert.SerializeObject(new { e.Message, e.Description });
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
            catch(Exception e)
            {
                _logger.LogError($"Перехвачено неожиданное исключение: {e.GetType()} - {e.Message}" + Environment.NewLine + e.StackTrace);

                CustomStatusCodeException customException = new CustomStatusCodeException(HttpStatusCode.InternalServerError, "Произошла непредвиденная ошибка", 
                    "В ходе работы сервера произошла непредвиденная ошибка. Попробуйте позже или обратитесь к программистам");

                context.Response.StatusCode = (int)customException.StatusCode;

                string json = JsonConvert.SerializeObject(customException);

                await context.Response.WriteAsync(json);

                context.Response.ContentType = "application/json";
            }
        }
    }
}
