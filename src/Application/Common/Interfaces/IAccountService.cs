using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Models;

namespace EcommerceAPI.Application.Common.Interfaces;
public interface IAccountService
{
    Task<Result> ProfileAsync(string userId);
    Task<Result> GenerateRecoveryCodesAsync(string userId);
    Task<Result> EnableAuthenticator(string userId);
    Task<Result> Disable2FAuthentication(string userId);
}
