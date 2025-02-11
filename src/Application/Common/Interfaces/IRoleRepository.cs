using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcommerceAPI.Application.Common.Models;

namespace RBACAPI.Application.Common.Interfaces;
public interface IRoleRepository
{
    Task<Result> CreateRole(string name);
}
