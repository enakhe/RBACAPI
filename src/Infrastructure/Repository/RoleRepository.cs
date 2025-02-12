using Microsoft.AspNetCore.Identity;
using RBACAPI.Application.Common.Interfaces;
using RBACAPI.Application.Common.Models;
using RBACAPI.Infrastructure.Identity;

namespace RBACAPI.Infrastructure.Repository;
public class RoleRepository : IRoleRepository
{
    private readonly RoleManager<IdentityRole> _roleManager;

    private RoleRepository(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Result> CreateRole(string name)
    {
        var role = new ApplicationRole()
        {
            Name = name,
            NormalizedName = name
        };

        var result = await _roleManager.CreateAsync(role);
        return !result.Succeeded
            ? Result.Failure(result.Errors.Select(e => e.Description))
            : Result.Success(new { role, message = "Role created successfully" });
    }
}
