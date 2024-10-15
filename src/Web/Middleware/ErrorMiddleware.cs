using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EcommerceAPI.Web.Middleware
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
                Status = context.Response.StatusCode,
                Detail = ex.Message,
            };

            switch (ex)
            {
                case InvalidOperationException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "Invalid operation";
                    break;
                case UnauthorizedAccessException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    problemDetails.Title = "Unauthorized";
                    break;
                default:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    problemDetails.Title = "Internal Server Error";
                    break;
            }

            context.Response.ContentType = "application/problem+json";

            var responseMessage = JsonSerializer.Serialize(problemDetails);
            return context.Response.WriteAsync(responseMessage);
        }
    }
}
