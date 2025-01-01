#nullable disable

namespace EcommerceAPI.Application.Common.Models;
public class OtpCookieData
{
    public string Otp { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}
