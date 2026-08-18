using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;
using MrCheater.Infrastructure.Native;

namespace MrCheater.Infrastructure.Input;

public class WindowsSendInputProvider : IInputProvider
{
    private readonly ConcurrentDictionary<(ushort vk, ushort sc, bool extended), bool> _heldKeys = new();

    public async Task SendActionAsync(
        string actionName,
        InputMappingProfile mapping,
        StepActionType actionType,
        int holdDurationMs,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(actionName))
            return;

        if (actionType == StepActionType.Delay)
        {
            await DelayAsync(holdDurationMs, cancellationToken);
            return;
        }

        if (mapping.Mappings.TryGetValue(actionName, out var entry))
        {
            await SendKeyDirectAsync(entry.KeyboardKey, actionType, holdDurationMs, cancellationToken);
        }
        else
        {
            await SendKeyDirectAsync(actionName, actionType, holdDurationMs, cancellationToken);
        }
    }

    public async Task SendKeyDirectAsync(
        string keyName,
        StepActionType actionType,
        int holdDurationMs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(keyName))
            return;

        if (actionType == StepActionType.Delay)
        {
            await DelayAsync(holdDurationMs, cancellationToken);
            return;
        }

        var (vk, sc, extended) = KeyLookup.ResolveKey(keyName);
        if (vk == 0 && sc == 0)
        {
            if (keyName.Length == 1)
            {
                vk = (ushort)char.ToUpperInvariant(keyName[0]);
            }
        }

        if (vk == 0 && sc == 0)
            return;

        if (sc == 0 && vk != 0)
        {
            sc = (ushort)NativeMethods.MapVirtualKey(vk, NativeMethods.MAPVK_VK_TO_VSC);
        }

        int hold = Math.Max(20, holdDurationMs > 0 ? holdDurationMs : 70);

        switch (actionType)
        {
            case StepActionType.KeyDown:
                SendKeyEvent(vk, sc, extended, isKeyUp: false);
                _heldKeys[(vk, sc, extended)] = true;
                break;

            case StepActionType.KeyUp:
                SendKeyEvent(vk, sc, extended, isKeyUp: true);
                _heldKeys.TryRemove((vk, sc, extended), out _);
                break;

            case StepActionType.Tap:
            default:
                SendKeyEvent(vk, sc, extended, isKeyUp: false);
                _heldKeys[(vk, sc, extended)] = true;

                await DelayAsync(hold, cancellationToken);

                SendKeyEvent(vk, sc, extended, isKeyUp: true);
                _heldKeys.TryRemove((vk, sc, extended), out _);
                break;
        }
    }

    public async Task DelayAsync(int delayMs, CancellationToken cancellationToken = default)
    {
        if (delayMs <= 0)
            return;

        if (delayMs < 15)
        {
            var sw = global::System.Diagnostics.Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < delayMs)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Thread.Yield();
            }
        }
        else
        {
            await Task.Delay(delayMs, cancellationToken);
        }
    }

    public void ReleaseAllKeys()
    {
        foreach (var key in _heldKeys.Keys)
        {
            try
            {
                SendKeyEvent(key.vk, key.sc, key.extended, isKeyUp: true);
            }
            catch
            {
                // Suppress errors during emergency release
            }
        }
        _heldKeys.Clear();
    }

    private static void SendKeyEvent(ushort vk, ushort sc, bool extended, bool isKeyUp)
    {
        uint flags = 0;
        if (isKeyUp)
        {
            flags |= NativeMethods.KEYEVENTF_KEYUP;
        }

        if (sc != 0)
        {
            flags |= NativeMethods.KEYEVENTF_SCANCODE;
        }

        if (extended)
        {
            flags |= NativeMethods.KEYEVENTF_EXTENDEDKEY;
        }

        var input = new NativeMethods.INPUT
        {
            type = NativeMethods.INPUT_KEYBOARD,
            u = new NativeMethods.InputUnion
            {
                ki = new NativeMethods.KEYBDINPUT
                {
                    wVk = vk,
                    wScan = sc,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = IntPtr.Zero
                }
            }
        };

        NativeMethods.SendInput(1, new[] { input }, Marshal.SizeOf<NativeMethods.INPUT>());
    }
}
