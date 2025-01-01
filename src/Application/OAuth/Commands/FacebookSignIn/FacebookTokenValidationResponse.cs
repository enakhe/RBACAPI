#nullable disable
using Newtonsoft.Json;

namespace EcommerceAPI.Application.OAuth.Commands.FacebookSignIn;
public class FacebookTokenValidationResponse
{
    [JsonProperty("data")]
    public FacebookTokenValidationData Data { get; set; }
}
