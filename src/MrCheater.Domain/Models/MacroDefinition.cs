using System.Text.Json.Serialization;

namespace MrCheater.Domain.Models;

public class MacroDefinition
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("sequence")]
    public List<SequenceStep> Sequence { get; set; } = new();

    [JsonPropertyName("loopCount")]
    public int LoopCount { get; set; } = 1;

    [JsonPropertyName("hotkey")]
    public string? AssignedHotkey { get; set; }

    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; } = true;

    public MacroDefinition Clone()
    {
        return new MacroDefinition
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = Name + " (Copy)",
            Description = Description,
            Sequence = Sequence.Select(s => s.Clone()).ToList(),
            LoopCount = LoopCount,
            AssignedHotkey = AssignedHotkey,
            IsEnabled = IsEnabled
        };
    }
}
