using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using VikunjaWinUI.Models;

namespace VikunjaWinUI.Services;

/// <summary>Raised when the API returns a non-success status.</summary>
public sealed class VikunjaApiException : Exception
{
    public VikunjaApiException(string message, HttpStatusCode statusCode) : base(message)
        => StatusCode = statusCode;

    public HttpStatusCode StatusCode { get; }

    public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized;
}

public sealed class VikunjaApiClient : IVikunjaApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient _http;
    private readonly AuthSession _session;

    public VikunjaApiClient(HttpClient http, AuthSession session)
    {
        _http = http;
        _session = session;
    }

    public async Task<string> LoginAsync(string serverUrl, LoginRequest request, CancellationToken ct = default)
    {
        var uri = new Uri(AuthSession.NormalizeServerUrl(serverUrl) + "/api/v2/login");
        using var message = new HttpRequestMessage(HttpMethod.Post, uri)
        {
            Content = JsonContent.Create(request),
        };

        using var response = await _http.SendAsync(message, ct).ConfigureAwait(false);
        await EnsureSuccessAsync(response, ct).ConfigureAwait(false);

        var token = await response.Content.ReadFromJsonAsync<TokenResponse>(JsonOptions, ct).ConfigureAwait(false);
        if (token is null || string.IsNullOrEmpty(token.Token))
        {
            throw new VikunjaApiException("The server did not return a token.", response.StatusCode);
        }

        return token.Token;
    }

    public async Task<IReadOnlyList<TaskItem>> GetTasksAsync(bool includeDone, CancellationToken ct = default)
    {
        // The v2 task collection accepts the same filter grammar as the web UI.
        var query = includeDone
            ? "tasks?per_page=100&sort_by=due_date&order_by=asc"
            : "tasks?per_page=100&sort_by=due_date&order_by=asc&filter=done%3Dfalse";

        var tasks = await GetJsonAsync<List<TaskItem>>(query, ct).ConfigureAwait(false);
        return tasks ?? new List<TaskItem>();
    }

    public async Task<IReadOnlyList<Project>> GetProjectsAsync(CancellationToken ct = default)
    {
        var projects = await GetJsonAsync<List<Project>>("projects?per_page=100", ct).ConfigureAwait(false);
        return projects ?? new List<Project>();
    }

    public async Task<TaskItem> UpdateTaskAsync(TaskItem task, CancellationToken ct = default)
    {
        using var message = CreateAuthorizedRequest(HttpMethod.Put, $"tasks/{task.Id}");
        message.Content = JsonContent.Create(task);

        using var response = await _http.SendAsync(message, ct).ConfigureAwait(false);
        await EnsureSuccessAsync(response, ct).ConfigureAwait(false);

        var updated = await response.Content.ReadFromJsonAsync<TaskItem>(JsonOptions, ct).ConfigureAwait(false);
        return updated ?? task;
    }

    private async Task<T?> GetJsonAsync<T>(string relativePath, CancellationToken ct)
    {
        using var message = CreateAuthorizedRequest(HttpMethod.Get, relativePath);
        using var response = await _http.SendAsync(message, ct).ConfigureAwait(false);
        await EnsureSuccessAsync(response, ct).ConfigureAwait(false);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions, ct).ConfigureAwait(false);
    }

    private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, string relativePath)
    {
        if (!_session.IsAuthenticated)
        {
            throw new VikunjaApiException("Not signed in.", HttpStatusCode.Unauthorized);
        }

        var message = new HttpRequestMessage(method, new Uri(_session.ApiBase, relativePath));
        message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.Token);
        return message;
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        // Vikunja errors are JSON { "message": "..." }; fall back to the raw body.
        var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        var message = TryExtractMessage(body) ?? $"Request failed ({(int)response.StatusCode} {response.ReasonPhrase}).";
        throw new VikunjaApiException(message, response.StatusCode);
    }

    private static string? TryExtractMessage(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("message", out var messageProp) &&
                messageProp.ValueKind == JsonValueKind.String)
            {
                return messageProp.GetString();
            }
        }
        catch (JsonException)
        {
            // Not JSON — caller uses the status-based fallback.
        }

        return null;
    }
}
