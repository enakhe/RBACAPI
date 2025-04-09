using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Auth.Commands.ResetPassword;

public record ResetPasswordCommand : IRequest<IActionResult>
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Code { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
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
            .WithMessage("The password field is required")
            .MinimumLength(6)
            .WithMessage("The minimum length of the password must be more than six characters")
            .Matches("[A-Z]").WithMessage("The password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("The password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("The password must contain at least one digit");

        RuleFor(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty()
            .WithMessage("The confirmation password field is required.")
            .Matches(x => x.Password)
            .WithMessage("The password and confirmation password do not match.");
    }
}

public class ResetPasswordCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<ResetPasswordCommand, IActionResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<IActionResult> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var resetPasswordResponse = await _identityService.ResetPasswordAsync(request.Email, request.Code, request.Password);

        if (!resetPasswordResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = resetPasswordResponse.Message,
                succeded = resetPasswordResponse.Succeeded,
                errors = resetPasswordResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                resetPasswordResponse.Title,
                resetPasswordResponse.Message,
                resetPasswordResponse.Succeeded,
                resetPasswordResponse.Response
            }
        });
    }
}
