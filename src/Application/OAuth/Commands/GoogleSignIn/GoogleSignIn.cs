using System.ComponentModel.DataAnnotations;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.OAuth.Commands.GoogleSignIn;

public record GoogleSignInCommand : IRequest<IActionResult>
{
    [Required]
    public required string code { get; set; }
}

public class GoogleSignInCommandValidator : AbstractValidator<GoogleSignInCommand>
{
    public GoogleSignInCommandValidator()
    {
    }
}

public class GoogleSignInCommandHandler : IRequestHandler<GoogleSignInCommand, IActionResult>
{
    private readonly IOAuthService _oAuthService;

    public GoogleSignInCommandHandler(IOAuthService oAuthService)
    {
        _oAuthService = oAuthService;
    }

    public async Task<IActionResult> Handle(GoogleSignInCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new BadRequestObjectResult("Invalid sign in request");
        }

        var signInResponse = await _oAuthService.GoogleSignIn(request.code);
        if (!signInResponse.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid sign in attempt");
        }

        return new OkObjectResult(new
        {
            signInResponse.UserId,
            signInResponse.UserName,
            signInResponse.Email,
            signInResponse.PhoneNumber,
            signInResponse.EmailConfirmed,
            signInResponse.Token,
        });
    }
}
