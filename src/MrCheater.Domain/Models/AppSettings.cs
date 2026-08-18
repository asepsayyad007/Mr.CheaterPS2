using System.Text.Json.Serialization;

namespace MrCheater.Domain.Models;

public class AppSettings
{
    [JsonPropertyName("formatVersion")]
    public int FormatVersion { get; set; } = 1;

    [JsonPropertyName("requireTargetProcessRunning")]
    public bool RequireTargetProcessRunning { get; set; } = true;

    [JsonPropertyName("requireTargetWindowFocus")]
    public bool RequireTargetWindowFocus { get; set; } = false;

    [JsonPropertyName("allowBackgroundInput")]
    public bool AllowBackgroundInput { get; set; } = true;

    [JsonPropertyName("emergencyStopHotkey")]
    public string EmergencyStopHotkey { get; set; } = "Ctrl+Shift+Escape";

    [JsonPropertyName("maxSequenceDurationSeconds")]
    public int MaxSequenceDurationSeconds { get; set; } = 15;

    [JsonPropertyName("defaultGlobalDelayMs")]
    public int DefaultGlobalDelayMs { get; set; } = 70;

    [JsonPropertyName("defaultHoldDurationMs")]
    public int DefaultHoldDurationMs { get; set; } = 70;

    [JsonPropertyName("minimizeToTray")]
    public bool MinimizeToTray { get; set; } = true;

    [JsonPropertyName("startWithWindows")]
    public bool StartWithWindows { get; set; } = false;

    [JsonPropertyName("startMinimized")]
    public bool StartMinimized { get; set; } = false;

    [JsonPropertyName("enableToastNotifications")]
    public bool EnableToastNotifications { get; set; } = true;

    [JsonPropertyName("enableSoundEffects")]
    public bool EnableSoundEffects { get; set; } = true;

    [JsonPropertyName("selectedProfileId")]
    public string? SelectedProfileId { get; set; }

    [JsonPropertyName("isFirstRunCompleted")]
    public bool IsFirstRunCompleted { get; set; } = false;

    [JsonPropertyName("preferredTheme")]
    public string PreferredTheme { get; set; } = "Dark";
}
