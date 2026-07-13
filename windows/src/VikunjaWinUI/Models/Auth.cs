using System.Text.Json.Serialization;

namespace VikunjaWinUI.Models;

/// <summary>Body of POST /api/v2/login.</summary>
public sealed class LoginRequest
{
    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; set; } = string.Empty;

    // Only sent when the account has TOTP enabled.
    [JsonPropertyName("totp_passcode")]
    public string TotpPasscode { get; set; } = string.Empty;

    // "Remember me" — asks the server for a longer-lived token.
    [JsonPropertyName("long_token")]
    public bool LongToken { get; set; }
}

/// <summary>Response of POST /api/v2/login: the short-lived JWT.</summary>
public sealed class TokenResponse
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}
