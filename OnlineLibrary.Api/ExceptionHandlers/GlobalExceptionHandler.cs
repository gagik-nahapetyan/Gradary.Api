using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace OnlineLibrary.Api.ExceptionHandlers;

internal sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IHostEnvironment _environment;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger,
        IProblemDetailsService problemDetailsService,
        IHostEnvironment environment)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception");

        var (statusCode, title, detail) = MapException(exception);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        if (statusCode == StatusCodes.Status500InternalServerError && !_environment.IsDevelopment())
        {
            problemDetails.Detail = "An error occurred while processing your request.";
            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        }

        httpContext.Response.StatusCode = statusCode;

        await _problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true;
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception)
    {
        return exception switch
        {
            KeyNotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Not Found",
                ex.Message),
            ArgumentException ex => (
                StatusCodes.Status400BadRequest,
                "Bad Request",
                ex.Message),
            UnauthorizedAccessException ex => (
                StatusCodes.Status401Unauthorized,
                "Unauthorized",
                ex.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                exception.Message)
        };
    }
}
