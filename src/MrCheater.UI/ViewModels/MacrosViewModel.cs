using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Services;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class MacrosViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ProfileService _profileService;
    private readonly SequenceEngine _sequenceEngine;
    private readonly AppLogger _logger;

    public ObservableCollection<MacroDefinition> Macros { get; } = new();

    public MacrosViewModel(
        MainViewModel mainVM,
        ProfileService profileService,
        SequenceEngine sequenceEngine,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _profileService = profileService;
        _sequenceEngine = sequenceEngine;
        _logger = logger;

        _profileService.ActiveProfileChanged += (s, p) => RefreshMacros();
        RefreshMacros();
    }

    public void RefreshMacros()
    {
        Macros.Clear();
        var profile = _profileService.ActiveProfile;
        if (profile != null)
        {
            foreach (var m in profile.Macros)
            {
                Macros.Add(m);
            }
        }
    }

    [RelayCommand]
    public async Task ExecuteMacro(MacroDefinition macro)
    {
        var profile = _profileService.ActiveProfile;
        if (macro == null || profile == null)
            return;

        _mainVM.SetExecutionState(true, macro.Name, "Running macro...");
        try
        {
            var result = await _sequenceEngine.ExecuteMacroAsync(macro, profile);
            if (result.Success)
            {
                _mainVM.ShowToast($"✓ Macro '{macro.Name}' completed.", "Success");
            }
            else if (result.WasCancelled)
            {
                _mainVM.ShowToast($"⚠ Macro '{macro.Name}' was stopped.", "Warning");
            }
            else
            {
                _mainVM.ShowToast($"❌ Macro failed: {result.Message}", "Error");
            }
        }
        finally
        {
            _mainVM.SetExecutionState(false);
        }
    }

    [RelayCommand]
    public async Task AddNewMacro()
    {
        var profile = _profileService.ActiveProfile;
        if (profile == null) return;

        var newMacro = new MacroDefinition
        {
            Name = "New Combo Macro",
            Description = "Multi-step automated key sequence",
            LoopCount = 1,
            Sequence = new List<SequenceStep>
            {
                new() { ActionName = "CROSS", HoldDurationMs = 70, DelayAfterMs = 100 },
                new() { ActionName = "TRIANGLE", HoldDurationMs = 70, DelayAfterMs = 70 }
            }
        };

        profile.Macros.Add(newMacro);
        await _profileService.SaveProfileAsync(profile);
        RefreshMacros();
        _mainVM.ShowToast("Created new macro.", "Success");
    }

    [RelayCommand]
    public async Task DeleteMacro(MacroDefinition macro)
    {
        var profile = _profileService.ActiveProfile;
        if (macro == null || profile == null) return;

        profile.Macros.Remove(macro);
        await _profileService.SaveProfileAsync(profile);
        RefreshMacros();
        _mainVM.ShowToast($"Deleted macro '{macro.Name}'", "Info");
    }
}
