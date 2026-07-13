namespace VikunjaWinUI.Services;

/// <summary>Small key/value persistence for non-secret preferences.</summary>
public interface ISettingsService
{
    /// <summary>The Vikunja server base URL the user last signed in to.</summary>
    string? ServerUrl { get; set; }
}
