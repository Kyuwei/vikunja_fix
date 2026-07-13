using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using VikunjaWinUI.Models;
using VikunjaWinUI.ViewModels;

namespace VikunjaWinUI.Views;

public sealed partial class TaskListPage : Page
{
    public TaskListViewModel ViewModel { get; }

    public TaskListPage()
    {
        ViewModel = App.GetService<TaskListViewModel>();
        InitializeComponent();

        ViewModel.SignedOut += OnSignedOut;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
        => await ViewModel.RefreshCommand.ExecuteAsync(null);

    private void OnSignedOut(object? sender, System.EventArgs e)
    {
        if (App.Current.MainWindow is MainWindow window)
        {
            window.NavigateRoot(typeof(LoginPage));
        }
    }

    // The check box's two-way binding has already flipped TaskItem.Done by the
    // time Click fires; the command only needs to persist and reconcile.
    private void OnTaskDoneClick(object sender, RoutedEventArgs e)
    {
        if (sender is CheckBox { DataContext: TaskItem task })
        {
            ViewModel.PersistTaskCommand.Execute(task);
        }
    }
}
