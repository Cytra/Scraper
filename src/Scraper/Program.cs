using Application.Options;
using Serilog;
using Application.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Reflection;
using Application.Models;
using Application.Queries;
using Scraper.Middleware;
using Infrastructure.Scrapers;

Log.Logger = new LoggerConfiguration()
    .WriteTo
    .Console()
    .CreateLogger();

try
{
    Log.Logger.Debug("init main");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Add services to the container.

    builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    builder.Services.AddMediatR(typeof(GetHtml.Query).Assembly);

    builder.Services.Configure<AppOptions>(builder.Configuration);
    builder.Services.AddScoped<ISeleniumDriverFactory, SeleniumDriverFactory>();


    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(setupAction =>
        {
            setupAction.InvalidModelStateResponseFactory = context =>
            {
                var apiResponse = new ErrorResponse();
                foreach (var modelState in context.ModelState)
                {
                    foreach (var error in modelState.Value.Errors)
                    {
                        apiResponse.Errors.Add(new Error() { ErrorCode = ErrorCodes.BadRequest, ErrorMessage = error.ErrorMessage });
                    }
                }
                return new BadRequestObjectResult(apiResponse);
            };
        });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.UseHttpLogging();
    var env = app.Environment;
    //    if (env.IsDevelopment())
    //    {
    //;
    //    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
    });
    app.UseDeveloperExceptionPage();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapHealthChecks("/healthz");
    });

    app.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Stopped program because of exception");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}