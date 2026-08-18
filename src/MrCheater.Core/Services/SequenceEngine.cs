using System.Collections.Concurrent;
using System.Diagnostics;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.Core.Services;

public class SequenceEngine
{
    private readonly IInputProvider _inputProvider;
    private readonly IProcessMonitor _processMonitor;
    private readonly ISettingsRepository _settingsRepository;
    private readonly PrerequisiteResolver _prerequisiteResolver;
    private readonly EmergencyStopManager _emergencyStopManager;
    private readonly AppLogger _logger;

    private readonly SemaphoreSlim _executionLock = new(1, 1);
    private readonly ConcurrentDictionary<string, bool> _masterCodeActive = new(StringComparer.OrdinalIgnoreCase);

    public event EventHandler<ExecutionProgress>? ProgressChanged;
    public event EventHandler<(string ProfileId, bool Active)>? MasterCodeStateChanged;

    public bool IsExecuting => _executionLock.CurrentCount == 0;

    public SequenceEngine(
        IInputProvider inputProvider,
        IProcessMonitor processMonitor,
        ISettingsRepository settingsRepository,
        PrerequisiteResolver prerequisiteResolver,
        EmergencyStopManager emergencyStopManager,
        AppLogger logger)
    {
        _inputProvider = inputProvider;
        _processMonitor = processMonitor;
        _settingsRepository = settingsRepository;
        _prerequisiteResolver = prerequisiteResolver;
        _emergencyStopManager = emergencyStopManager;
        _logger = logger;
    }

    public bool IsMasterCodeActive(string profileId)
    {
        return _masterCodeActive.TryGetValue(profileId, out bool active) && active;
    }

    public void SetMasterCodeActive(string profileId, bool active)
    {
        _masterCodeActive[profileId] = active;
        MasterCodeStateChanged?.Invoke(this, (profileId, active));
        _logger.Info($"Master Code state for profile '{profileId}' set to: {(active ? "ACTIVE (Unlocked)" : "INACTIVE (Locked)")}");
    }

