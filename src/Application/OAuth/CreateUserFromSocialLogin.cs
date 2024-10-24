#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Application.OAuth;
public class CreateUserFromSocialLogin
{
    public string Email { get; set; }
    public string LoginProviderSubject { get; set; }
}
