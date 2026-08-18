using System.Runtime.InteropServices;
using MrCheater.Domain.Interfaces;
using MrCheater.Infrastructure.Input;
using MrCheater.Infrastructure.Native;

namespace MrCheater.Infrastructure.System;

public class WindowsGlobalHotkeyManager : IGlobalHotkeyService
{
    private IntPtr _hwnd = IntPtr.Zero;
    private int _currentId = 1000;
    private readonly Dictionary<int, (string hotkey, Action callback)> _idToCallback = new();
    private readonly Dictionary<string, int> _hotkeyToId = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _lock = new();

    public void SetHwnd(IntPtr hwnd)
    {
        _hwnd = hwnd;
    }

    public bool RegisterHotkey(string hotkeyString, Action callback)
    {
        if (string.IsNullOrWhiteSpace(hotkeyString) || callback == null)
            return false;

        lock (_lock)
        {
            if (_hotkeyToId.TryGetValue(hotkeyString, out var existingId))
            {
                UnregisterHotkey(hotkeyString);
            }

            var (modifiers, vk) = ParseHotkey(hotkeyString);
            if (vk == 0)
                return false;

            if (_hwnd == IntPtr.Zero)
            {
                // Can still store registration for when hwnd is provided
                int idTemp = ++_currentId;
                _idToCallback[idTemp] = (hotkeyString, callback);
                _hotkeyToId[hotkeyString] = idTemp;
                return true;
            }

            int id = ++_currentId;
            uint fsModifiers = 0;
            if ((modifiers & Domain.Enums.HotkeyModifier.Alt) != 0) fsModifiers |= NativeMethods.MOD_ALT;
            if ((modifiers & Domain.Enums.HotkeyModifier.Control) != 0) fsModifiers |= NativeMethods.MOD_CONTROL;
            if ((modifiers & Domain.Enums.HotkeyModifier.Shift) != 0) fsModifiers |= NativeMethods.MOD_SHIFT;
            if ((modifiers & Domain.Enums.HotkeyModifier.Win) != 0) fsModifiers |= NativeMethods.MOD_WIN;
            fsModifiers |= NativeMethods.MOD_NOREPEAT;

            bool success = NativeMethods.RegisterHotKey(_hwnd, id, fsModifiers, vk);
            if (success)
            {
                _idToCallback[id] = (hotkeyString, callback);
                _hotkeyToId[hotkeyString] = id;
            }

            return success;
        }
    }

    public void UnregisterHotkey(string hotkeyString)
    {
        if (string.IsNullOrWhiteSpace(hotkeyString))
            return;

        lock (_lock)
        {
            if (_hotkeyToId.TryGetValue(hotkeyString, out int id))
            {
                if (_hwnd != IntPtr.Zero)
                {
                    NativeMethods.UnregisterHotKey(_hwnd, id);
                }
                _idToCallback.Remove(id);
                _hotkeyToId.Remove(hotkeyString);
            }
        }
    }

    public void UnregisterAll()
    {
        lock (_lock)
        {
            if (_hwnd != IntPtr.Zero)
            {
                foreach (var id in _idToCallback.Keys)
                {
                    NativeMethods.UnregisterHotKey(_hwnd, id);
                }
            }
            _idToCallback.Clear();
            _hotkeyToId.Clear();
        }
    }

    public void ReRegisterAll(IntPtr newHwnd)
    {
        lock (_lock)
        {
            _hwnd = newHwnd;
            var list = _idToCallback.Values.ToList();
            _idToCallback.Clear();
            _hotkeyToId.Clear();

            foreach (var item in list)
            {
                RegisterHotkey(item.hotkey, item.callback);
            }
        }
    }

    public bool ProcessWindowMessage(int msg, IntPtr wParam, IntPtr lParam)
    {
        if (msg == NativeMethods.WM_HOTKEY)
        {
            int id = wParam.ToInt32();
            Action? callback = null;

            lock (_lock)
            {
                if (_idToCallback.TryGetValue(id, out var item))
                {
                    callback = item.callback;
                }
            }

            callback?.Invoke();
            return true;
        }

        return false;
    }

    public static (Domain.Enums.HotkeyModifier modifiers, uint vk) ParseHotkey(string hotkey)
    {
        if (string.IsNullOrWhiteSpace(hotkey))
            return (Domain.Enums.HotkeyModifier.None, 0);

        var parts = hotkey.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var modifiers = Domain.Enums.HotkeyModifier.None;
        uint vk = 0;

        foreach (var part in parts)
        {
            if (part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || part.Equals("Control", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= Domain.Enums.HotkeyModifier.Control;
            }
            else if (part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= Domain.Enums.HotkeyModifier.Alt;
            }
            else if (part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= Domain.Enums.HotkeyModifier.Shift;
            }
            else if (part.Equals("Win", StringComparison.OrdinalIgnoreCase) || part.Equals("Windows", StringComparison.OrdinalIgnoreCase))
            {
                modifiers |= Domain.Enums.HotkeyModifier.Win;
            }
            else
            {
                var (kVk, _, _) = KeyLookup.ResolveKey(part);
                if (kVk != 0)
                {
                    vk = kVk;
                }
                else if (part.Length == 1)
                {
                    vk = (uint)char.ToUpperInvariant(part[0]);
                }
            }
        }

        return (modifiers, vk);
    }

    public void Dispose()
    {
        UnregisterAll();
    }
}