    public async Task<ExecutionResult> ExecuteCheatAsync(
        CheatDefinition cheat,
        GameProfile profile,
        IProgress<ExecutionProgress>? progress = null,
        CancellationToken externalCancellationToken = default)
    {
        if (cheat == null)
            return ExecutionResult.Failed("Cheat definition is null.");
        if (profile == null)
            return ExecutionResult.Failed("Game profile is null.");

        if (!await _executionLock.WaitAsync(100, externalCancellationToken))
        {
            return ExecutionResult.Failed("Another sequence is currently executing.");
        }

        var settings = await _settingsRepository.LoadSettingsAsync();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken);
        linkedCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(5, settings.MaxSequenceDurationSeconds)));

        var token = _emergencyStopManager.RegisterExecution(linkedCts);
        var stopwatch = Stopwatch.StartNew();
        int totalStepsExecuted = 0;

        try
        {
            // Safety checks & Target Activation
            var safetyResult = await EnsureTargetFocusAsync(profile, settings, token);
            if (!safetyResult.Success)
            {
                _logger.Warning($"Safety check failed: {safetyResult.Message}");
                return safetyResult;
            }

            _logger.Info($"Starting cheat execution: '{cheat.Name}' for game '{profile.Name}'");

            bool isMasterCheat = PrerequisiteResolver.IsMasterCode(cheat);
            bool isMasterAlreadyActive = IsMasterCodeActive(profile.Id);

            // Determine execution chain: If Master Code is already active in this session, do NOT re-enter it!
            bool includeMasterCode = !isMasterAlreadyActive;
            var executionPlan = _prerequisiteResolver.ResolveExecutionChain(cheat, profile.Cheats, includeMasterCode);
            int grandTotalSteps = executionPlan.Sum(c => c.Sequence.Count);

            foreach (var currentCheat in executionPlan)
            {
                token.ThrowIfCancellationRequested();
                bool isPrereq = currentCheat.Id != cheat.Id;

                if (isPrereq)
                {
                    _logger.Info($"Executing prerequisite: '{currentCheat.Name}'");
                }

                int stepDelay = currentCheat.DefaultDelayMs > 0 ? currentCheat.DefaultDelayMs : profile.DefaultStepDelayMs;

                for (int i = 0; i < currentCheat.Sequence.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var step = currentCheat.Sequence[i];
                    totalStepsExecuted++;

                    var prog = new ExecutionProgress
                    {
                        SequenceName = currentCheat.Name,
                        CurrentStepIndex = totalStepsExecuted,
                        TotalSteps = grandTotalSteps,
                        CurrentAction = step.ActionName,
                        IsPrerequisiteStep = isPrereq,
                        StatusMessage = isPrereq
                            ? $"Sending Prerequisite: {currentCheat.Name} [{i + 1}/{currentCheat.Sequence.Count}]"
                            : $"Sending Cheat: {currentCheat.Name} [{i + 1}/{currentCheat.Sequence.Count}]"
                    };

                    progress?.Report(prog);
                    ProgressChanged?.Invoke(this, prog);

                    int hold = step.HoldDurationMs > 0 ? step.HoldDurationMs : profile.DefaultHoldDurationMs;
                    int delay = step.DelayAfterMs > 0 ? step.DelayAfterMs : stepDelay;

                    await _inputProvider.SendActionAsync(
                        step.ActionName,
                        profile.InputMapping,
                        step.ActionType,
                        hold,
                        token);

                    await _inputProvider.DelayAsync(delay, token);
                }

                // If this step was Master Code, record that Master Code is now active
                if (PrerequisiteResolver.IsMasterCode(currentCheat))
                {
                    SetMasterCodeActive(profile.Id, isMasterCheat ? !isMasterAlreadyActive : true);
                }

                // Pause between prerequisite and main cheat
                if (isPrereq)
                {
                    await _inputProvider.DelayAsync(250, token);
                }
            }

            stopwatch.Stop();
            _logger.Success($"Successfully executed cheat '{cheat.Name}' in {stopwatch.ElapsedMilliseconds}ms ({totalStepsExecuted} steps)");

            return ExecutionResult.Succeeded(
                $"Cheat '{cheat.Name}' executed successfully.",
                totalStepsExecuted,
                stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _logger.Warning($"Execution cancelled for '{cheat.Name}' after {stopwatch.ElapsedMilliseconds}ms");
            _inputProvider.ReleaseAllKeys();
            return ExecutionResult.Cancelled("Sequence was cancelled or emergency stopped.", totalStepsExecuted, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.Error($"Error executing cheat '{cheat.Name}': {ex.Message}");
            _inputProvider.ReleaseAllKeys();
            return ExecutionResult.Failed($"Execution failed: {ex.Message}");
        }
        finally
        {
            _emergencyStopManager.UnregisterExecution(linkedCts);
            _executionLock.Release();
        }
    }

    public async Task<ExecutionResult> ExecuteMacroAsync(
        MacroDefinition macro,
        GameProfile profile,
        IProgress<ExecutionProgress>? progress = null,
        CancellationToken externalCancellationToken = default)
    {
        if (macro == null)
            return ExecutionResult.Failed("Macro definition is null.");
        if (profile == null)
            return ExecutionResult.Failed("Game profile is null.");

        if (!await _executionLock.WaitAsync(100, externalCancellationToken))
        {
            return ExecutionResult.Failed("Another sequence is currently executing.");
        }

        var settings = await _settingsRepository.LoadSettingsAsync();
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(externalCancellationToken);
        linkedCts.CancelAfter(TimeSpan.FromSeconds(Math.Max(5, settings.MaxSequenceDurationSeconds)));

        var token = _emergencyStopManager.RegisterExecution(linkedCts);
        var stopwatch = Stopwatch.StartNew();
        int totalStepsExecuted = 0;

        try
        {
            var safetyResult = await EnsureTargetFocusAsync(profile, settings, token);
            if (!safetyResult.Success)
            {
                return safetyResult;
            }

            _logger.Info($"Starting macro execution: '{macro.Name}' (Loops: {macro.LoopCount})");
            int grandTotalSteps = macro.Sequence.Count * Math.Max(1, macro.LoopCount);

            for (int loop = 1; loop <= Math.Max(1, macro.LoopCount); loop++)
            {
                for (int i = 0; i < macro.Sequence.Count; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var step = macro.Sequence[i];
                    totalStepsExecuted++;

                    var prog = new ExecutionProgress
                    {
                        SequenceName = macro.Name,
                        CurrentStepIndex = totalStepsExecuted,
                        TotalSteps = grandTotalSteps,
                        CurrentAction = step.ActionName,
                        IsPrerequisiteStep = false,
                        StatusMessage = $"Macro: {macro.Name} (Loop {loop}/{macro.LoopCount}) [{i + 1}/{macro.Sequence.Count}]"
                    };

                    progress?.Report(prog);
                    ProgressChanged?.Invoke(this, prog);

                    int hold = step.HoldDurationMs > 0 ? step.HoldDurationMs : profile.DefaultHoldDurationMs;
                    int delay = step.DelayAfterMs > 0 ? step.DelayAfterMs : profile.DefaultStepDelayMs;

                    await _inputProvider.SendActionAsync(
                        step.ActionName,
                        profile.InputMapping,
                        step.ActionType,
                        hold,
                        token);

                    await _inputProvider.DelayAsync(delay, token);
                }
            }

            stopwatch.Stop();
            _logger.Success($"Successfully completed macro '{macro.Name}' in {stopwatch.ElapsedMilliseconds}ms");

            return ExecutionResult.Succeeded(
                $"Macro '{macro.Name}' completed successfully.",
                totalStepsExecuted,
                stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _inputProvider.ReleaseAllKeys();
            return ExecutionResult.Cancelled("Macro cancelled or stopped.", totalStepsExecuted, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _inputProvider.ReleaseAllKeys();
            return ExecutionResult.Failed($"Macro execution failed: {ex.Message}");
        }
        finally
        {
            _emergencyStopManager.UnregisterExecution(linkedCts);
            _executionLock.Release();
        }
    }

    public async Task<ExecutionResult> ExecuteRawSequenceAsync(
        IEnumerable<SequenceStep> sequence,
        InputMappingProfile mapping,
        int defaultDelayMs = 70,
        int defaultHoldMs = 70,
        CancellationToken cancellationToken = default)
    {
        var stepsList = sequence.ToList();
        if (stepsList.Count == 0)
            return ExecutionResult.Failed("Sequence is empty.");

        var stopwatch = Stopwatch.StartNew();
        int total = 0;

        try
        {
            foreach (var step in stepsList)
            {
                cancellationToken.ThrowIfCancellationRequested();
                total++;

                int hold = step.HoldDurationMs > 0 ? step.HoldDurationMs : defaultHoldMs;
                int delay = step.DelayAfterMs > 0 ? step.DelayAfterMs : defaultDelayMs;

                await _inputProvider.SendActionAsync(
                    step.ActionName,
                    mapping,
                    step.ActionType,
                    hold,
                    cancellationToken);

                await _inputProvider.DelayAsync(delay, cancellationToken);
            }

            stopwatch.Stop();
            return ExecutionResult.Succeeded("Sequence executed.", total, stopwatch.Elapsed);
        }
        catch (OperationCanceledException)
        {
            stopwatch.Stop();
            _inputProvider.ReleaseAllKeys();
            return ExecutionResult.Cancelled("Sequence cancelled.", total, stopwatch.Elapsed);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _inputProvider.ReleaseAllKeys();
            return ExecutionResult.Failed(ex.Message);
        }
    }

    private async Task<ExecutionResult> EnsureTargetFocusAsync(GameProfile profile, AppSettings settings, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(profile.TargetProcess))
            return ExecutionResult.Succeeded("No target process specified.", 0, TimeSpan.Zero);

        var targetProcesses = new[] { profile.TargetProcess };

        if (settings.RequireTargetProcessRunning)
        {
            var status = _processMonitor.GetProcessStatus(targetProcesses, profile.WindowTitlePattern);
            if (status == TargetProcessStatus.NotRunning)
            {
                return ExecutionResult.Failed($"Target process '{profile.TargetProcess}' is not running. Please launch PCSX2 / your game.");
            }

            if (status != TargetProcessStatus.Focused)
            {
                bool focused = _processMonitor.TryFocusTargetProcess(profile.TargetProcess);
                if (focused)
                {
                    await Task.Delay(180, token);
                }
            }
        }

        return ExecutionResult.Succeeded("Safety checks passed.", 0, TimeSpan.Zero);
    }
}
