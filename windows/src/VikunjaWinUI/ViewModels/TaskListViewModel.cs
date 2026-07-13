using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VikunjaWinUI.Models;
using VikunjaWinUI.Services;

namespace VikunjaWinUI.ViewModels;

public sealed partial class TaskListViewModel : ObservableObject
{
    private readonly IVikunjaApiClient _api;
    private readonly AuthSession _session;

    public TaskListViewModel(IVikunjaApiClient api, AuthSession session)
    {
        _api = api;
        _session = session;
    }

    public ObservableCollection<TaskItem> Tasks { get; } = new();

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _includeDone;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public bool IsEmpty => !IsBusy && Tasks.Count == 0;

    public string ServerLabel => _session.ServerUrl;

    /// <summary>Raised after the session is cleared so the shell can show login.</summary>
    public event EventHandler? SignedOut;

    partial void OnErrorMessageChanged(string value) => OnPropertyChanged(nameof(HasError));

    partial void OnIsBusyChanged(bool value) => OnPropertyChanged(nameof(IsEmpty));

    partial void OnIncludeDoneChanged(bool value) => _ = RefreshAsync();

    [RelayCommand]
    private async Task RefreshAsync()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var tasks = await _api.GetTasksAsync(IncludeDone);
            Tasks.Clear();
            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }
        catch (VikunjaApiException ex) when (ex.IsUnauthorized)
        {
            // The stored token expired — send the user back to sign in.
            SignOut();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    /// <summary>
    /// Persists a task whose <see cref="TaskItem.Done"/> was just toggled by the
    /// check box (two-way bound). On failure the toggle is reverted so the UI
    /// never drifts from the server.
    /// </summary>
    [RelayCommand]
    private async Task PersistTaskAsync(TaskItem task)
    {
        try
        {
            await _api.UpdateTaskAsync(task);

            // When hiding completed tasks, a freshly-done task leaves the list.
            if (!IncludeDone && task.Done)
            {
                Tasks.Remove(task);
                OnPropertyChanged(nameof(IsEmpty));
            }
        }
        catch (Exception ex)
        {
            task.Done = !task.Done;
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private void SignOut()
    {
        _session.SignOut();
        Tasks.Clear();
        SignedOut?.Invoke(this, EventArgs.Empty);
    }
}
