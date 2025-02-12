using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Auth.Commands.ChangePassword;

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
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.ConfirmPassword)
            .NotNull()
            .NotEmpty();
    }
}

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangePasswordCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid request");

        if (request.NewPassword != request.ConfirmPassword)
            return new BadRequestObjectResult(new
            {
                error = "The new password and confirmation password do not match."
            });

        var changePasswordResponse = await _identityService.ChangePassword(userId, request.Password, request.NewPassword);
        if (!changePasswordResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = changePasswordResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = changePasswordResponse
        });
    }
}
