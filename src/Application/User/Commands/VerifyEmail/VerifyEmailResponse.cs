#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Application.User.Commands.VerifyEmail;
public class VerifyEmailResponse
{
    public bool IsValid { get; set; }
    public string UserId { get; set; }
    public string Message { get; set; }
}
