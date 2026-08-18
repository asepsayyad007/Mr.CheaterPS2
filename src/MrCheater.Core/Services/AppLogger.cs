using System.Collections.ObjectModel;
using MrCheater.Core.Common;

namespace MrCheater.Core.Services;

public enum LogLevel
{
    Info,
    Warning,
    Error,
    Success
}

public class LogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    public string DisplayText => $"[{Timestamp:HH:mm:ss}] [{Level.ToString().ToUpper()}] {Message}";
}

public class AppLogger
{
    public static AppLogger Instance { get; } = new();

    private readonly object _lock = new();
    private readonly string _logFilePath;
    private Action<Action>? _uiDispatcher;

    public ObservableCollection<LogEntry> RecentLogs { get; } = new();
    public event EventHandler<LogEntry>? LogAdded;

    public AppLogger()
    {
        _logFilePath = Path.Combine(StorageHelper.GetLogsDirectory(), $"app_{DateTime.Now:yyyyMMdd}.log");
    }

    public void SetUiDispatcher(Action<Action> dispatcherRunner)
    {
        _uiDispatcher = dispatcherRunner;
    }

    public void Log(LogLevel level, string message)
    {
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message
        };

        lock (_lock)
        {
            try
            {
                File.AppendAllText(_logFilePath, entry.DisplayText + Environment.NewLine);
            }
            catch
            {
                // Silently ignore disk logging error
            }
        }

        if (_uiDispatcher != null)
        {
            _uiDispatcher(() => AddEntry(entry));
        }
        else
        {
            AddEntry(entry);
        }

        LogAdded?.Invoke(this, entry);
    }

    private void AddEntry(LogEntry entry)
    {
        lock (_lock)
        {
            if (RecentLogs.Count > 300)
            {
                RecentLogs.RemoveAt(0);
            }
            RecentLogs.Add(entry);
        }
    }

    public void Info(string message) => Log(LogLevel.Info, message);
    public void Warning(string message) => Log(LogLevel.Warning, message);
    public void Error(string message) => Log(LogLevel.Error, message);
    public void Success(string message) => Log(LogLevel.Success, message);
}
