using Microsoft.AspNetCore.Identity;
using RBACAPI.Application.Common.Models;

namespace RBACAPI.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success(new { })
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
