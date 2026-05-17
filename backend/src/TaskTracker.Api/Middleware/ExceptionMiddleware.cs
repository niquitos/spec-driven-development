using System.Net;
using System.Text.Json;

namespace TaskTracker.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ArgumentException => new ErrorResponse(
                HttpStatusCode.BadRequest,
                "Invalid argument",
                exception.Message
            ),
            KeyNotFoundException => new ErrorResponse(
                HttpStatusCode.NotFound,
                "Resource not found",
                exception.Message
            ),
            InvalidOperationException => new ErrorResponse(
                HttpStatusCode.BadRequest,
                "Invalid operation",
                exception.Message
            ),
            _ => new ErrorResponse(
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred",
                "Please try again later"
            )
        };

        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public record ErrorResponse(HttpStatusCode StatusCode, string Title, string Detail);

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
