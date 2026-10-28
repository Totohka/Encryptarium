using System.Text.Json.Serialization;

namespace Encryptarium.Domain.DTO.Auth;

public class TokenDto
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refreshToken")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("expiresIn")]
    public DateTime ExpiresIn { get; set; }
}
