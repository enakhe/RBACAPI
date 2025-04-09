using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Auth.Commands.LogOut;

public record LogOutCommand : IRequest<IActionResult>;

public class LogOutCommandValidator : AbstractValidator<LogOutCommand>
{
    public LogOutCommandValidator()
    {
    }
}

public class LogOutCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor) : IRequestHandler<LogOutCommand, IActionResult>
{
    private readonly IIdentityService _identityService = identityService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async Task<IActionResult> Handle(LogOutCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var logOutResponse = await _identityService.LogOut(userId!);

        return !logOutResponse.Succeeded
            ? new BadRequestObjectResult(new
            {
                message = logOutResponse.Message,
                succeded = logOutResponse.Succeeded,
                errors = logOutResponse.Errors
            })
            : new OkObjectResult(new
            {
                data = new
                {
                    logOutResponse.Title,
                    logOutResponse.Message,
                    logOutResponse.Succeeded,
                    logOutResponse.Response
                }
            });
    }
}
