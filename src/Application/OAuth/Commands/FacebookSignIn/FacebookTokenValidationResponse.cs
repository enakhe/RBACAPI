#nullable disable
using Newtonsoft.Json;

namespace RBACAPI.Application.OAuth.Commands.FacebookSignIn;
public class FacebookTokenValidationResponse
{
    [JsonProperty("data")]
    public FacebookTokenValidationData Data { get; set; }
}
