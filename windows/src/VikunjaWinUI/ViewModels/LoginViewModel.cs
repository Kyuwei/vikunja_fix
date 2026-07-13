using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VikunjaWinUI.Models;
using VikunjaWinUI.Services;

namespace VikunjaWinUI.ViewModels;

public sealed partial class LoginViewModel : ObservableObject
{
    private readonly IVikunjaApiClient _api;
    private readonly AuthSession _session;

    public LoginViewModel(IVikunjaApiClient api, AuthSession session)
    {
        _api = api;
        _session = session;

        // Pre-fill the last server the user signed in to, if any.
        _serverUrl = string.IsNullOrEmpty(session.ServerUrl) ? "https://" : session.ServerUrl;
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _serverUrl;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _totpPasscode = string.Empty;

    [ObservableProperty]
    private bool _rememberMe = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private bool _isBusy;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    /// <summary>Raised once a token has been obtained and the session saved.</summary>
    public event EventHandler? LoginSucceeded;

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    private bool CanLogin() =>
        !IsBusy &&
        !string.IsNullOrWhiteSpace(ServerUrl) &&
        ServerUrl.Trim() != "https://" &&
        !string.IsNullOrWhiteSpace(Username) &&
        !string.IsNullOrWhiteSpace(Password);

    [RelayCommand(CanExecute = nameof(CanLogin))]
    private async Task LoginAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var request = new LoginRequest
            {
                Username = Username.Trim(),
                Password = Password,
                TotpPasscode = TotpPasscode.Trim(),
                LongToken = RememberMe,
            };

            var token = await _api.LoginAsync(ServerUrl, request);
            _session.SignIn(ServerUrl, token);
            LoginSucceeded?.Invoke(this, EventArgs.Empty);
        }
        catch (VikunjaApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not reach the server: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
