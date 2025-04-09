using Microsoft.AspNetCore.Http;
using RBACAPI.Application.Common.Models;
using RBACAPI.Domain.Enums;

namespace RBACAPI.Application.Common.Interfaces;
public interface IAccountService
{
    Task<Result> ProfileAsync(string userId);
    Task<Result> ChangePassword(string userId, string password, string confirmPassword);
    Task<Result> ChangeEmail(string userId, string email);
    Task<Result> GenerateRecoveryCodesAsync(string userId);
    Task<Result> EnableAuthenticator(string userId);
    Task<Result> Disable2FAuthentication(string userId);
    Task<Result> UpdateProfileAsync(string userId, string firstName, string lastName, IFormFile file, GenderData gender, string email, string phoneNumber);
}
