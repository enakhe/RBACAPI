using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IRoleRepository
{
    Task<Result> CreateRole(string name);
}
