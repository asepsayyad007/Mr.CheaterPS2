using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Common;
using MrCheater.Core.Services;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IGlobalHotkeyService _hotkeyService;
    private readonly ProfileService _profileService;
    private readonly EmergencyStopManager _emergencyStopManager;
    private readonly AppLogger _logger;

    [ObservableProperty] private bool _requireTargetProcessRunning = true;
    [ObservableProperty] private bool _requireTargetWindowFocus = true;
    [ObservableProperty] private bool _allowBackgroundInput = false;

    [ObservableProperty] private string _emergencyStopHotkey = "Ctrl+Shift+Escape";
    [ObservableProperty] private int _maxSequenceDurationSeconds = 15;
    [ObservableProperty] private int _defaultGlobalDelayMs = 70;
    [ObservableProperty] private int _defaultHoldDurationMs = 70;

    [ObservableProperty] private bool _minimizeToTray = true;
    [ObservableProperty] private bool _startWithWindows = false;
    [ObservableProperty] private bool _startMinimized = false;
    [ObservableProperty] private bool _enableToastNotifications = true;
    [ObservableProperty] private bool _enableSoundEffects = true;

    public SettingsViewModel(
        MainViewModel mainVM,
        ISettingsRepository settingsRepository,
        IGlobalHotkeyService hotkeyService,
        ProfileService profileService,
        EmergencyStopManager emergencyStopManager,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _settingsRepository = settingsRepository;
        _hotkeyService = hotkeyService;
        _profileService = profileService;
        _emergencyStopManager = emergencyStopManager;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        var s = await _settingsRepository.LoadSettingsAsync();
        RequireTargetProcessRunning = s.RequireTargetProcessRunning;
        RequireTargetWindowFocus = s.RequireTargetWindowFocus;
        AllowBackgroundInput = s.AllowBackgroundInput;
        EmergencyStopHotkey = s.EmergencyStopHotkey;
        MaxSequenceDurationSeconds = s.MaxSequenceDurationSeconds;
        DefaultGlobalDelayMs = s.DefaultGlobalDelayMs;
        DefaultHoldDurationMs = s.DefaultHoldDurationMs;
        MinimizeToTray = s.MinimizeToTray;
        StartWithWindows = s.StartWithWindows;
        StartMinimized = s.StartMinimized;
        EnableToastNotifications = s.EnableToastNotifications;
        EnableSoundEffects = s.EnableSoundEffects;

        RegisterConfiguredHotkeys();
    }

    public void RegisterConfiguredHotkeys()
    {
        _hotkeyService.UnregisterAll();

        // 1. Register Emergency Stop Hotkey
        if (!string.IsNullOrWhiteSpace(EmergencyStopHotkey))
        {
            _hotkeyService.RegisterHotkey(EmergencyStopHotkey, () =>
            {
                _emergencyStopManager.TriggerEmergencyStop();
                _mainVM.ShowToast("EMERGENCY STOP TRIGGERED via global hotkey!", "Warning");
            });
        }

        // 2. Register Active Profile cheat & macro hotkeys
        var profile = _profileService.ActiveProfile;
        if (profile != null)
        {
            foreach (var cheat in profile.Cheats.Where(c => !string.IsNullOrWhiteSpace(c.AssignedHotkey)))
            {
                _hotkeyService.RegisterHotkey(cheat.AssignedHotkey!, () =>
                {
                    _ = _mainVM.CheatsVM.ExecuteCheat(cheat);
                });
            }

            foreach (var macro in profile.Macros.Where(m => !string.IsNullOrWhiteSpace(m.AssignedHotkey)))
            {
                _hotkeyService.RegisterHotkey(macro.AssignedHotkey!, () =>
                {
                    _ = _mainVM.MacrosVM.ExecuteMacro(macro);
                });
            }
        }
    }

    [RelayCommand]
    public async Task SaveSettings()
    {
        var s = new AppSettings
        {
            RequireTargetProcessRunning = RequireTargetProcessRunning,
            RequireTargetWindowFocus = RequireTargetWindowFocus,
            AllowBackgroundInput = AllowBackgroundInput,
            EmergencyStopHotkey = EmergencyStopHotkey,
            MaxSequenceDurationSeconds = MaxSequenceDurationSeconds,
            DefaultGlobalDelayMs = DefaultGlobalDelayMs,
            DefaultHoldDurationMs = DefaultHoldDurationMs,
            MinimizeToTray = MinimizeToTray,
            StartWithWindows = StartWithWindows,
            StartMinimized = StartMinimized,
            EnableToastNotifications = EnableToastNotifications,
            EnableSoundEffects = EnableSoundEffects,
            SelectedProfileId = _profileService.ActiveProfile?.Id
        };

        await _settingsRepository.SaveSettingsAsync(s);
        RegisterConfiguredHotkeys();
        _mainVM.ShowToast("Settings saved successfully.", "Success");
    }

    [RelayCommand]
    public void OpenDataFolder()
    {
        var path = StorageHelper.GetBaseDirectory();
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _mainVM.ShowToast($"Could not open folder: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    public void OpenLogsFolder()
    {
        var path = StorageHelper.GetLogsDirectory();
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            _mainVM.ShowToast($"Could not open folder: {ex.Message}", "Error");
        }
    }
}
