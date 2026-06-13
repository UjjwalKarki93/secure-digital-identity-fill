using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Identity.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized access");
            await WriteErrorResponse(context, HttpStatusCode.Unauthorized, new[] { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Business rule violation");
            await WriteErrorResponse(context, HttpStatusCode.BadRequest, new[] { ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorResponse(context, HttpStatusCode.InternalServerError, new[] { "An unexpected error occurred." });
        }
    }

    private static async Task WriteErrorResponse(HttpContext context, HttpStatusCode statusCode, IEnumerable<string> errors)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var payload = JsonSerializer.Serialize(new { errors });
        await context.Response.WriteAsync(payload);
    }
}
