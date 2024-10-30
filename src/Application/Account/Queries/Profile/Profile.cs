using System.Security.Claims;
using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceAPI.Application.Account.Queries.Profile;

[Authorize]
public record ProfileQuery : IRequest<IActionResult>;

public class ProfileQueryHandler : IRequestHandler<ProfileQuery, IActionResult>
{
    private readonly IAccountService _accountService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ProfileQueryHandler(IAccountService accountService, IHttpContextAccessor httpContextAccessor)
    {
        _accountService = accountService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Handle(ProfileQuery request, CancellationToken cancellationToken)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var user = httpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;


        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("Invalid request");

        var profileResponse = await _accountService.ProfileAsync(userId);
        if (!profileResponse.Succeeded)
        {
            _httpContextAccessor.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return new BadRequestObjectResult(new
            {
                error = profileResponse.Errors
            });
        }

        return new OkObjectResult(new
        {
            data = profileResponse
        });
    }
}
