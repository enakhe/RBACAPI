using System.ComponentModel.DataAnnotations;
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
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");
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
                message = otpResponse.Message,
                succeded = otpResponse.Succeeded,
                errors = otpResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                otpResponse.Title,
                otpResponse.Message,
                otpResponse.Succeeded,
                otpResponse.Response
            }
        });
    }
}

