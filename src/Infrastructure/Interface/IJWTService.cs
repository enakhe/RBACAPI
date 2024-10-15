using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Infrastructure.Identity;
using Microsoft.AspNetCore.Http;

namespace EcommerceAPI.Infrastructure.Interface;
public interface IJWTService
{
    string GenerateJWTToken(HttpContext context, ApplicationUser user);
}
