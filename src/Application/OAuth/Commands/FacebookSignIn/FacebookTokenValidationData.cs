﻿#nullable disable
using Newtonsoft.Json;

namespace RBACAPI.Application.OAuth.Commands.FacebookSignIn;
public class FacebookTokenValidationData
{
    [JsonProperty("app_id")]
    public string AppId { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("application")]
    public string Application { get; set; }

    [JsonProperty("data_access_expires_at")]
    public long DataAccessExpiresAt { get; set; }

    [JsonProperty("expires_at")]
    public long ExpiresAt { get; set; }

    [JsonProperty("is_valid")]
    public bool IsValid { get; set; }

    [JsonProperty("metadata")]
    public Metadata Metadata { get; set; }

    [JsonProperty("scopes")]
    public string[] Scopes { get; set; }

    [JsonProperty("user_id")]
    public string UserId { get; set; }
}
