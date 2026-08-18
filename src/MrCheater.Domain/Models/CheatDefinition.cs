using System.Text.Json.Serialization;

namespace MrCheater.Domain.Models;

public class CheatDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = "General";

    [JsonPropertyName("sequence")]
    public List<SequenceStep> Sequence { get; set; } = new();

    [JsonPropertyName("delay_ms")]
    public int DefaultDelayMs { get; set; } = 70;

    [JsonPropertyName("requires_master_code")]
    public bool RequiresMasterCode { get; set; }

    [JsonPropertyName("prerequisites")]
    public List<string> PrerequisiteCheatIds { get; set; } = new();

    [JsonPropertyName("hotkey")]
    public string? AssignedHotkey { get; set; }

    [JsonPropertyName("isFavorite")]
    public bool IsFavorite { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    public CheatDefinition Clone()
    {
        return new CheatDefinition
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = Name + " (Copy)",
            Description = Description,
            Category = Category,
            Sequence = Sequence.Select(s => s.Clone()).ToList(),
            DefaultDelayMs = DefaultDelayMs,
            RequiresMasterCode = RequiresMasterCode,
            PrerequisiteCheatIds = new List<string>(PrerequisiteCheatIds),
            AssignedHotkey = AssignedHotkey,
            IsFavorite = IsFavorite,
            IsEnabled = IsEnabled
        };
    }
}
