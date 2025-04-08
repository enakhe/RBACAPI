using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Auth.Commands.SendOTP;

public record SendOTPCommand : IRequest<ActionResult>;

public class SendOTPCommandValidator : AbstractValidator<SendOTPCommand>
{
    public SendOTPCommandValidator()
    {
    }
}

public class SendOTPCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<SendOTPCommand, ActionResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ActionResult> Handle(SendOTPCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            return
                new ObjectResult(new
                {
                    message = "Unauthorized access",
                    errors = "Invalid attempt. The email is not provided or incorrect"
                })
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

        var otpResponse = await _identityService.SendOTPAsync(email);

        if (!otpResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = "Unable to generate and send otp.",
                error = otpResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            message = "Great news! Your one-time password (OTP) has been generated and sent to your email inbox. Please check your email (and spam folder, if necessary) and use the OTP to verify your identity.",
            data = otpResponse
        });
    }
}

