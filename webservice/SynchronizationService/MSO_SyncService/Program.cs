using Microsoft.EntityFrameworkCore;
using MSO.SyncService.Extentions;
using MSO.SyncService.Services;
using MSO_SyncService.Models.WashCompanyDb;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Runtime.CompilerServices;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(swagger =>
    {
        swagger.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "MSO.SyncService",
            Description = "Сервис получения данных постов моек самообслуживаня",
            Version = "v1"
        });
    });

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Register WashCompany Database Context
    builder.Services.AddDbContext<WashCompanyDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("WashCompany"));
    });

    // Register services
    builder.Services.AddTransient<DeviceService>();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.ConfigureExceptionHandler(new NLogLoggerProvider().CreateLogger("Program.cs"));

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}
