using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.User.Commands.SignUp;
public record SignUpCommand : IRequest<Result>
{
    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required, Compare(nameof(Password))]
    public required string ConfirmPassword { get; set; }
}

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    private readonly IApplicationDbContext _context;

    public SignUpCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage("The password field is required")
            .MinimumLength(6)
            .WithMessage("The minimum length of the password must be more than six characters");

        RuleFor(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage("The confirmation password field is required.")
            .Matches(x => x.Password)
            .WithMessage("The password and confirmation password do not match.");

    }
}

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, Result>
{
    private readonly IIdentityService _identityService;
    private readonly ICookieService _cookieService;
    public SignUpCommandHandler(IIdentityService identityService, ICookieService cookieService)
    {
        _identityService = identityService;
        _cookieService = cookieService;
    }

    public SignUpCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor, ICookieService cookieService)
    {
        _identityService = identityService;
        _cookieService = cookieService;
    }

    public async Task<Result> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        var signUpResponse = await _identityService.SignUpAsync(request.Email, request.Password);
        if (!signUpResponse.Succeeded)
        {
            return Result.Failure(signUpResponse.Errors);
        }

        _cookieService.SetCookie(signUpResponse.AccessToken, "Auth.JWT.AccessToken");
        _cookieService.SetCookie(signUpResponse.RefreshToken, "Auth.JWT.RefreshToken");

        return Result.Success(new
        {
            message = "Welcome aboard! Your account has been created successfully",
            data = signUpResponse
        });
    }
}
