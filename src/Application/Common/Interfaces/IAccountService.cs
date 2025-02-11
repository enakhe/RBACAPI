using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IAccountService
{
    Task<Result> ProfileAsync(string userId);
    Task<Result> GenerateRecoveryCodesAsync(string userId);
    Task<Result> EnableAuthenticator(string userId);
    Task<Result> Disable2FAuthentication(string userId);
}
