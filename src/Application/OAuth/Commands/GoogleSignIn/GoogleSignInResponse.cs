#nullable disable

namespace EcommerceAPI.Application.OAuth.Commands.GoogleSignIn;
public class GoogleSignInResponse
{
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
}
