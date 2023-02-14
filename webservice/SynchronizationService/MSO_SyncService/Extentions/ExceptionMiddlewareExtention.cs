using Microsoft.AspNetCore.Diagnostics;
using MSO.SyncService.Models;
using Newtonsoft.Json;
using System.Net;

namespace MSO.SyncService.Extentions
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
                                    new Error() { ErrorCode = "command", ErrorMessage = "Ошибка при выполнении запроса к бд" })
                                );
                                break;

                            case "db":
                                logger.LogError("Ошибка при попытке подключиться к бд: " + contextFeature.Error.InnerException.Message + Environment.NewLine
                                    + contextFeature.Error.InnerException.StackTrace + Environment.NewLine);

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new Error() { ErrorCode = "db", ErrorMessage = "Ошибка при подключении к бд" })
                                );
                                break;

                            default:
                                logger.LogError(contextFeature.Error.Message + Environment.NewLine + contextFeature.Error.StackTrace + Environment.NewLine);

                                await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                    new Error() { ErrorCode = "unexpexted", ErrorMessage = "Нерпедвиденное исключение" })
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
