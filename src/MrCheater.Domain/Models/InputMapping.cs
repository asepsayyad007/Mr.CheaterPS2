using System.Text.Json.Serialization;

namespace MrCheater.Domain.Models;

public class InputMappingEntry
{
    [JsonPropertyName("actionName")]
    public string ActionName { get; set; } = string.Empty;

    [JsonPropertyName("displayLabel")]
    public string DisplayLabel { get; set; } = string.Empty;

    [JsonPropertyName("keyboardKey")]
    public string KeyboardKey { get; set; } = string.Empty;

    [JsonPropertyName("xInputButton")]
    public string XInputButton { get; set; } = string.Empty;

    [JsonPropertyName("virtualKey")]
    public ushort VirtualKey { get; set; }

    [JsonPropertyName("scanCode")]
    public ushort ScanCode { get; set; }

    public InputMappingEntry Clone()
    {
        return new InputMappingEntry
        {
            ActionName = ActionName,
            DisplayLabel = DisplayLabel,
            KeyboardKey = KeyboardKey,
            XInputButton = XInputButton,
            VirtualKey = VirtualKey,
            ScanCode = ScanCode
        };
    }
}

public class InputMappingProfile
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "ps2_default";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "PCSX2 Standard Keyboard Mapping";

    [JsonPropertyName("mappings")]
    public Dictionary<string, InputMappingEntry> Mappings { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public InputMappingProfile Clone()
    {
        var clone = new InputMappingProfile
        {
            Id = Id,
            Name = Name,
            Mappings = new Dictionary<string, InputMappingEntry>(StringComparer.OrdinalIgnoreCase)
        };

        foreach (var kvp in Mappings)
        {
            clone.Mappings[kvp.Key] = kvp.Value.Clone();
        }

        return clone;
    }

    public static InputMappingProfile CreateDefaultPs2Mapping()
    {
        var profile = new InputMappingProfile
        {
            Id = "ps2_default",
            Name = "PCSX2 Standard Keyboard Mapping",
            Mappings = new Dictionary<string, InputMappingEntry>(StringComparer.OrdinalIgnoreCase)
        };

        void Add(string action, string label, string key, string xbutton, ushort vk = 0, ushort sc = 0)
        {
            profile.Mappings[action] = new InputMappingEntry
            {
                ActionName = action,
                DisplayLabel = label,
                KeyboardKey = key,
                XInputButton = xbutton,
                VirtualKey = vk,
                ScanCode = sc
            };
        }

        // D-Pad
        Add("UP", "D-Pad Up (↑)", "Up", "DPadUp");
        Add("DOWN", "D-Pad Down (↓)", "Down", "DPadDown");
        Add("LEFT", "D-Pad Left (←)", "Left", "DPadLeft");
        Add("RIGHT", "D-Pad Right (→)", "Right", "DPadRight");

        // Face Buttons (PCSX2 Standard)
        Add("TRIANGLE", "Triangle (△)", "I", "Y");
        Add("CIRCLE", "Circle (◯)", "L", "B");
        Add("CROSS", "Cross (✕)", "K", "A");
        Add("SQUARE", "Square (▢)", "J", "X");

        // Shoulder & Triggers
        Add("L1", "L1 Bumper", "Q", "LeftShoulder");
        Add("L2", "L2 Trigger", "1", "LeftTrigger");
        Add("R1", "R1 Bumper", "E", "RightShoulder");
        Add("R2", "R2 Trigger", "3", "RightTrigger");

        // Center / System Buttons
        Add("SELECT", "Select", "Backspace", "Back");
        Add("START", "Start", "Return", "Start");

        // Thumb Clicks
        Add("L3", "L3 Stick Click", "2", "LeftThumb");
        Add("R3", "R3 Stick Click", "4", "RightThumb");

        // Left Analog Stick
        Add("L_UP", "Left Stick Up", "W", "ThumbLY+");
        Add("L_DOWN", "Left Stick Down", "S", "ThumbLY-");
        Add("L_LEFT", "Left Stick Left", "A", "ThumbLX-");
        Add("L_RIGHT", "Left Stick Right", "D", "ThumbLX+");

        // Right Analog Stick
        Add("R_UP", "Right Stick Up", "T", "ThumbRY+");
        Add("R_DOWN", "Right Stick Down", "G", "ThumbRY-");
        Add("R_LEFT", "Right Stick Left", "F", "ThumbRX-");
        Add("R_RIGHT", "Right Stick Right", "H", "ThumbRX+");

        return profile;
    }
}
