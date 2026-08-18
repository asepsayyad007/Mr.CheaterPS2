using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Services;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class CheatsViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ProfileService _profileService;
    private readonly SequenceEngine _sequenceEngine;
    private readonly AppLogger _logger;

    [ObservableProperty]
    private string _searchQuery = string.Empty;

    [ObservableProperty]
    private string _selectedCategory = "All";

    public ObservableCollection<string> Categories { get; } = new()
    {
        "All",
        "Favorites",
        "System",
        "Unlocks",
        "Money",
        "Gameplay",
        "Physics",
        "Combat"
    };

    public ObservableCollection<CheatDefinition> FilteredCheats { get; } = new();

    public CheatsViewModel(
        MainViewModel mainVM,
        ProfileService profileService,
        SequenceEngine sequenceEngine,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _profileService = profileService;
        _sequenceEngine = sequenceEngine;
        _logger = logger;

        _profileService.ActiveProfileChanged += (s, p) => ApplyFilter();
        ApplyFilter();
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilter();
    partial void OnSelectedCategoryChanged(string value) => ApplyFilter();

    public void ApplyFilter()
    {
        FilteredCheats.Clear();
        var profile = _profileService.ActiveProfile;
        if (profile == null)
            return;

        var query = SearchQuery.Trim();
        var cheats = profile.Cheats.AsEnumerable();

        if (SelectedCategory == "Favorites")
        {
            cheats = cheats.Where(c => c.IsFavorite);
        }
        else if (!string.IsNullOrEmpty(SelectedCategory) && SelectedCategory != "All")
        {
            cheats = cheats.Where(c => c.Category.Equals(SelectedCategory, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(query))
        {
            cheats = cheats.Where(c =>
                c.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(query, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var cheat in cheats)
        {
            FilteredCheats.Add(cheat);
        }
    }

    [RelayCommand]
    public async Task ExecuteCheat(CheatDefinition cheat)
    {
        if (cheat == null || _profileService.ActiveProfile == null)
            return;

        _mainVM.SetExecutionState(true, cheat.Name, "Preparing sequence...");
        try
        {
            var result = await _sequenceEngine.ExecuteCheatAsync(cheat, _profileService.ActiveProfile);
            if (result.Success)
            {
                _mainVM.ShowToast($"{cheat.Name} sequence sent successfully.", "Success");
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
    public void EditCheat(CheatDefinition cheat)
    {
        if (cheat == null)
            return;

        _mainVM.SequenceEditorVM.LoadCheatForEditing(cheat);
        _mainVM.Navigate(NavigationPage.SequenceEditor);
    }

    [RelayCommand]
    public async Task ToggleFavorite(CheatDefinition cheat)
    {
        if (cheat == null || _profileService.ActiveProfile == null)
            return;

        cheat.IsFavorite = !cheat.IsFavorite;
        await _profileService.SaveProfileAsync(_profileService.ActiveProfile);
        ApplyFilter();
    }

    [RelayCommand]
    public async Task DuplicateCheat(CheatDefinition cheat)
    {
        if (cheat == null || _profileService.ActiveProfile == null)
            return;

        var copy = cheat.Clone();
        _profileService.ActiveProfile.Cheats.Add(copy);
        await _profileService.SaveProfileAsync(_profileService.ActiveProfile);
        ApplyFilter();
        _mainVM.ShowToast($"Duplicated '{cheat.Name}'", "Info");
    }

    [RelayCommand]
    public async Task DeleteCheat(CheatDefinition cheat)
    {
        if (cheat == null || _profileService.ActiveProfile == null)
            return;

        _profileService.ActiveProfile.Cheats.Remove(cheat);
        await _profileService.SaveProfileAsync(_profileService.ActiveProfile);
        ApplyFilter();
        _mainVM.ShowToast($"Deleted '{cheat.Name}'", "Info");
    }

    [RelayCommand]
    public void AddNewCheat()
    {
        _mainVM.SequenceEditorVM.CreateNewCheat();
        _mainVM.Navigate(NavigationPage.SequenceEditor);
    }
}
