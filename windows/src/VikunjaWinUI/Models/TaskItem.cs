using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace VikunjaWinUI.Models;

/// <summary>
/// A Vikunja task (GET /api/v2/tasks, PUT /api/v2/tasks/{id}). It is an
/// <see cref="ObservableObject"/> so toggling <see cref="Done"/> in the list
/// updates the UI immediately, before the server round-trip confirms it.
/// </summary>
public sealed partial class TaskItem : ObservableObject
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [ObservableProperty]
    [property: JsonPropertyName("title")]
    private string _title = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("description")]
    private string _description = string.Empty;

    [ObservableProperty]
    [property: JsonPropertyName("done")]
    private bool _done;

    [ObservableProperty]
    [property: JsonPropertyName("priority")]
    private int _priority;

    [ObservableProperty]
    [property: JsonPropertyName("percent_done")]
    private double _percentDone;

    [JsonPropertyName("due_date")]
    public DateTimeOffset DueDate { get; set; }

    [JsonPropertyName("project_id")]
    public long ProjectId { get; set; }

    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;

    [JsonPropertyName("is_favorite")]
    public bool IsFavorite { get; set; }

    // Vikunja serialises "no date" as year 1 (0001-01-01T00:00:00Z).
    [JsonIgnore]
    public bool HasDueDate => DueDate.Year > 1;

    [JsonIgnore]
    public string DueDateDisplay => HasDueDate ? DueDate.ToLocalTime().ToString("ddd, dd MMM") : string.Empty;

    [JsonIgnore]
    public string PriorityLabel => Priority switch
    {
        >= 5 => "DO NOW",
        4 => "Urgent",
        3 => "High",
        2 => "Medium",
        1 => "Low",
        _ => string.Empty,
    };

    [JsonIgnore]
    public bool HasPriority => Priority >= 3;
}
