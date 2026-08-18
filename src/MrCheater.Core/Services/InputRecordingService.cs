using System.Collections.ObjectModel;
using System.Diagnostics;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.Core.Services;

public class RecordedInputEvent
{
    public string ActionName { get; set; } = string.Empty;
    public StepActionType Type { get; set; } = StepActionType.Tap;
    public long TimestampMs { get; set; }
}

public class InputRecordingService
{
    private readonly IXInputControllerService _xInputService;
    private readonly AppLogger _logger;
    private readonly Stopwatch _stopwatch = new();
    private readonly List<RecordedInputEvent> _rawEvents = new();
    private readonly HashSet<string> _currentlyPressedButtons = new(StringComparer.OrdinalIgnoreCase);

    public bool IsRecording { get; private set; }
    public ObservableCollection<SequenceStep> RecordedSequence { get; } = new();
    public event EventHandler<bool>? RecordingStateChanged;
    public event EventHandler<SequenceStep>? StepRecorded;

    public InputRecordingService(IXInputControllerService xInputService, AppLogger logger)
    {
        _xInputService = xInputService;
        _logger = logger;
        _xInputService.StatePolled += OnControllerStatePolled;
    }

    public void StartRecording()
    {
        if (IsRecording)
            return;

        _rawEvents.Clear();
        _currentlyPressedButtons.Clear();
        RecordedSequence.Clear();
        _stopwatch.Restart();
        IsRecording = true;

        _logger.Info("Input recording session started.");
        RecordingStateChanged?.Invoke(this, true);
    }

    public List<SequenceStep> StopRecording()
    {
        if (!IsRecording)
            return RecordedSequence.ToList();

        _stopwatch.Stop();
        IsRecording = false;

        _logger.Info($"Input recording session stopped. Recorded {RecordedSequence.Count} steps.");
        RecordingStateChanged?.Invoke(this, false);

        return RecordedSequence.ToList();
    }

    public void RecordKeyAction(string actionName, StepActionType type = StepActionType.Tap, int holdMs = 70, int delayAfterMs = 70)
    {
        if (!IsRecording || string.IsNullOrWhiteSpace(actionName))
            return;

        var step = new SequenceStep
        {
            ActionName = actionName.ToUpperInvariant(),
            ActionType = type,
            HoldDurationMs = holdMs,
            DelayAfterMs = delayAfterMs
        };

        RecordedSequence.Add(step);
        StepRecorded?.Invoke(this, step);
    }

    private void OnControllerStatePolled(object? sender, ControllerStateSnapshot snapshot)
    {
        if (!IsRecording || !snapshot.IsConnected)
            return;

        var activeButtons = new HashSet<string>(snapshot.GetActiveButtonNames(), StringComparer.OrdinalIgnoreCase);

        // Detect newly pressed buttons
        foreach (var btn in activeButtons)
        {
            if (!_currentlyPressedButtons.Contains(btn))
            {
                _currentlyPressedButtons.Add(btn);
                
                // Map controller button to standard action name if possible
                string action = MapControllerButtonToAction(btn);
                RecordKeyAction(action, StepActionType.Tap, holdMs: 70, delayAfterMs: 70);
            }
        }

        // Detect released buttons
        _currentlyPressedButtons.RemoveWhere(b => !activeButtons.Contains(b));
    }

    private static string MapControllerButtonToAction(string btn)
    {
        return btn switch
        {
            "DPadUp" => "UP",
            "DPadDown" => "DOWN",
            "DPadLeft" => "LEFT",
            "DPadRight" => "RIGHT",
            "Y" => "TRIANGLE",
            "B" => "CIRCLE",
            "A" => "CROSS",
            "X" => "SQUARE",
            "LeftShoulder" => "L1",
            "RightShoulder" => "R1",
            "LeftTrigger" => "L2",
            "RightTrigger" => "R2",
            "LeftThumb" => "L3",
            "RightThumb" => "R3",
            "Start" => "START",
            "Back" => "SELECT",
            _ => btn.ToUpperInvariant()
        };
    }
}
