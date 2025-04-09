using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Domain.Enums;

namespace RBACAPI.Application.Account.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest<ActionResult>
{
    [Required]
    public string? FirstName { get; init; }

    [Required]
    public string? LastName { get; init; }

    [Required]
    public IFormFile? ProfilePicture { get; init; }

    [Required]
    public GenderData GenderData { get; init; }

    [Required, EmailAddress]
    public required string Email { get; set; }

    [Required, Phone]
    public string? PhoneNumber { get; init; }
}

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotNull()
            .NotEmpty()
            .WithMessage("The first name field is required")
            .MaximumLength(20)
            .WithMessage("The first name field has a maximum of 20 characters. Please shorten your first name and try again");

        RuleFor(x => x.LastName)
            .NotNull()
            .NotEmpty()
            .WithMessage("The last name field is required")
            .MaximumLength(20)
            .WithMessage("The last name field has a maximum of 20 characters. Please shorten your last name and try again");

        RuleFor(x => x.ProfilePicture)
            .NotNull()
            .NotEmpty()
            .WithMessage("The profile picture field is required")
            .Must(x => x != null && x.Length <= 2 * 1024 * 1024)
            .WithMessage("The profile picture field must be less than 2MB")
            .Must(x => x != null && (x.ContentType == "image/jpeg" || x.ContentType == "image/png" || x.ContentType == "image/jpg"))
            .WithMessage("The profile picture field must be a jpeg, jpg or png file");

        RuleFor(x => x.GenderData)
            .NotNull()
            .NotEmpty()
            .WithMessage("The gender field is required")
            .IsInEnum()
            .WithMessage("The provided gender is not an allowed type");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("The email field must be a valid email address")
            .NotNull()
            .NotEmpty()
            .WithMessage("The email field is required");

        RuleFor(x => x.PhoneNumber)
            .NotNull()
            .NotEmpty()
            .WithMessage("The phone number field is required");
    }
}


public class UpdateProfileCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<UpdateProfileCommand, ActionResult>
{
    private readonly IAccountService _accountService = accountService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<ActionResult> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            return new UnauthorizedResult();

        var updateProfileResponse = await _accountService.UpdateProfileAsync(userId, request.FirstName!, request.LastName!, request.ProfilePicture!, request.GenderData, request.Email, request.PhoneNumber!);

        if (!updateProfileResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                message = updateProfileResponse.Message,
                succeded = updateProfileResponse.Succeeded,
                errors = updateProfileResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = new
            {
                updateProfileResponse.Title,
                updateProfileResponse.Message,
                updateProfileResponse.Succeeded,
                updateProfileResponse.Response
            }
        });
    }
}
