using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Auth.Commands.SendOTP;

public record SendOTPCommand : IRequest<ActionResult>
{
    [Required, EmailAddress]
    public required string Email { get; set; }
}

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
            data = otpResponse
        });
    }
}

