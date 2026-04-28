using GivingChampion.Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GivingChampion.API.Handlers
{
    public class GlobalExceptionHandler(
        IProblemDetailsService problemDetailsService,
        ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var statusCode = exception switch
            {
                BadRequestException => StatusCodes.Status400BadRequest,
                NotFoundException => StatusCodes.Status404NotFound,
                ConflictException => StatusCodes.Status409Conflict,

                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                ApplicationException => StatusCodes.Status400BadRequest,

                _ => StatusCodes.Status500InternalServerError
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "Unhandled exception occurred");
            else
                logger.LogWarning(exception, "Handled business exception occurred");

            httpContext.Response.StatusCode = statusCode;

            var title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "Bad request",
                StatusCodes.Status404NotFound => "Not found",
                StatusCodes.Status409Conflict => "Conflict",
                StatusCodes.Status403Forbidden => "Forbidden",
                _ => "An error occurred"
            };

            return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = title,
                    Status = statusCode,
                    Detail = exception.Message,
                    Extensions =
                    {
                        ["traceId"] = httpContext.TraceIdentifier
                    }
                }
            });
        }
    }
}