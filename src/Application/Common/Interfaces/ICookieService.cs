using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RBACAPI.Application.Common.Interfaces;
public interface ICookieService
{
    void SetCookie(string data, string cookieName);
}
