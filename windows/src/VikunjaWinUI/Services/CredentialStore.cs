using System;
using Windows.Security.Credentials;

namespace VikunjaWinUI.Services;

/// <summary>
/// Wraps <see cref="PasswordVault"/>, the OS-managed credential locker. Secrets
/// are encrypted at rest by Windows and scoped to the signed-in user, which is
/// why the token is kept here instead of in app settings or on disk.
/// </summary>
public sealed class CredentialStore : ICredentialStore
{
    private readonly PasswordVault _vault = new();

    public void Store(string resource, string userName, string secret)
    {
        // Overwrite any previous secret for this (resource, user) pair.
        Remove(resource, userName);
        _vault.Add(new PasswordCredential(resource, userName, secret));
    }

    public string? Retrieve(string resource, string userName)
    {
        try
        {
            var credential = _vault.Retrieve(resource, userName);
            credential.RetrievePassword();
            return credential.Password;
        }
        catch (Exception)
        {
            // PasswordVault throws when nothing is stored for the pair.
            return null;
        }
    }

    public void Remove(string resource, string userName)
    {
        try
        {
            var credential = _vault.Retrieve(resource, userName);
            _vault.Remove(credential);
        }
        catch (Exception)
        {
            // Nothing stored — treat removal as a no-op.
        }
    }
}
