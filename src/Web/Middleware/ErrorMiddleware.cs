using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using RBACAPI.Application.Common.Exceptions;

namespace RBACAPI.Web.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger, ProblemDetailsFactory problemDetailsFactory)
        {
            _next = next;
            _logger = logger;
            _problemDetailsFactory = problemDetailsFactory;
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

            ProblemDetails problem;

            switch (ex)
            {
                case ValidationException vex:
                    problem = _problemDetailsFactory.CreateProblemDetails(
                        context,
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Validation Error",
                        detail: vex.Message);
                    problem.Extensions["errors"] = vex.Errors;
                    break;

                case UnauthorizedAccessException:
                    problem = _problemDetailsFactory.CreateProblemDetails(
                        context,
                        statusCode: StatusCodes.Status401Unauthorized,
                        title: "Unauthorized",
                        detail: ex.Message);
                    break;

                case KeyNotFoundException:
                    problem = _problemDetailsFactory.CreateProblemDetails(
                        context,
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Not Found",
                        detail: ex.Message);
                    break;

                default:
                    problem = _problemDetailsFactory.CreateProblemDetails(
                        context,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "Internal Server Error",
                        detail: ex.Message);
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

            var result = JsonSerializer.Serialize(problem);
            return context.Response.WriteAsync(result);
        }

    }
}
