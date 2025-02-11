namespace RBACAPI.Application.Auth.EventHandlers;
public class AuthResponse
{
    public string? Token { get; set; }
    public string Message { get; set; } = string.Empty;
}
