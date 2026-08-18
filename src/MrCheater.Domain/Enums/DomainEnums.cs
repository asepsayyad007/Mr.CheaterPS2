namespace MrCheater.Domain.Enums;

public enum StepActionType
{
    Tap = 0,
    KeyDown = 1,
    KeyUp = 2,
    Delay = 3
}

public enum TargetProcessStatus
{
    NotRunning = 0,
    Running = 1,
    Focused = 2
}

[Flags]
public enum HotkeyModifier
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8
}

public enum InputDeviceType
{
    Keyboard = 0,
    XInputController = 1,
    Both = 2
}
