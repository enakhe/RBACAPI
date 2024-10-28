using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.Auth.Commands.GetPasswordResetToken;

public record GetPasswordResetTokenCommand : IRequest<IActionResult>
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}

public class GetPasswordResetTokenCommandValidator : AbstractValidator<GetPasswordResetTokenCommand>
{
    public GetPasswordResetTokenCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .NotNull()
            .NotEmpty();
    }
}

public class GetPasswordResetTokenCommandHandler : IRequestHandler<GetPasswordResetTokenCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public GetPasswordResetTokenCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(GetPasswordResetTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Email))
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new UnauthorizedObjectResult(new
            {
                error = new[] { "Not authorized" }
            });
        }

        var getResetPasswordResponse = await _identityService.GetPasswordResetTokenAsync(request.Email);
        if(!getResetPasswordResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = getResetPasswordResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            getResetPasswordResponse
        });
    }
}
