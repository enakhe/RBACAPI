#nullable disable
using Newtonsoft.Json;

namespace EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
public class Metadata
{
    [JsonProperty("auth_type")]
    public string AuthType { get; set; }
}
