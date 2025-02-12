using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IRoleService
{
    Task<Result> CreateRole(string name);
}
