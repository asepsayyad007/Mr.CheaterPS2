using MrCheater.Domain.Interfaces;

namespace MrCheater.Core.Services;

public class EmergencyStopManager
{
    private readonly IInputProvider _inputProvider;
    private readonly AppLogger _logger;
    private CancellationTokenSource? _activeCts;
    private readonly object _lock = new();

    public event EventHandler? EmergencyStopTriggered;

    public EmergencyStopManager(IInputProvider inputProvider, AppLogger logger)
    {
        _inputProvider = inputProvider;
        _logger = logger;
    }

    public CancellationToken RegisterExecution(CancellationTokenSource cts)
    {
        lock (_lock)
        {
            _activeCts = cts;
            return cts.Token;
        }
    }

    public void UnregisterExecution(CancellationTokenSource cts)
    {
        lock (_lock)
        {
            if (_activeCts == cts)
            {
                _activeCts = null;
            }
        }
    }

    public void TriggerEmergencyStop()
    {
        _logger.Warning("EMERGENCY STOP TRIGGERED! Aborting all active sequences and releasing inputs.");

        lock (_lock)
        {
            if (_activeCts != null && !_activeCts.IsCancellationRequested)
            {
                try
                {
                    _activeCts.Cancel();
                }
                catch
                {
                    // Ignore
                }
            }
        }

        try
        {
            _inputProvider.ReleaseAllKeys();
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to release keys during emergency stop: {ex.Message}");
        }

        EmergencyStopTriggered?.Invoke(this, EventArgs.Empty);
    }
}
