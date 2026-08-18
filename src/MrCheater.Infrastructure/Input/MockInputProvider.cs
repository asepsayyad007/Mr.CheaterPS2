using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.Infrastructure.Input;

public class MockInputProvider : IInputProvider
{
    public List<string> SentActions { get; } = new();
    public List<string> SentKeys { get; } = new();
    public List<string> LogHistory { get; } = new();
    public bool SimulateDelays { get; set; } = false;
    public bool IsAllReleasedCalled { get; private set; }

    public async Task SendActionAsync(
        string actionName,
        InputMappingProfile mapping,
        StepActionType actionType,
        int holdDurationMs,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        SentActions.Add(actionName);
        LogHistory.Add($"[Action] {actionName} ({actionType}, {holdDurationMs}ms)");

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

        SentKeys.Add(keyName);
        LogHistory.Add($"[Key] {keyName} ({actionType}, {holdDurationMs}ms)");

        if (SimulateDelays && holdDurationMs > 0)
        {
            await DelayAsync(holdDurationMs, cancellationToken);
        }
    }

    public async Task DelayAsync(int delayMs, CancellationToken cancellationToken = default)
    {
        if (delayMs <= 0)
            return;

        LogHistory.Add($"[Delay] {delayMs}ms");

        if (SimulateDelays)
        {
            await Task.Delay(delayMs, cancellationToken);
        }
    }

    public void ReleaseAllKeys()
    {
        IsAllReleasedCalled = true;
        LogHistory.Add("[ReleaseAllKeys]");
    }

    public void Clear()
    {
        SentActions.Clear();
        SentKeys.Clear();
        LogHistory.Clear();
        IsAllReleasedCalled = false;
    }
}
