using System;

namespace VikunjaWinUI.Services;

/// <summary>
/// Holds the currently authenticated server + token and mediates their secure
/// persistence. Injected everywhere the API client and views need to know who
/// is signed in.
/// </summary>
public sealed class AuthSession
{
    // Credential-locker resource name; the server URL is the "user" so multiple
    // servers can be remembered independently.
    private const string TokenResource = "Vikunja.WinUI";

    private readonly ISettingsService _settings;
    private readonly ICredentialStore _credentials;

    public AuthSession(ISettingsService settings, ICredentialStore credentials)
    {
        _settings = settings;
        _credentials = credentials;
    }

    public string ServerUrl { get; private set; } = string.Empty;

    public string Token { get; private set; } = string.Empty;

    public bool IsAuthenticated => !string.IsNullOrEmpty(Token) && !string.IsNullOrEmpty(ServerUrl);

    /// <summary>Rehydrates a saved session at startup, if one exists.</summary>
    public void TryRestore()
    {
        ServerUrl = _settings.ServerUrl ?? string.Empty;
        Token = string.IsNullOrEmpty(ServerUrl)
            ? string.Empty
            : _credentials.Retrieve(TokenResource, ServerUrl) ?? string.Empty;
    }

    public void SignIn(string serverUrl, string token)
    {
        ServerUrl = NormalizeServerUrl(serverUrl);
        Token = token;
        _settings.ServerUrl = ServerUrl;
        _credentials.Store(TokenResource, ServerUrl, token);
    }

    public void SignOut()
    {
        if (!string.IsNullOrEmpty(ServerUrl))
        {
            _credentials.Remove(TokenResource, ServerUrl);
        }

        Token = string.Empty;
        // Keep ServerUrl in settings so the login form pre-fills it next time.
    }

    /// <summary>Base for v2 requests, e.g. https://try.vikunja.io/api/v2/.</summary>
    public Uri ApiBase => new(NormalizeServerUrl(ServerUrl) + "/api/v2/");

    public static string NormalizeServerUrl(string url)
        => url.Trim().TrimEnd('/');
}
