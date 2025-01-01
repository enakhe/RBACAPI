#nullable disable

namespace EcommerceAPI.Application.OAuth;
public class CreateUserFromSocialLogin
{
    public string Email { get; set; }
    public string LoginProviderSubject { get; set; }
}
