using System.Text.Json.Serialization;

namespace MrCheater.Domain.Models;

public class GameProfile
{
    [JsonPropertyName("formatVersion")]
    public int FormatVersion { get; set; } = 1;

    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    [JsonPropertyName("name")]
    public string Name { get; set; } = "New Game Profile";

    [JsonPropertyName("gameCode")]
    public string? GameCode { get; set; }

    [JsonPropertyName("emulator")]
    public string EmulatorName { get; set; } = "PCSX2";

    [JsonPropertyName("process")]
    public string TargetProcess { get; set; } = "pcsx2-qt.exe";

    [JsonPropertyName("windowTitlePattern")]
    public string? WindowTitlePattern { get; set; }

    [JsonPropertyName("inputProfile")]
    public string InputProfileName { get; set; } = "PS2 Keyboard Mapping";

    [JsonPropertyName("inputMapping")]
    public InputMappingProfile InputMapping { get; set; } = InputMappingProfile.CreateDefaultPs2Mapping();

    [JsonPropertyName("cheats")]
    public List<CheatDefinition> Cheats { get; set; } = new();

    [JsonPropertyName("macros")]
    public List<MacroDefinition> Macros { get; set; } = new();

    [JsonPropertyName("defaultStepDelayMs")]
    public int DefaultStepDelayMs { get; set; } = 70;

    [JsonPropertyName("defaultHoldDurationMs")]
    public int DefaultHoldDurationMs { get; set; } = 70;

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    public GameProfile Clone()
    {
        return new GameProfile
        {
            FormatVersion = FormatVersion,
            Id = Guid.NewGuid().ToString("N"),
            Name = Name + " (Copy)",
            GameCode = GameCode,
            EmulatorName = EmulatorName,
            TargetProcess = TargetProcess,
            WindowTitlePattern = WindowTitlePattern,
            InputProfileName = InputProfileName,
            InputMapping = InputMapping.Clone(),
            Cheats = Cheats.Select(c => c.Clone()).ToList(),
            Macros = Macros.Select(m => m.Clone()).ToList(),
            DefaultStepDelayMs = DefaultStepDelayMs,
            DefaultHoldDurationMs = DefaultHoldDurationMs,
            Notes = Notes
        };
    }
}
