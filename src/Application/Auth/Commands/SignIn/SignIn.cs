using System.ComponentModel.DataAnnotations;
using RBACAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RBACAPI.Application.User.Commands.Login;

public record SignInCommand : IRequest<IActionResult>
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SignInCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var signInResponse = await _identityService.SignInAsync(request.Email, request.Password);

        if (!signInResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return new UnauthorizedObjectResult(new
            {
                error = signInResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = signInResponse
        });
    }
}
