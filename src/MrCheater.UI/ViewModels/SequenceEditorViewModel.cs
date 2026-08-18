using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Services;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class SequenceEditorViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ProfileService _profileService;
    private readonly SequenceEngine _sequenceEngine;
    private readonly IInputProvider _inputProvider;
    private readonly InputRecordingService _recordingService;
    private readonly AppLogger _logger;

    private CheatDefinition? _editingCheat;
    private bool _isNewCheat;

    [ObservableProperty]
    private string _editorTitle = "Cheat Sequence Editor";

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _category = "General";

    [ObservableProperty]
    private bool _requiresMasterCode = false;

    [ObservableProperty]
    private string _assignedHotkey = string.Empty;

    [ObservableProperty]
    private int _defaultDelayMs = 70;

    [ObservableProperty]
    private int _defaultHoldMs = 70;

    [ObservableProperty]
    private SequenceStep? _selectedStep;

    [ObservableProperty]
    private bool _isRecording = false;

    [ObservableProperty]
    private string _testOutputLog = string.Empty;

    public ObservableCollection<SequenceStep> Steps { get; } = new();

    public ObservableCollection<string> AvailableCategories { get; } = new()
    {
        "General",
        "System",
        "Unlocks",
        "Money",
        "Gameplay",
        "Physics",
        "Combat",
        "Weapons",
        "Vehicles"
    };

    public SequenceEditorViewModel(
        MainViewModel mainVM,
        ProfileService profileService,
        SequenceEngine sequenceEngine,
        IInputProvider inputProvider,
        InputRecordingService recordingService,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _profileService = profileService;
        _sequenceEngine = sequenceEngine;
        _inputProvider = inputProvider;
        _recordingService = recordingService;
        _logger = logger;

        _recordingService.RecordingStateChanged += (s, active) => IsRecording = active;
        _recordingService.StepRecorded += (s, step) =>
        {
            App.Current?.Dispatcher?.Invoke(() => Steps.Add(step));
        };
    }

    public void LoadCheatForEditing(CheatDefinition cheat)
    {
        _editingCheat = cheat;
        _isNewCheat = false;

        EditorTitle = $"Editing Cheat: {cheat.Name}";
        Name = cheat.Name;
        Description = cheat.Description;
        Category = cheat.Category;
        RequiresMasterCode = cheat.RequiresMasterCode;
        AssignedHotkey = cheat.AssignedHotkey ?? string.Empty;
        DefaultDelayMs = cheat.DefaultDelayMs > 0 ? cheat.DefaultDelayMs : 70;

        Steps.Clear();
        foreach (var step in cheat.Sequence)
        {
            Steps.Add(step.Clone());
        }

        TestOutputLog = $"Loaded cheat '{cheat.Name}' with {Steps.Count} steps.";
    }

    public void CreateNewCheat()
    {
        _editingCheat = new CheatDefinition();
        _isNewCheat = true;

        EditorTitle = "Create New Cheat";
        Name = "New Cheat Sequence";
        Description = "Custom input sequence";
        Category = "General";
        RequiresMasterCode = false;
        AssignedHotkey = string.Empty;
        DefaultDelayMs = 70;
        DefaultHoldMs = 70;

        Steps.Clear();
        TestOutputLog = "Ready. Add steps using the buttons below or record inputs.";
    }

    [RelayCommand]
    public void AddAction(string actionName)
    {
        var step = new SequenceStep
        {
            ActionName = actionName.ToUpperInvariant(),
            ActionType = StepActionType.Tap,
            HoldDurationMs = DefaultHoldMs > 0 ? DefaultHoldMs : 70,
            DelayAfterMs = DefaultDelayMs > 0 ? DefaultDelayMs : 70
        };

        Steps.Add(step);
        SelectedStep = step;
        TestOutputLog = $"Added step: {step.ActionName}";
    }

    [RelayCommand]
    public void AddDelayStep()
    {
        var step = new SequenceStep
        {
            ActionName = "DELAY",
            ActionType = StepActionType.Delay,
            HoldDurationMs = 150,
            DelayAfterMs = 0,
            Comment = "Pause"
        };

        Steps.Add(step);
        SelectedStep = step;
    }

    [RelayCommand]
    public void RemoveStep(SequenceStep? step)
    {
        var target = step ?? SelectedStep;
        if (target != null)
        {
            Steps.Remove(target);
            SelectedStep = Steps.LastOrDefault();
        }
    }

    [RelayCommand]
    public void MoveStepUp(SequenceStep? step)
    {
        var target = step ?? SelectedStep;
        if (target == null) return;

        int index = Steps.IndexOf(target);
        if (index > 0)
        {
            Steps.Move(index, index - 1);
            SelectedStep = target;
        }
    }

    [RelayCommand]
    public void MoveStepDown(SequenceStep? step)
    {
        var target = step ?? SelectedStep;
        if (target == null) return;

        int index = Steps.IndexOf(target);
        if (index >= 0 && index < Steps.Count - 1)
        {
            Steps.Move(index, index + 1);
            SelectedStep = target;
        }
    }

    [RelayCommand]
    public void DuplicateStep(SequenceStep? step)
    {
        var target = step ?? SelectedStep;
        if (target != null)
        {
            var clone = target.Clone();
            int index = Steps.IndexOf(target);
            Steps.Insert(index + 1, clone);
            SelectedStep = clone;
        }
    }

    [RelayCommand]
    public void ClearAllSteps()
    {
        Steps.Clear();
        TestOutputLog = "Cleared all steps.";
    }

    [RelayCommand]
    public void ToggleRecording()
    {
        if (IsRecording)
        {
            _recordingService.StopRecording();
            TestOutputLog = $"Recording stopped. Total steps: {Steps.Count}";
        }
        else
        {
            _recordingService.StartRecording();
            TestOutputLog = "Recording started! Press inputs on your keyboard or controller...";
        }
    }

    [RelayCommand]
    public async Task TestSequenceMock()
    {
        if (Steps.Count == 0)
        {
            TestOutputLog = "Cannot test: sequence is empty.";
            return;
        }

        var mock = new Infrastructure.Input.MockInputProvider { SimulateDelays = false };
        var mapping = _profileService.ActiveProfile?.InputMapping ?? InputMappingProfile.CreateDefaultPs2Mapping();

        TestOutputLog = $"--- In-App Test Run ({Steps.Count} steps) ---\n";

        var result = await _sequenceEngine.ExecuteRawSequenceAsync(Steps, mapping, DefaultDelayMs, DefaultHoldMs);

        foreach (var log in mock.LogHistory)
        {
            TestOutputLog += log + "\n";
        }

        TestOutputLog += $"\n✓ Result: {result.Message} (Execution time: {result.ElapsedTime.TotalMilliseconds:F1}ms)";
        _mainVM.ShowToast("Sequence simulated successfully.", "Success");
    }

    [RelayCommand]
    public async Task TestSequenceLive()
    {
        var profile = _profileService.ActiveProfile;
        if (profile == null)
        {
            TestOutputLog = "No active profile selected.";
            return;
        }

        if (Steps.Count == 0)
        {
            TestOutputLog = "Cannot test: sequence is empty.";
            return;
        }

        var tempCheat = new CheatDefinition
        {
            Name = string.IsNullOrWhiteSpace(Name) ? "Test Sequence" : Name,
            Sequence = Steps.Select(s => s.Clone()).ToList(),
            DefaultDelayMs = DefaultDelayMs,
            RequiresMasterCode = RequiresMasterCode
        };

        _mainVM.SetExecutionState(true, tempCheat.Name, "Sending live sequence to game...");
        try
        {
            var result = await _sequenceEngine.ExecuteCheatAsync(tempCheat, profile);
            if (result.Success)
            {
                TestOutputLog = $"✓ Live Execution Succeeded! ({result.TotalStepsExecuted} steps in {result.ElapsedTime.TotalMilliseconds:F0}ms)";
                _mainVM.ShowToast("✓ Live sequence sent to target!", "Success");
            }
            else
            {
                TestOutputLog = $"❌ Live Execution Failed: {result.Message}";
                _mainVM.ShowToast($"Failed: {result.Message}", "Error");
            }
        }
        finally
        {
            _mainVM.SetExecutionState(false);
        }
    }

    [RelayCommand]
    public async Task SaveCheat()
    {
        var profile = _profileService.ActiveProfile;
        if (profile == null)
        {
            _mainVM.ShowToast("No active profile to save to.", "Error");
            return;
        }

        if (string.IsNullOrWhiteSpace(Name))
        {
            _mainVM.ShowToast("Please enter a cheat name.", "Warning");
            return;
        }

        if (Steps.Count == 0)
        {
            _mainVM.ShowToast("Cannot save an empty sequence.", "Warning");
            return;
        }

        var target = _editingCheat ?? new CheatDefinition();
        target.Name = Name.Trim();
        target.Description = Description.Trim();
        target.Category = Category;
        target.RequiresMasterCode = RequiresMasterCode;
        target.AssignedHotkey = string.IsNullOrWhiteSpace(AssignedHotkey) ? null : AssignedHotkey.Trim();
        target.DefaultDelayMs = DefaultDelayMs > 0 ? DefaultDelayMs : 70;
        target.Sequence = Steps.Select(s => s.Clone()).ToList();

        if (RequiresMasterCode && !target.PrerequisiteCheatIds.Contains("master_code"))
        {
            target.PrerequisiteCheatIds.Add("master_code");
        }
        else if (!RequiresMasterCode)
        {
            target.PrerequisiteCheatIds.Remove("master_code");
        }

        if (_isNewCheat)
        {
            profile.Cheats.Add(target);
            _isNewCheat = false;
        }

        await _profileService.SaveProfileAsync(profile);
        _mainVM.CheatsVM.ApplyFilter();
        _mainVM.DashboardVM.RefreshDashboard();

        _mainVM.ShowToast($"Saved cheat '{target.Name}'", "Success");
        _mainVM.Navigate(NavigationPage.Cheats);
    }
}
