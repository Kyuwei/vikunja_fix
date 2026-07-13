using Windows.Storage;

namespace VikunjaWinUI.Services;

/// <summary>
/// Persists preferences in the app's roaming-free local settings. Available
/// because the app runs packaged (MSIX); non-secret data only — the auth token
/// lives in <see cref="CredentialStore"/>.
/// </summary>
public sealed class SettingsService : ISettingsService
{
    private const string ServerUrlKey = "server_url";

    private static ApplicationDataContainer Local => ApplicationData.Current.LocalSettings;

    public string? ServerUrl
    {
        get => Local.Values.TryGetValue(ServerUrlKey, out var value) ? value as string : null;
        set
        {
            if (value is null)
            {
                Local.Values.Remove(ServerUrlKey);
            }
            else
            {
                Local.Values[ServerUrlKey] = value;
            }
        }
    }
}
