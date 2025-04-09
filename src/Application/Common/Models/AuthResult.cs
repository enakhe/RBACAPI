namespace RBACAPI.Application.Common.Models;
public class AuthResult
{
    internal AuthResult(bool succeeded, IEnumerable<string> errors, TokenResponse token, string title, string message)
    {
        Succeeded = succeeded;
        Errors = [.. errors];
        Response = token;
        Title = title;
        Message = message;
    }

    public bool Succeeded { get; init; }
    public string[] Errors { get; init; }
    public TokenResponse Response { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public static AuthResult Success(string title, string message, TokenResponse token)
    {
        return new AuthResult(true, [], token, title, message);
    }

    public static AuthResult Failure(string message, IEnumerable<string> errors)
    {
        return new AuthResult(false, errors, new TokenResponse(), String.Empty, message);
    }
}
