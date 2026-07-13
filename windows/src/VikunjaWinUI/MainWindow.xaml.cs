using Microsoft.UI.Xaml;
using VikunjaWinUI.Services;
using VikunjaWinUI.Views;

namespace VikunjaWinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        var session = App.GetService<AuthSession>();
        session.TryRestore();

        // A restored token skips the login page and lands straight on the task
        // list; a genuine 401 later bounces the user back to LoginPage.
        if (session.IsAuthenticated)
        {
            RootFrame.Navigate(typeof(TaskListPage));
        }
        else
        {
            RootFrame.Navigate(typeof(LoginPage));
        }
    }

    /// <summary>Swaps the root content, clearing back-stack so signing out or in
    /// can't be undone with the back gesture.</summary>
    public void NavigateRoot(System.Type pageType)
    {
        RootFrame.Navigate(pageType);
        RootFrame.BackStack.Clear();
    }
}
