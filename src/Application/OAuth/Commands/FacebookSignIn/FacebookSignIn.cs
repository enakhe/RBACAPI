using System.ComponentModel.DataAnnotations;
using RBACAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace RBACAPI.Application.OAuth.Commands.FacebookSignIn;

public record FacebookSignInCommand : IRequest<IActionResult>
{
    [Required]
    public required string accessToken { get; set; }
}

public class FacebookSignInCommandValidator : AbstractValidator<FacebookSignInCommand>
{
    public FacebookSignInCommandValidator()
    {
    }
}

public class FacebookSignInCommandHandler : IRequestHandler<FacebookSignInCommand, IActionResult>
{
    private readonly IOAuthService _oAuthService;

    public FacebookSignInCommandHandler(IOAuthService oAuthService)
    {
        _oAuthService = oAuthService;
    }

    public async Task<IActionResult> Handle(FacebookSignInCommand request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new BadRequestObjectResult("Invalid sign in request");
        }

        var signInResponse = await _oAuthService.FacebookSignIn(request.accessToken);
        if (!signInResponse.Data.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid sign in attempt");
        }

        return new OkObjectResult(new
        {
            signInResponse.Data.UserId,
            signInResponse.Data.UserName,
            signInResponse.Data.Email,
            signInResponse.Data.PhoneNumber,
            signInResponse.Data.EmailConfirmed,
            signInResponse.Data.Token,
        });
    }
}
