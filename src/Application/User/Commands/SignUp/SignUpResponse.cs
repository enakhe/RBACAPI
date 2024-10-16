#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Application.User.Commands.SignUp;
public class SignUpResponse
{
    public bool Succeeded { get; set; }
    public string Token { get; set; }
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
}
