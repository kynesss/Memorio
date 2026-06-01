using System.Text.Json.Serialization;

namespace Memorio.Users.Application.Contracts;

public sealed record AccessTokenResponse(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("token_type")] string TokenType,
    [property: JsonPropertyName("expires_in")] int ExpiresInSeconds);
