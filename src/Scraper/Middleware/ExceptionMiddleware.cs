using System.Net;
using System.Text.Json;
using Application.Models;
using Application.Models.Enums;

namespace Scraper.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error - {ex.Message}, Stack Trace - {ex.StackTrace}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        return context.Response.WriteAsync(JsonSerializer.Serialize(GetErrorResponse(exception)));
    }

    private ErrorResponse GetErrorResponse(Exception exception)
    {
        var result = new ErrorResponse();
        result.Errors.Add(new Error
        {
            ErrorCode = ErrorCodes.InternalError,
            ErrorMessage = "Internal Server Error"
        });
        return result;
    }
}