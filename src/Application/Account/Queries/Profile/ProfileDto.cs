namespace RBACAPI.Application.Account.Queries.Profile;
public class ProfileDto
{
    public string? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Gender { get; set; }
    public byte[]? ProfilePicture { get; set; }
    public DateTime LastLogin { get; set; }
    public bool EmailConfimed { get; set; }
}
