using EcommerceAPI.Application.Common.Models;

namespace EcommerceAPI.Application.Common.Interfaces;
public interface IAccountService
{
    Task<Result> ProfileAsync(string userId);
    Task<Result> GenerateRecoveryCodesAsync(string userId);
    Task<Result> EnableAuthenticator(string userId);
    Task<Result> Disable2FAuthentication(string userId);
}
