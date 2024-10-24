#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Application.User.Commands.SendOTP;
public class SendOTPResponse
{
    public bool Succeeded { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Code { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ExpiryDate { get; set; } = DateTime.Now.AddMinutes(5);
}
