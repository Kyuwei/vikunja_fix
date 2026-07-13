namespace VikunjaWinUI.Services;

/// <summary>Secure storage for secrets (the JWT), backed by the Windows
/// Credential Locker rather than plaintext settings or files.</summary>
public interface ICredentialStore
{
    void Store(string resource, string userName, string secret);

    string? Retrieve(string resource, string userName);

    void Remove(string resource, string userName);
}
