#nullable disable
namespace RBACAPI.Application.OAuth.Commands.FacebookSignIn;
public class FacebookSignInResponse
{
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public List<string> Errors { get; init; }
}
