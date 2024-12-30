#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
public class FacebookTokenValidationResponse
{
    [JsonProperty("data")]
    public FacebookTokenValidationData Data { get; set; }
}
