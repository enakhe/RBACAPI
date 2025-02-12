#nullable disable

using Microsoft.AspNetCore.Identity;
using RBACAPI.Domain.Enums;

namespace RBACAPI.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    public byte[] ProfilePicture { get; set; }

    public GenderData Gender { get; set; }

    public DateTime LastLoginDate { get; set; }
}
