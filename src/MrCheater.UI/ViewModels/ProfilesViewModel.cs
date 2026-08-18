using System.Collections.ObjectModel;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MrCheater.Core.Services;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class ProfilesViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ProfileService _profileService;
    private readonly IProfileRepository _profileRepository;
    private readonly IEmulatorRepository _emulatorRepository;
    private readonly AppLogger _logger;

    [ObservableProperty]
    private GameProfile? _selectedProfile;

    public ObservableCollection<GameProfile> Profiles => new(_profileService.Profiles);
    public ObservableCollection<EmulatorProfile> AvailableEmulators { get; } = new();

    public ProfilesViewModel(
        MainViewModel mainVM,
        ProfileService profileService,
        IProfileRepository profileRepository,
        IEmulatorRepository emulatorRepository,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _profileService = profileService;
        _profileRepository = profileRepository;
        _emulatorRepository = emulatorRepository;
        _logger = logger;

        _profileService.ActiveProfileChanged += (s, p) =>
        {
            SelectedProfile = p;
            OnPropertyChanged(nameof(Profiles));
        };
    }

    public async Task InitializeAsync()
    {
        var emulators = await _emulatorRepository.GetAllEmulatorsAsync();
        AvailableEmulators.Clear();
        foreach (var emu in emulators)
        {
            AvailableEmulators.Add(emu);
        }

        SelectedProfile = _profileService.ActiveProfile;
        OnPropertyChanged(nameof(Profiles));
    }

    [RelayCommand]
    public void SetActive(GameProfile? profile)
    {
        if (profile != null)
        {
            _profileService.SetActiveProfile(profile);
            SelectedProfile = profile;
            _mainVM.ShowToast($"Activated profile: '{profile.Name}'", "Success");
        }
    }

    [RelayCommand]
    public async Task CreateNewProfile()
    {
        var newProfile = new GameProfile
        {
            Name = "New Game Profile",
            EmulatorName = "PCSX2",
            TargetProcess = "pcsx2-qt.exe",
            InputMapping = InputMappingProfile.CreateDefaultPs2Mapping(),
            DefaultStepDelayMs = 70,
            DefaultHoldDurationMs = 70
        };

        await _profileService.SaveProfileAsync(newProfile);
        _profileService.SetActiveProfile(newProfile);
        SelectedProfile = newProfile;
        OnPropertyChanged(nameof(Profiles));
        _mainVM.ShowToast("Created new profile.", "Success");
    }

    [RelayCommand]
    public async Task DuplicateProfile(GameProfile? profile)
    {
        var target = profile ?? SelectedProfile;
        if (target == null) return;

        var copy = await _profileService.DuplicateProfileAsync(target);
        _profileService.SetActiveProfile(copy);
        SelectedProfile = copy;
        OnPropertyChanged(nameof(Profiles));
        _mainVM.ShowToast($"Duplicated profile '{target.Name}'", "Info");
    }

    [RelayCommand]
    public async Task DeleteProfile(GameProfile? profile)
    {
        var target = profile ?? SelectedProfile;
        if (target == null) return;

        if (_profileService.Profiles.Count <= 1)
        {
            _mainVM.ShowToast("Cannot delete the only profile.", "Warning");
            return;
        }

        await _profileService.DeleteProfileAsync(target.Id);
        SelectedProfile = _profileService.ActiveProfile;
        OnPropertyChanged(nameof(Profiles));
        _mainVM.ShowToast($"Deleted profile '{target.Name}'", "Info");
    }

    [RelayCommand]
    public async Task SaveSelectedProfile()
    {
        if (SelectedProfile == null) return;

        await _profileService.SaveProfileAsync(SelectedProfile);
        OnPropertyChanged(nameof(Profiles));
        _mainVM.ShowToast($"Saved profile '{SelectedProfile.Name}'", "Success");
    }

    [RelayCommand]
    public async Task ExportProfile(GameProfile? profile)
    {
        var target = profile ?? SelectedProfile;
        if (target == null) return;

        var saveDialog = new SaveFileDialog
        {
            Filter = "JSON Profile (*.json)|*.json",
            FileName = $"{target.Name.Replace(" ", "_")}_profile.json"
        };

        if (saveDialog.ShowDialog() == true)
        {
            var json = _profileRepository.ExportProfileToJson(target);
            await File.WriteAllTextAsync(saveDialog.FileName, json);
            _mainVM.ShowToast($"Profile exported to {Path.GetFileName(saveDialog.FileName)}", "Success");
        }
    }

    [RelayCommand]
    public async Task ImportProfile()
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "JSON Profiles (*.json)|*.json",
            Multiselect = false
        };

        if (openDialog.ShowDialog() == true)
        {
            try
            {
                var json = await File.ReadAllTextAsync(openDialog.FileName);
                var imported = await _profileRepository.ImportProfilesBundleAsync(json);
                if (imported.Count > 0)
                {
                    _profileService.SetActiveProfile(imported.First());
                    SelectedProfile = imported.First();
                    OnPropertyChanged(nameof(Profiles));
                    _mainVM.ShowToast($"Imported {imported.Count} profile(s) successfully.", "Success");
                }
            }
            catch (Exception ex)
            {
                _mainVM.ShowToast($"Failed to import: {ex.Message}", "Error");
            }
        }
    }
}
