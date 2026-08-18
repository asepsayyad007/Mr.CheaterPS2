using System.Text.Json.Serialization;

namespace MrCheater.Domain.Models;

public class EmulatorProfile
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("processNames")]
    public List<string> ProcessNames { get; set; } = new();

    [JsonPropertyName("defaultInputProfile")]
    public string DefaultInputProfile { get; set; } = "PS2 Keyboard Mapping";

    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    public static List<EmulatorProfile> CreateDefaultEmulators()
    {
        return new List<EmulatorProfile>
        {
            new EmulatorProfile
            {
                Name = "PCSX2",
                ProcessNames = new List<string> { "pcsx2-qt.exe", "pcsx2.exe", "pcsx2-wx.exe" },
                DefaultInputProfile = "PS2 Keyboard Mapping",
                Notes = "PlayStation 2 Emulator"
            },
            new EmulatorProfile
            {
                Name = "RPCS3",
                ProcessNames = new List<string> { "rpcs3.exe" },
                DefaultInputProfile = "PS3 Keyboard Mapping",
                Notes = "PlayStation 3 Emulator"
            },
            new EmulatorProfile
            {
                Name = "Dolphin",
                ProcessNames = new List<string> { "Dolphin.exe", "DolphinWx.exe" },
                DefaultInputProfile = "GameCube/Wii Keyboard Mapping",
                Notes = "GameCube & Wii Emulator"
            },
            new EmulatorProfile
            {
                Name = "DuckStation",
                ProcessNames = new List<string> { "duckstation-qt-x64-ReleaseLTCG.exe", "duckstation-nogui-x64-ReleaseLTCG.exe", "duckstation.exe" },
                DefaultInputProfile = "PS1 Keyboard Mapping",
                Notes = "PlayStation 1 Emulator"
            },
            new EmulatorProfile
            {
                Name = "RetroArch",
                ProcessNames = new List<string> { "retroarch.exe" },
                DefaultInputProfile = "RetroArch Keyboard Mapping",
                Notes = "Multi-system Frontend"
            },
            new EmulatorProfile
            {
                Name = "PPSSPP",
                ProcessNames = new List<string> { "PPSSPPWindows64.exe", "PPSSPPWindows.exe" },
                DefaultInputProfile = "PSP Keyboard Mapping",
                Notes = "PlayStation Portable Emulator"
            },
            new EmulatorProfile
            {
                Name = "Custom / Native Game",
                ProcessNames = new List<string>(),
                DefaultInputProfile = "Generic Keyboard Mapping",
                Notes = "Custom PC Game or Emulator"
            }
        };
    }
}
