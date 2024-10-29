using System.Security.Claims;
using EcommerceAPI.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.Auth.Commands.LogOut;

public record LogOutCommand : IRequest<IActionResult>
{
}

public class LogOutCommandValidator : AbstractValidator<LogOutCommand>
{
    public LogOutCommandValidator()
    {
    }
}

public class LogOutCommandHandler : IRequestHandler<LogOutCommand, IActionResult>
{
    private readonly IIdentityService _identityService;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public LogOutCommandHandler(IIdentityService identityService, IHttpContextAccessor httpContextAccessor)
    {
        _identityService = identityService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(LogOutCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var userId = httpContext!.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var logOutResponse = await _identityService.LogOut(userId!);
        if(!logOutResponse.Succeeded)
            return new BadRequestObjectResult("Invalid  request");

        return new OkObjectResult(new
        {
            data = logOutResponse
        });
    }
}
