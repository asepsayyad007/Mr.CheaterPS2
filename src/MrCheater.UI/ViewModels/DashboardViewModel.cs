using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Services;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ProfileService _profileService;
    private readonly SequenceEngine _sequenceEngine;
    private readonly IProcessMonitor _processMonitor;
    private readonly IXInputControllerService _xInputService;
    private readonly AppLogger _logger;

    [ObservableProperty]
    private GameProfile? _activeProfile;

    [ObservableProperty]
    private GameProfile? _selectedProfileCombo;

    public ObservableCollection<GameProfile> AvailableProfiles { get; } = new();
    public ObservableCollection<CheatDefinition> QuickCheats { get; } = new();
    public ObservableCollection<LogEntry> RecentLogs => _logger.RecentLogs;

    public DashboardViewModel(
        MainViewModel mainVM,
        ProfileService profileService,
        SequenceEngine sequenceEngine,
        IProcessMonitor processMonitor,
        IXInputControllerService xInputService,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _profileService = profileService;
        _sequenceEngine = sequenceEngine;
        _processMonitor = processMonitor;
        _xInputService = xInputService;
        _logger = logger;

        _profileService.ActiveProfileChanged += (s, p) =>
        {
            App.Current?.Dispatcher?.BeginInvoke(() => RefreshDashboard());
        };

        RefreshDashboard();
    }

    public void RefreshDashboard()
    {
        ActiveProfile = _profileService.ActiveProfile;
        SelectedProfileCombo = ActiveProfile;

        AvailableProfiles.Clear();
        foreach (var p in _profileService.Profiles)
        {
            AvailableProfiles.Add(p);
        }

        QuickCheats.Clear();
        if (ActiveProfile != null)
        {
            var favs = ActiveProfile.Cheats.Where(c => c.IsFavorite).ToList();
            if (favs.Count == 0)
            {
                favs = ActiveProfile.Cheats.Take(6).ToList();
            }

            foreach (var cheat in favs)
            {
                QuickCheats.Add(cheat);
            }
        }
    }

    partial void OnSelectedProfileComboChanged(GameProfile? value)
    {
        if (value != null && value.Id != _profileService.ActiveProfile?.Id)
        {
            _profileService.SetActiveProfile(value);
        }
    }

    [RelayCommand]
    public async Task ExecuteQuickCheat(CheatDefinition cheat)
    {
        if (cheat == null || ActiveProfile == null)
            return;

        _mainVM.SetExecutionState(true, cheat.Name, "Preparing sequence...");
        try
        {
            var result = await _sequenceEngine.ExecuteCheatAsync(cheat, ActiveProfile);
            if (result.Success)
            {
                _mainVM.ShowToast($"{cheat.Name} sent successfully!", "Success");
            }
            else if (result.WasCancelled)
            {
                _mainVM.ShowToast("Execution cancelled.", "Warning");
            }
            else
            {
                _mainVM.ShowToast($"Failed: {result.Message}", "Error");
            }
        }
        finally
        {
            _mainVM.SetExecutionState(false);
        }
    }

    [RelayCommand]
    public void OpenCheatLibrary()
    {
        _mainVM.Navigate(NavigationPage.Cheats);
    }

    [RelayCommand]
    public void CreateNewCheat()
    {
        _mainVM.SequenceEditorVM.CreateNewCheat();
        _mainVM.Navigate(NavigationPage.SequenceEditor);
    }
}
