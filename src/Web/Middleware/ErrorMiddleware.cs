using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Exceptions;

namespace RBACAPI.Web.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
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

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred");

            var problemDetails = new ProblemDetails
            {
                Title = "An error occurred",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
            };

            switch (ex)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Extensions.Add("errors", validationException.Errors);
                    break;

                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid operation";
                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    break;

                case KeyNotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    problemDetails.Title = "Not found";
                    problemDetails.Status = StatusCodes.Status404NotFound;
                    break;

                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    problemDetails.Status = StatusCodes.Status401Unauthorized;
                    break;

                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    problemDetails.Status = StatusCodes.Status500InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            var responseMessage = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(responseMessage);
        }
    }
}
