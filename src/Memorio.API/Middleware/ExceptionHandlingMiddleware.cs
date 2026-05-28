using System.Diagnostics;
using System.Text.Json;
using Memorio.Shared.Exceptions;

namespace Memorio.API.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = GetStatusCode(exception);
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        _logger.LogError(
            exception,
            "Unhandled exception occurred. TraceId: {TraceId}, StatusCode: {StatusCode}",
            traceId,
            statusCode);

        var response = new ErrorResponse(
            Type: GetType(statusCode),
            Title: GetTitle(statusCode),
            Status: statusCode,
            Detail: GetDetail(exception, statusCode),
            TraceId: traceId);

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await JsonSerializer.SerializeAsync(context.Response.Body, response, JsonOptions);
    }

    private static int GetStatusCode(Exception exception) =>
        exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            ValidationException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetType(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "https://httpstatuses.com/400",
            StatusCodes.Status401Unauthorized => "https://httpstatuses.com/401",
            StatusCodes.Status404NotFound => "https://httpstatuses.com/404",
            _ => "https://httpstatuses.com/500"
        };

    private static string GetTitle(int statusCode) =>
        statusCode switch
        {
            StatusCodes.Status400BadRequest => "Validation error",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status404NotFound => "Resource not found",
            _ => "Internal server error"
        };

    private string GetDetail(Exception exception, int statusCode)
    {
        if (_environment.IsDevelopment())
        {
            return exception.ToString();
        }

        return statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred."
            : exception.Message;
    }

    private sealed record ErrorResponse(
        string Type,
        string Title,
        int Status,
        string Detail,
        string TraceId);
}
