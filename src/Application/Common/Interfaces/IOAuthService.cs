using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.OAuth.Commands.GoogleSignIn;

namespace EcommerceAPI.Application.Common.Interfaces;
public interface IOAuthService
{
    Task<GoogleSignInResponse> GoogleSignIn(string code);
}
