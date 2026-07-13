using System.Text.Json.Serialization;

namespace VikunjaWinUI.Models;

/// <summary>A Vikunja project as returned by GET /api/v2/projects.</summary>
public sealed class Project
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("hex_color")]
    public string HexColor { get; set; } = string.Empty;

    [JsonPropertyName("is_archived")]
    public bool IsArchived { get; set; }

    [JsonPropertyName("is_favorite")]
    public bool IsFavorite { get; set; }
}
