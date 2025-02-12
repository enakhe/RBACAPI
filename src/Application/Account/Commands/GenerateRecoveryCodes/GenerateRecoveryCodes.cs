using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RBACAPI.Application.Common.Interfaces;

namespace RBACAPI.Application.Account.Commands.GenerateRecoveryCodes;

public record GenerateRecoveryCodesCommand : IRequest<IActionResult>;

public class GenerateRecoveryCodesCommandValidator : AbstractValidator<GenerateRecoveryCodesCommand>
{
    public GenerateRecoveryCodesCommandValidator()
    {
    }
}

public class GenerateRecoveryCodesCommandHandler : IRequestHandler<GenerateRecoveryCodesCommand, IActionResult>
{
    private readonly IAccountService _accountService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GenerateRecoveryCodesCommandHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(GenerateRecoveryCodesCommand request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("We couldn’t get your recovery codes. Please ensure you're authenticated");

        var getRecoveryCodesResponse = await _accountService.GenerateRecoveryCodesAsync(userId);
        if (!getRecoveryCodesResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = getRecoveryCodesResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            response = getRecoveryCodesResponse
        });
    }
}
