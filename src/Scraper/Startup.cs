using System.Reflection;
using Application.Interfaces;
using Application.Models;
using Application.Models.Enums;
using Application.Options;
using Application.Ports;
using Application.Queries;
using Application.Services;
using Application.Services.Parsers;
using Application.Services.Selector;
using Infrastructure.Scrapers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Scraper.Middleware;
using Swashbuckle.AspNetCore.Filters;

namespace Scraper;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    public virtual void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpLogging(_ => { });
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(GetHtml.Query).Assembly);
        });

        services.Configure<AppOptions>(Configuration);
        services.AddScoped<ISeleniumDriverFactory, SeleniumDriverFactory>();
        services.AddScoped<ISeleniumService, SeleniumService>();
        services.AddScoped<IHtmlToJsonService, HtmlToJsonService>();
        services.AddScoped<IHtmlParser<ImplicitExtractRule>, HtmlParser<ImplicitExtractRule>>();
        services.AddScoped<IHtmlParser<ExplicitExtractRule>, HtmlParser<ExplicitExtractRule>>();
        services.AddScoped<IJsonExtractorFacade<ImplicitExtractRule>, JsonExtractorFacade<ImplicitExtractRule>>();
        services.AddScoped<IJsonExtractorFacade<ExplicitExtractRule>, JsonExtractorFacade<ExplicitExtractRule>>();
        services.AddScoped<ISelectorService<ImplicitExtractRule>, ImplicitSelectorService>();
        services.AddScoped<ISelectorService<ExplicitExtractRule>, ExplicitSelectorService>();

        services.AddControllers()
            .ConfigureApiBehaviorOptions(setupAction =>
            {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var apiResponse = new ErrorResponse();
                    foreach (var modelState in context.ModelState)
                    {
                        foreach (var error in modelState.Value.Errors)
                        {
                            apiResponse.Errors.Add(new Error()
                                { ErrorCode = ErrorCodes.BadRequest, ErrorMessage = error.ErrorMessage });
                        }
                    }

                    return new BadRequestObjectResult(apiResponse);
                };
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.ExampleFilters();
        });
        services.AddSwaggerExamplesFromAssemblyOf();

        services.AddHealthChecks();
        services.AddCors(options =>
        {
            options.AddPolicy("Test",
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod();
                });
        });
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        app.UseHttpLogging();

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
        });
        app.UseDeveloperExceptionPage();
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseRouting();

        app.UseCors("Test");

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthz");
        });
    }
}