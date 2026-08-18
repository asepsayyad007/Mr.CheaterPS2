using System.Diagnostics;
using System.Timers;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Infrastructure.Native;
using Timer = System.Timers.Timer;

namespace MrCheater.Infrastructure.System;

public class WindowsProcessMonitor : IProcessMonitor
{
    private readonly Timer _timer;
    private readonly List<string> _watchedProcesses = new();
    private string? _windowTitlePattern;
    private TargetProcessStatus _lastStatus = TargetProcessStatus.NotRunning;

    public event EventHandler<TargetProcessStatus>? StatusChanged;

    public WindowsProcessMonitor(int checkIntervalMs = 500)
    {
        _timer = new Timer(checkIntervalMs);
        _timer.Elapsed += OnTimerElapsed;
        _timer.AutoReset = true;
    }

    public void SetWatchedTargets(IEnumerable<string> processNames, string? windowTitlePattern = null)
    {
        lock (_watchedProcesses)
        {
            _watchedProcesses.Clear();
            foreach (var p in processNames)
            {
                var clean = p.Trim();
                if (clean.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    clean = clean[..^4];
                _watchedProcesses.Add(clean);
            }
            _windowTitlePattern = windowTitlePattern;
        }

        CheckAndUpdateStatus();
    }

    public TargetProcessStatus GetProcessStatus(IEnumerable<string> processNames, string? windowTitlePattern = null)
    {
        var targetList = processNames
            .Select(p => p.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) ? p[..^4] : p)
            .ToList();

        if (targetList.Count == 0)
            return TargetProcessStatus.NotRunning;

        var runningProcesses = new List<Process>();
        foreach (var name in targetList)
        {
            try
            {
                var found = Process.GetProcessesByName(name);
                runningProcesses.AddRange(found);
            }
            catch
            {
                // Ignored
            }
        }

        if (runningProcesses.Count == 0)
            return TargetProcessStatus.NotRunning;

        var fgHwnd = NativeMethods.GetForegroundWindow();
        if (fgHwnd != IntPtr.Zero)
        {
            NativeMethods.GetWindowThreadProcessId(fgHwnd, out uint fgPid);
            foreach (var proc in runningProcesses)
            {
                if (proc.Id == fgPid)
                    return TargetProcessStatus.Focused;
            }
        }

        return TargetProcessStatus.Running;
    }

    public bool IsProcessFocused(IEnumerable<string> processNames)
    {
        return GetProcessStatus(processNames) == TargetProcessStatus.Focused;
    }

    public bool TryFocusTargetProcess(string processName)
    {
        if (string.IsNullOrWhiteSpace(processName))
            return false;

        var cleanName = processName.Trim();
        if (cleanName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
            cleanName = cleanName[..^4];

        Process[] procs;
        try
        {
            procs = Process.GetProcessesByName(cleanName);
        }
        catch
        {
            return false;
        }

        if (procs.Length == 0)
            return false;

        var targetProc = procs[0];
        IntPtr targetHwnd = targetProc.MainWindowHandle;

        if (targetHwnd == IntPtr.Zero)
        {
            // Enumerate windows for this PID
            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                NativeMethods.GetWindowThreadProcessId(hWnd, out uint pid);
                if (pid == targetProc.Id && NativeMethods.IsWindowVisible(hWnd))
                {
                    targetHwnd = hWnd;
                    return false; // stop enumeration
                }
                return true;
            }, IntPtr.Zero);
        }

        if (targetHwnd != IntPtr.Zero)
        {
            NativeMethods.ShowWindow(targetHwnd, NativeMethods.SW_RESTORE);

            uint currentThreadId = NativeMethods.GetCurrentThreadId();
            NativeMethods.GetWindowThreadProcessId(targetHwnd, out uint targetThreadId);

            if (currentThreadId != targetThreadId)
            {
                NativeMethods.AttachThreadInput(currentThreadId, targetThreadId, true);
                NativeMethods.BringWindowToTop(targetHwnd);
                NativeMethods.SetForegroundWindow(targetHwnd);
                NativeMethods.AttachThreadInput(currentThreadId, targetThreadId, false);
            }
            else
            {
                NativeMethods.BringWindowToTop(targetHwnd);
                NativeMethods.SetForegroundWindow(targetHwnd);
            }

            return true;
        }

        return false;
    }

    public void Start()
    {
        _timer.Start();
        CheckAndUpdateStatus();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    private void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        CheckAndUpdateStatus();
    }

    private void CheckAndUpdateStatus()
    {
        List<string> targets;
        string? pattern;

        lock (_watchedProcesses)
        {
            targets = new List<string>(_watchedProcesses);
            pattern = _windowTitlePattern;
        }

        var status = GetProcessStatus(targets, pattern);
        if (status != _lastStatus)
        {
            _lastStatus = status;
            StatusChanged?.Invoke(this, status);
        }
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
