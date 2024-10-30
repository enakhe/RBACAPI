using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Interfaces;
using EcommerceAPI.Application.Common.Models;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EcommerceAPI.Infrastructure.Repository;
public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    public AccountService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result> ProfileAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            IEnumerable<string> errors = new List<string> { "Invalid login attempt" };
            return Result.Failure(errors);
        }

        return Result.Success(new
        {
            user.Id,
            user.FirstName,
            user.LastName,
            user.FullName,
            user.UserName,
            user.Email,
            user.PhoneNumber,
            user.Gender,
            user.ProfilePicture,
            user.LastLoginDate,
            user.EmailConfirmed
        });
    }
}
