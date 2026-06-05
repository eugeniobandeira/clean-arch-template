using CleanArch.Domain.Constants;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CleanArch.Api.Middlewares;

internal sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    private const int StatusClientClosedRequest = 499;
    private const string ProblemDetailsContentType = "application/problem+json";

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is OperationCanceledException && httpContext.RequestAborted.IsCancellationRequested)
        {
            logger.LogWarning("Request cancelled by client.");
            httpContext.Response.StatusCode = StatusClientClosedRequest;
            return true;
        }

        if (httpContext.Response.HasStarted)
        {
            logger.LogWarning("Response already started, cannot write error.");
            return true;
        }

        string? correlationId = httpContext.Items[CorrelationIdConstants.Key]?.ToString();
        string traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Unhandled exception.");

        ProblemDetails problem = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An unexpected error occurred.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["correlationId"] = correlationId,
                ["traceId"] = traceId
            }
        };

        if (environment.IsDevelopment())
            problem.Extensions["exceptionType"] = exception.GetType().FullName;

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        httpContext.Response.ContentType = ProblemDetailsContentType;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);

        return true;
    }
}
