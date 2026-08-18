using MrCheater.Domain.Enums;
using MrCheater.Domain.Models;

namespace MrCheater.Domain.Interfaces;

public interface IProcessMonitor : IDisposable
{
    TargetProcessStatus GetProcessStatus(IEnumerable<string> processNames, string? windowTitlePattern = null);
    bool IsProcessFocused(IEnumerable<string> processNames);
    bool TryFocusTargetProcess(string processName);
    event EventHandler<TargetProcessStatus>? StatusChanged;
    void SetWatchedTargets(IEnumerable<string> processNames, string? windowTitlePattern = null);
    void Start();
    void Stop();
}

public interface IXInputControllerService : IDisposable
{
    bool IsControllerConnected(int userIndex = 0);
    string GetControllerName(int userIndex = 0);
    ControllerStateSnapshot GetState(int userIndex = 0);
    event EventHandler<bool>? ConnectionChanged;
    event EventHandler<ControllerStateSnapshot>? StatePolled;
    void StartPolling();
    void StopPolling();
}

public interface IGlobalHotkeyService : IDisposable
{
    bool RegisterHotkey(string hotkeyString, Action callback);
    void UnregisterHotkey(string hotkeyString);
    void UnregisterAll();
}
