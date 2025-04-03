using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.User.Commands.SendOTP;

public record SendOTPCommand : IRequest<ActionResult>
{
    [Required, EmailAddress]
    public required string Email { get; set; }
}

public class SendOTPCommandValidator : AbstractValidator<SendOTPCommand>
{
    public SendOTPCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");
    }
}

public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, ActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SendOTPCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ActionResult> Handle(SendOTPCommand request, CancellationToken cancellationToken)
    {
        var otpResponse = await _identityService.SendOTPAsync(request.Email);
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

