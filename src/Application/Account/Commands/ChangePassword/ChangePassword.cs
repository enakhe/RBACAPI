using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Account.Commands.ChangePassword;

public record ChangePasswordCommand : IRequest<IActionResult>
{

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Required, Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }
}

public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.Password)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.NewPassword)
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
            .Matches(x => x.NewPassword)
            .WithMessage("The password and confirmation password do not match.");
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IActionResult>
{
    private readonly IAccountService _accountService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangePasswordCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();

        var changePasswordResponse = await _accountService.ChangePassword(userId, request.Password, request.NewPassword);

        if (!changePasswordResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = changePasswordResponse.Message,
                succeded = changePasswordResponse.Succeeded,
                errors = changePasswordResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                changePasswordResponse.Title,
                changePasswordResponse.Message,
                changePasswordResponse.Succeeded,
                changePasswordResponse.Response
            }
        });
    }
}
