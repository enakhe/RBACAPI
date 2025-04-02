using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBACAPI.Application.Common.Models;
public class AuthResult
{
    internal AuthResult(bool succeeded, IEnumerable<string> errors, string accessToken, string refreshToken)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public bool Succeeded { get; init; }
    public string[] Errors { get; init; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public static AuthResult Success(string accessToken, string refreshToken)
    {
        return new AuthResult(true, Array.Empty<string>(), accessToken, refreshToken);
    }

    public static AuthResult Failure(IEnumerable<string> errors)
    {
        return new AuthResult(false, errors, String.Empty, String.Empty);
    }
}
