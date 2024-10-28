using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.User.Commands.Login;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.User.Commands.SignUp;

public record SignUpCommand : IRequest<IActionResult>
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
            .NotNull()
            .NotEmpty();
    }
}

public class SignUpCommandHandler : IRequestHandler<SignUpCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SignUpCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(SignUpCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
            return new BadRequestObjectResult("Invalid sign up request");

        var signUpResponse = await _identityService.SignUpAsync(request.Email, request.Password);
        if (!signUpResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = signUpResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = signUpResponse
        });
    }
}
