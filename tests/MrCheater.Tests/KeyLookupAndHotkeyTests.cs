using MrCheater.Domain.Enums;
using MrCheater.Infrastructure.Input;
using MrCheater.Infrastructure.System;
using Xunit;

namespace MrCheater.Tests;

public class KeyLookupAndHotkeyTests
{
    [Theory]
    [InlineData("Up", 0x26, 0x48)]
    [InlineData("Down", 0x28, 0x50)]
    [InlineData("Left", 0x25, 0x4B)]
    [InlineData("Right", 0x27, 0x4D)]
    [InlineData("Escape", 0x1B, 0x01)]
    [InlineData("Space", 0x20, 0x39)]
    public void KeyLookup_ResolvesCorrectVirtualKeyAndScanCode(string keyName, ushort expectedVk, ushort expectedSc)
    {
        var (vk, sc, _) = KeyLookup.ResolveKey(keyName);
        Assert.Equal(expectedVk, vk);
        Assert.Equal(expectedSc, sc);
    }

    [Fact]
    public void ParseHotkey_CorrectlyExtractsModifiersAndKey()
    {
        var (mods, vk) = WindowsGlobalHotkeyManager.ParseHotkey("Ctrl+Shift+Escape");
        Assert.True((mods & HotkeyModifier.Control) != 0);
        Assert.True((mods & HotkeyModifier.Shift) != 0);
        Assert.Equal((uint)0x1B, vk); // VK_ESCAPE = 0x1B

        var (mods2, vk2) = WindowsGlobalHotkeyManager.ParseHotkey("F1");
        Assert.Equal(HotkeyModifier.None, mods2);
        Assert.Equal((uint)0x70, vk2); // VK_F1 = 0x70

        var (mods3, vk3) = WindowsGlobalHotkeyManager.ParseHotkey("Alt+F2");
        Assert.True((mods3 & HotkeyModifier.Alt) != 0);
        Assert.Equal((uint)0x71, vk3); // VK_F2 = 0x71
    }
}
