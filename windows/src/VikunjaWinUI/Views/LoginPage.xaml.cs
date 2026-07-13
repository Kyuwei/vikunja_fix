using Microsoft.UI.Xaml.Controls;
using VikunjaWinUI.ViewModels;
using VikunjaWinUI.Views;

namespace VikunjaWinUI.Views;

public sealed partial class LoginPage : Page
{
    public LoginViewModel ViewModel { get; }

    public LoginPage()
    {
        ViewModel = App.GetService<LoginViewModel>();
        InitializeComponent();

        ViewModel.LoginSucceeded += OnLoginSucceeded;
    }

    private void OnLoginSucceeded(object? sender, System.EventArgs e)
    {
        if (App.Current.MainWindow is MainWindow window)
        {
            window.NavigateRoot(typeof(TaskListPage));
        }
    }

    // PasswordBox.Password is deliberately not a bindable dependency property,
    // so the value is pushed into the view model on change instead.
    private void OnPasswordChanged(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ViewModel.Password = PasswordInput.Password;
    }
}
