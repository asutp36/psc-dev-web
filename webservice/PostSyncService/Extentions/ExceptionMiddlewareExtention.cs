using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PostSyncService.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PostSyncService.Extentions
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
                    if (contextFeature != null)
                    {
                        switch (contextFeature.Error.Message)
                        {
                            case "command":
                                logger.LogError("Ошибка во время выполнения запроса: " + contextFeature.Error.InnerException.Message + Environment.NewLine 
                                    + contextFeature.Error.InnerException.StackTrace + Environment.NewLine);

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new Error() { errorCode = "command", errorMessage = "Ошибка при выполнении запроса к бд" })
                                );
                                break;

                            case "db":
                                logger.LogError("Ошибка при попытке подключиться к бд: " + contextFeature.Error.InnerException.Message + Environment.NewLine
                                    + contextFeature.Error.InnerException.StackTrace + Environment.NewLine);

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new Error() { errorCode = "db", errorMessage = "Ошибка при подключении к бд" })
                                );
                                break;

                            default:
                                logger.LogError(contextFeature.Error.Message + Environment.NewLine + contextFeature.Error.StackTrace + Environment.NewLine);

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new Error() { errorCode = "unexpexted", errorMessage = "Нерпедвиденное исключение" })
                                );
                                break;
                        }
                    }
                });
            }
            );
        }
    }
}
