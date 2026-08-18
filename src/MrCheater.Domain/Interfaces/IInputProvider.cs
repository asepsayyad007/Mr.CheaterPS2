using MrCheater.Domain.Enums;
using MrCheater.Domain.Models;

namespace MrCheater.Domain.Interfaces;

public interface IInputProvider
{
    Task SendActionAsync(
        string actionName,
        InputMappingProfile mapping,
        StepActionType actionType,
        int holdDurationMs,
        CancellationToken cancellationToken = default);

    Task SendKeyDirectAsync(
        string keyName,
        StepActionType actionType,
        int holdDurationMs,
        CancellationToken cancellationToken = default);

    Task DelayAsync(int delayMs, CancellationToken cancellationToken = default);

    void ReleaseAllKeys();
}
