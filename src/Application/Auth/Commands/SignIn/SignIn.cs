using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Auth.Commands.SignIn;

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

public class SignInCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor, ICookieService cookieService) : IRequestHandler<SignInCommand, ActionResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly ICookieService _cookieService = cookieService;

    public async Task<ActionResult> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var signInResponse = await _identityService.SignInAsync(request.Email, request.Password);

        if (!signInResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var problemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = "Invalid credentials. Please check your email and password.",
                Status = StatusCodes.Status401Unauthorized,
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = StatusCodes.Status401Unauthorized,
            };
        }

        _cookieService.SetCookie(signInResponse.AccessToken, "Auth.JWT.AccessToken", DateTimeOffset.UtcNow.AddMinutes(30));
        _cookieService.SetCookie(signInResponse.RefreshToken, "Auth.JWT.RefreshToken", DateTimeOffset.UtcNow.AddDays(7));

        return new OkObjectResult(new
        {
            message = "Great news! You’ve logged in successfully.",
            data = signInResponse
        });
    }
}
