namespace RBACAPI.Application.Common.Interfaces;
public interface ICookieService
{
    void SetCookie(string data, string cookieName, DateTimeOffset date);
}
