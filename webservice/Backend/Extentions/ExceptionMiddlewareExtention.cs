using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Backend.Controllers.Supplies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Backend.Extentions
{
    public static class ExceptionMiddlewareExtention
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError => 
                {
                    appError.Run(async context => 
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            context.Response.ContentType = "application/json";

                            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                            if(contextFeature != null)
                            {
                                logger.LogError("" + Environment.NewLine);

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new Error("Что-то пошло не так в ходе работы программы сервера. Обратитесь к специалисту.", "unexpected"))
                                );
                            }
                        });
                }
            );
        }
    }
}
