using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.User.Commands.Login;

public record SignInCommand : IRequest<ActionResult>
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}

public class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage("The password field is required");
    }
}

public class SignInCommandHandler : IRequestHandler<SignInCommand, ActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICookieService _cookieService;

    public SignInCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor, ICookieService cookieService)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
        _cookieService = cookieService;
    }

    public async Task<ActionResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var signInResponse = await _identityService.SignInAsync(request.Email, request.Password);

        if (!signInResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var result = 
                new ObjectResult(new
                {
                    message = "Unauthorized access. The email or password is incorrect.",
                    errors = signInResponse.Errors
                })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };

            return result;
        }

        _cookieService.SetCookie(signInResponse.AccessToken, "Auth.JWT.AccessToken");
        _cookieService.SetCookie(signInResponse.RefreshToken, "Auth.JWT.RefreshToken");

        return new OkObjectResult(new
        {
            message = "Great news! You’ve logged in successfully.",
            data = signInResponse
        });
    }
}
