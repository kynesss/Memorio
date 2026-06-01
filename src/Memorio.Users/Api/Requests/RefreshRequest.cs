using System.Text.Json.Serialization;

namespace Memorio.Users.Api.Requests;

public sealed record RefreshRequest([property: JsonPropertyName("refresh_token")] string RefreshToken);
