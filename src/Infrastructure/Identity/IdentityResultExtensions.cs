using RBACAPI.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace RBACAPI.Infrastructure.Identity;

public static class IdentityResultExtensions
{
    public static Result ToApplicationResult(this IdentityResult result)
    {
        return result.Succeeded
            ? Result.Success(new {})
            : Result.Failure(result.Errors.Select(e => e.Description));
    }
}
