using RBACAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IAccountService
{
    Task<Result> ProfileAsync(string userId);
    Task<Result> ChangePassword(string userId, string password, string confirmPassword);
    Task<Result> ChangeEmail(string userId, string email);
    Task<Result> GenerateRecoveryCodesAsync(string userId);
    Task<Result> EnableAuthenticator(string userId);
    Task<Result> Disable2FAuthentication(string userId);
}
