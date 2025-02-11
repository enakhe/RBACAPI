#nullable disable

using RBACAPI.Domain.Enums;
using Microsoft.AspNetCore.Identity;

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
