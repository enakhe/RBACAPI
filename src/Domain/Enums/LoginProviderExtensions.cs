namespace RBACAPI.Domain.Enums;
public static class LoginProviderExtensions
{
    public static string GetDisplayName(this LoginProvider provider)
    {
        return provider switch
        {
            LoginProvider.Google => "Google",
            LoginProvider.Facebook => "Facebook",
            _ => "Unknown"
        };
    }
}
