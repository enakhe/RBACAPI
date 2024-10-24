using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.User.Commands.SendOTP;

public record SendOTPCommand : IRequest<IActionResult>
{

}

public class SendOTPCommandValidator : AbstractValidator<SendOTPCommand>
{
    public SendOTPCommandValidator()
    {
    }
}

public class SendOTPCommandHandler : IRequestHandler<SendOTPCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SendOTPCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(SendOTPCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext; 
        var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(email))
            throw new UnauthorizedAccessException("User email not found.");

        var otpResponse = await _identityService.SendOTP(email);
        if(!otpResponse.Succeeded)
            throw new Exception("Something unexpected happened");

        return new OkObjectResult(new
        {
            otpResponse.Succeeded,
            otpResponse.UserId,
            otpResponse.Code,
            otpResponse.Email,
            otpResponse.CreatedDate,
            otpResponse.ExpiryDate
        });
    }
}

