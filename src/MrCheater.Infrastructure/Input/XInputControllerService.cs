using System.Timers;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;
using MrCheater.Infrastructure.Native;
using Timer = System.Timers.Timer;

namespace MrCheater.Infrastructure.Input;

public class XInputControllerService : IXInputControllerService
{
    private readonly Timer _pollTimer;
    private bool _lastConnectedState = false;
    private bool _use14 = true;
    private int _isPolling = 0;

    public event EventHandler<bool>? ConnectionChanged;
    public event EventHandler<ControllerStateSnapshot>? StatePolled;

    public XInputControllerService(int pollIntervalMs = 60)
    {
        _pollTimer = new Timer(pollIntervalMs);
        _pollTimer.Elapsed += OnPollTimerElapsed;
        _pollTimer.AutoReset = true;
    }

    public void StartPolling()
    {
        _pollTimer.Start();
    }

    public void StopPolling()
    {
        _pollTimer.Stop();
    }

    public bool IsControllerConnected(int userIndex = 0)
    {
        var snapshot = GetState(userIndex);
        return snapshot.IsConnected;
    }

    public string GetControllerName(int userIndex = 0)
    {
        return IsControllerConnected(userIndex) ? $"XInput Controller {userIndex + 1} (Xbox Standard)" : "Disconnected";
    }

    public ControllerStateSnapshot GetState(int userIndex = 0)
    {
        var state = new NativeMethods.XINPUT_STATE();
        uint result;

        try
        {
            if (_use14)
            {
                result = NativeMethods.XInput14_GetState((uint)userIndex, ref state);
            }
            else
            {
                result = NativeMethods.XInput91_GetState((uint)userIndex, ref state);
            }
        }
        catch (DllNotFoundException)
        {
            try
            {
                _use14 = false;
                result = NativeMethods.XInput91_GetState((uint)userIndex, ref state);
            }
            catch
            {
                result = NativeMethods.ERROR_DEVICE_NOT_CONNECTED;
            }
        }
        catch
        {
            result = NativeMethods.ERROR_DEVICE_NOT_CONNECTED;
        }

        if (result != NativeMethods.ERROR_SUCCESS)
        {
            return new ControllerStateSnapshot
            {
                IsConnected = false,
                UserIndex = userIndex,
                ControllerType = "Disconnected"
            };
        }

        var pad = state.Gamepad;
        ushort btn = pad.wButtons;

        return new ControllerStateSnapshot
        {
            IsConnected = true,
            UserIndex = userIndex,
            ControllerType = "XInput Gamepad",

            DPadUp = (btn & NativeMethods.XINPUT_GAMEPAD_DPAD_UP) != 0,
            DPadDown = (btn & NativeMethods.XINPUT_GAMEPAD_DPAD_DOWN) != 0,
            DPadLeft = (btn & NativeMethods.XINPUT_GAMEPAD_DPAD_LEFT) != 0,
            DPadRight = (btn & NativeMethods.XINPUT_GAMEPAD_DPAD_RIGHT) != 0,

            Start = (btn & NativeMethods.XINPUT_GAMEPAD_START) != 0,
            Back = (btn & NativeMethods.XINPUT_GAMEPAD_BACK) != 0,
            LeftThumb = (btn & NativeMethods.XINPUT_GAMEPAD_LEFT_THUMB) != 0,
            RightThumb = (btn & NativeMethods.XINPUT_GAMEPAD_RIGHT_THUMB) != 0,

            LeftShoulder = (btn & NativeMethods.XINPUT_GAMEPAD_LEFT_SHOULDER) != 0,
            RightShoulder = (btn & NativeMethods.XINPUT_GAMEPAD_RIGHT_SHOULDER) != 0,

            A = (btn & NativeMethods.XINPUT_GAMEPAD_A) != 0,
            B = (btn & NativeMethods.XINPUT_GAMEPAD_B) != 0,
            X = (btn & NativeMethods.XINPUT_GAMEPAD_X) != 0,
            Y = (btn & NativeMethods.XINPUT_GAMEPAD_Y) != 0,

            LeftTrigger = pad.bLeftTrigger,
            RightTrigger = pad.bRightTrigger,

            ThumbLX = pad.sThumbLX,
            ThumbLY = pad.sThumbLY,
            ThumbRX = pad.sThumbRX,
            ThumbRY = pad.sThumbRY
        };
    }

    private void OnPollTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        if (Interlocked.CompareExchange(ref _isPolling, 1, 0) != 0)
            return;

        try
        {
            var snapshot = GetState(0);
            if (snapshot.IsConnected != _lastConnectedState)
            {
                _lastConnectedState = snapshot.IsConnected;
                ConnectionChanged?.Invoke(this, snapshot.IsConnected);
            }

            StatePolled?.Invoke(this, snapshot);
        }
        finally
        {
            Interlocked.Exchange(ref _isPolling, 0);
        }
    }

    public void Dispose()
    {
        _pollTimer.Stop();
        _pollTimer.Dispose();
    }
}
