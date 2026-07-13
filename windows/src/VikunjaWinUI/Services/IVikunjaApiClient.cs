using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VikunjaWinUI.Models;

namespace VikunjaWinUI.Services;

/// <summary>Thin typed wrapper over the Vikunja /api/v2 REST surface.</summary>
public interface IVikunjaApiClient
{
    /// <summary>Exchanges credentials for a JWT. Does not require an existing session.</summary>
    Task<string> LoginAsync(string serverUrl, LoginRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<TaskItem>> GetTasksAsync(bool includeDone, CancellationToken ct = default);

    Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken ct = default);

    /// <summary>Persists an edited task (PUT /api/v2/tasks/{id}) and returns the server copy.</summary>
    Task<TaskItem> UpdateTaskAsync(TaskItem task, CancellationToken ct = default);
}
