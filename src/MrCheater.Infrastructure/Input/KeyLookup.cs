namespace MrCheater.Infrastructure.Input;

public static class KeyLookup
{
    private static readonly Dictionary<string, (ushort vk, ushort sc, bool extended)> KeyMap = new(StringComparer.OrdinalIgnoreCase);

    static KeyLookup()
    {
        // Letters A-Z with standard hardware scancodes
        AddKey("A", 0x41, 0x1E, false);
        AddKey("B", 0x42, 0x30, false);
        AddKey("C", 0x43, 0x2E, false);
        AddKey("D", 0x44, 0x20, false);
        AddKey("E", 0x45, 0x12, false);
        AddKey("F", 0x46, 0x21, false);
        AddKey("G", 0x47, 0x22, false);
        AddKey("H", 0x48, 0x23, false);
        AddKey("I", 0x49, 0x17, false); // Triangle △ mapping in PS2
        AddKey("J", 0x4A, 0x24, false); // Square ▢ mapping in PS2
        AddKey("K", 0x4B, 0x25, false); // Cross ✕ mapping in PS2
        AddKey("L", 0x4C, 0x26, false);
        AddKey("M", 0x4D, 0x32, false);
        AddKey("N", 0x4E, 0x31, false);
        AddKey("O", 0x4F, 0x18, false); // Circle ◯ mapping in PS2
        AddKey("P", 0x50, 0x19, false);
        AddKey("Q", 0x51, 0x10, false); // L1 Bumper
        AddKey("R", 0x52, 0x13, false);
        AddKey("S", 0x53, 0x1F, false);
        AddKey("T", 0x54, 0x14, false);
        AddKey("U", 0x55, 0x16, false);
        AddKey("V", 0x56, 0x2F, false);
        AddKey("W", 0x57, 0x11, false);
        AddKey("X", 0x58, 0x2D, false);
        AddKey("Y", 0x59, 0x15, false);
        AddKey("Z", 0x5A, 0x2C, false);

        // Digits 0-9
        AddKey("1", 0x31, 0x02, false);
        AddKey("2", 0x32, 0x03, false);
        AddKey("3", 0x33, 0x04, false);
        AddKey("4", 0x34, 0x05, false);
        AddKey("5", 0x35, 0x06, false);
        AddKey("6", 0x36, 0x07, false);
        AddKey("7", 0x37, 0x08, false);
        AddKey("8", 0x38, 0x09, false);
        AddKey("9", 0x39, 0x0A, false);
        AddKey("0", 0x30, 0x0B, false);

        // Function keys F1-F12
        for (int i = 1; i <= 12; i++)
        {
            ushort vk = (ushort)(0x70 + (i - 1));
            ushort sc = (ushort)(0x3B + (i - 1));
            AddKey($"F{i}", vk, sc, false);
        }

        // Standard navigation & control keys (Extended = true for Arrow keys in hardware scancode)
        AddKey("Up", 0x26, 0x48, true);
        AddKey("Down", 0x28, 0x50, true);
        AddKey("Left", 0x25, 0x4B, true);
        AddKey("Right", 0x27, 0x4D, true);

        AddKey("UpArrow", 0x26, 0x48, true);
        AddKey("DownArrow", 0x28, 0x50, true);
        AddKey("LeftArrow", 0x25, 0x4B, true);
        AddKey("RightArrow", 0x27, 0x4D, true);

        AddKey("Space", 0x20, 0x39, false);
        AddKey("Enter", 0x0D, 0x1C, false);
        AddKey("Return", 0x0D, 0x1C, false);
        AddKey("Escape", 0x1B, 0x01, false);
        AddKey("Esc", 0x1B, 0x01, false);
        AddKey("Tab", 0x09, 0x0F, false);
        AddKey("Backspace", 0x08, 0x0E, false);
        AddKey("Back", 0x08, 0x0E, false);

        AddKey("Shift", 0x10, 0x2A, false);
        AddKey("LeftShift", 0xA0, 0x2A, false);
        AddKey("RightShift", 0xA1, 0x36, false);

        AddKey("Control", 0x11, 0x1D, false);
        AddKey("Ctrl", 0x11, 0x1D, false);
        AddKey("LeftCtrl", 0xA2, 0x1D, false);
        AddKey("RightCtrl", 0xA3, 0x1D, true);

        AddKey("Alt", 0x12, 0x38, false);
        AddKey("LeftAlt", 0xA4, 0x38, false);
        AddKey("RightAlt", 0xA5, 0x38, true);

        AddKey("Insert", 0x2D, 0x52, true);
        AddKey("Delete", 0x2E, 0x53, true);
        AddKey("Home", 0x24, 0x47, true);
        AddKey("End", 0x23, 0x4F, true);
        AddKey("PageUp", 0x21, 0x49, true);
        AddKey("PageDown", 0x22, 0x51, true);

        // Numpad
        for (int i = 0; i <= 9; i++)
        {
            AddKey($"NumPad{i}", (ushort)(0x60 + i), 0, false);
            AddKey($"Num{i}", (ushort)(0x60 + i), 0, false);
        }
        AddKey("NumMultiply", 0x6A, 0x37, false);
        AddKey("NumAdd", 0x6B, 0x4E, false);
        AddKey("NumSubtract", 0x6D, 0x4A, false);
        AddKey("NumDecimal", 0x6E, 0x53, false);
        AddKey("NumDivide", 0x6F, 0x35, true);
    }

    private static void AddKey(string name, ushort vk, ushort sc, bool extended)
    {
        KeyMap[name] = (vk, sc, extended);
    }

    public static (ushort vk, ushort sc, bool extended) ResolveKey(string keyName)
    {
        if (string.IsNullOrWhiteSpace(keyName))
            return (0, 0, false);

        keyName = keyName.Trim();

        if (KeyMap.TryGetValue(keyName, out var info))
            return info;

        if (keyName.Length == 1)
        {
            char c = char.ToUpperInvariant(keyName[0]);
            if (KeyMap.TryGetValue(c.ToString(), out var charInfo))
                return charInfo;

            if ((c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9'))
                return ((ushort)c, 0, false);
        }

        return (0, 0, false);
    }

    public static IEnumerable<string> GetAllSupportedKeyNames()
    {
        return KeyMap.Keys.Distinct().OrderBy(k => k);
    }
}
