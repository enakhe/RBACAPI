using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommerceAPI.Application.Auth.EventHandlers;
public class AuthResponse
{
    public string? Token { get; set; }
    public string Message { get; set; } = string.Empty;
}
