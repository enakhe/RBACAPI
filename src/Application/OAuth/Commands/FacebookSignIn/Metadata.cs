#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
public class Metadata
{
    [JsonProperty("auth_type")]
    public string AuthType { get; set; }
}
