using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Services;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public enum NavigationPage
{
    Dashboard,
    Cheats,
    SequenceEditor,
    Macros,
    Controllers,
    Profiles,
    Settings
}

public partial class MainViewModel : ObservableObject
{
    private readonly ProfileService _profileService;
    private readonly IProcessMonitor _processMonitor;
    private readonly IXInputControllerService _xInputService;
    private readonly SequenceEngine _sequenceEngine;
    private readonly EmergencyStopManager _emergencyStopManager;
    private readonly ISettingsRepository _settingsRepository;
    private readonly AppLogger _logger;
    private readonly Dispatcher _dispatcher;

    [ObservableProperty]
    private NavigationPage _currentPage = NavigationPage.Dashboard;

    [ObservableProperty]
    private ObservableObject _currentViewModel = null!;

    [ObservableProperty]
    private GameProfile? _activeProfile;

    [ObservableProperty]
    private TargetProcessStatus _targetProcessStatus = TargetProcessStatus.NotRunning;

    [ObservableProperty]
    private string _targetProcessText = "Not Running";

    [ObservableProperty]
    private bool _isControllerConnected = false;

    [ObservableProperty]
    private string _controllerStatusText = "Disconnected";

    [ObservableProperty]
    private bool _isMasterCodeActive = false;

    [ObservableProperty]
    private string _masterCodeStatusText = "Master: Not Applied";

    [ObservableProperty]
    private bool _isExecutingSequence = false;

    [ObservableProperty]
    private string _executingSequenceTitle = string.Empty;

    [ObservableProperty]
    private double _executionProgressPercentage = 0;

    [ObservableProperty]
    private string _executionStatusMessage = string.Empty;

    [ObservableProperty]
    private string _toastMessage = string.Empty;

    [ObservableProperty]
    private bool _isToastVisible = false;

    [ObservableProperty]
    private string _toastType = "Info";

    private DispatcherTimer? _toastTimer;

    public DashboardViewModel DashboardVM { get; }
    public CheatsViewModel CheatsVM { get; }
    public SequenceEditorViewModel SequenceEditorVM { get; }
    public MacrosViewModel MacrosVM { get; }
    public ControllersViewModel ControllersVM { get; }
    public ProfilesViewModel ProfilesVM { get; }
    public SettingsViewModel SettingsVM { get; }

    public MainViewModel(
        ProfileService profileService,
        IProcessMonitor processMonitor,
        IXInputControllerService xInputService,
        SequenceEngine sequenceEngine,
        EmergencyStopManager emergencyStopManager,
        ISettingsRepository settingsRepository,
        IProfileRepository profileRepository,
        IEmulatorRepository emulatorRepository,
        IGlobalHotkeyService hotkeyService,
        InputRecordingService recordingService,
        IInputProvider inputProvider,
        AppLogger logger)
    {
        _profileService = profileService;
        _processMonitor = processMonitor;
        _xInputService = xInputService;
        _sequenceEngine = sequenceEngine;
        _emergencyStopManager = emergencyStopManager;
        _settingsRepository = settingsRepository;
        _logger = logger;
        _dispatcher = Dispatcher.CurrentDispatcher;

        DashboardVM = new DashboardViewModel(this, profileService, sequenceEngine, processMonitor, xInputService, logger);
        CheatsVM = new CheatsViewModel(this, profileService, sequenceEngine, logger);
        SequenceEditorVM = new SequenceEditorViewModel(this, profileService, sequenceEngine, inputProvider, recordingService, logger);
        MacrosVM = new MacrosViewModel(this, profileService, sequenceEngine, logger);
        ControllersVM = new ControllersViewModel(this, profileService, xInputService, logger);
        ProfilesVM = new ProfilesViewModel(this, profileService, profileRepository, emulatorRepository, logger);
        SettingsVM = new SettingsViewModel(this, settingsRepository, hotkeyService, profileService, emergencyStopManager, logger);

        CurrentViewModel = DashboardVM;

        _profileService.ActiveProfileChanged += OnActiveProfileChanged;
        _processMonitor.StatusChanged += OnProcessStatusChanged;
        _xInputService.ConnectionChanged += OnControllerConnectionChanged;
        _sequenceEngine.ProgressChanged += OnSequenceProgressChanged;
        _sequenceEngine.MasterCodeStateChanged += OnMasterCodeStateChanged;
        _emergencyStopManager.EmergencyStopTriggered += OnEmergencyStopTriggered;
    }

    public async Task InitializeAsync()
    {
        await _profileService.InitializeAsync();
        ActiveProfile = _profileService.ActiveProfile;

        if (ActiveProfile != null)
        {
            _processMonitor.SetWatchedTargets(new[] { ActiveProfile.TargetProcess }, ActiveProfile.WindowTitlePattern);
            UpdateMasterCodeDisplay(ActiveProfile.Id);
        }

        _processMonitor.Start();
        _xInputService.StartPolling();

        IsControllerConnected = _xInputService.IsControllerConnected(0);
        ControllerStatusText = _xInputService.GetControllerName(0);

        await SettingsVM.InitializeAsync();
        await ProfilesVM.InitializeAsync();
        DashboardVM.RefreshDashboard();
    }

    [RelayCommand]
    public void Navigate(NavigationPage page)
    {
        CurrentPage = page;
        CurrentViewModel = page switch
        {
            NavigationPage.Dashboard => DashboardVM,
            NavigationPage.Cheats => CheatsVM,
            NavigationPage.SequenceEditor => SequenceEditorVM,
            NavigationPage.Macros => MacrosVM,
            NavigationPage.Controllers => ControllersVM,
            NavigationPage.Profiles => ProfilesVM,
            NavigationPage.Settings => SettingsVM,
            _ => DashboardVM
        };
    }

    [RelayCommand]
    public void ToggleMasterCodeState()
    {
        if (ActiveProfile == null) return;
        bool newState = !_sequenceEngine.IsMasterCodeActive(ActiveProfile.Id);
        _sequenceEngine.SetMasterCodeActive(ActiveProfile.Id, newState);
        ShowToast($"Master Code marked as {(newState ? "ACTIVE (Unlocked)" : "INACTIVE (Locked)")}", "Info");
    }

    [RelayCommand]
    public void TriggerEmergencyStop()
    {
        _emergencyStopManager.TriggerEmergencyStop();
        ShowToast("EMERGENCY STOP TRIGGERED! All inputs released.", "Warning");
    }

    public void ShowToast(string message, string type = "Info", int durationSeconds = 3)
    {
        if (_dispatcher.HasShutdownStarted) return;

        _dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
        {
            ToastMessage = message;
            ToastType = type;
            IsToastVisible = true;

            _toastTimer?.Stop();
            _toastTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(durationSeconds) };
            _toastTimer.Tick += (s, e) =>
            {
                IsToastVisible = false;
                _toastTimer?.Stop();
            };
            _toastTimer.Start();
        });
    }

    public void SetExecutionState(bool isExecuting, string title = "", string message = "", double progress = 0)
    {
        if (_dispatcher.HasShutdownStarted) return;

        _dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
        {
            IsExecutingSequence = isExecuting;
            ExecutingSequenceTitle = title;
            ExecutionStatusMessage = message;
            ExecutionProgressPercentage = progress;
        });
    }

    private void OnActiveProfileChanged(object? sender, GameProfile? profile)
    {
        if (_dispatcher.HasShutdownStarted) return;

        _dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
        {
            ActiveProfile = profile;
            if (profile != null)
            {
                _processMonitor.SetWatchedTargets(new[] { profile.TargetProcess }, profile.WindowTitlePattern);
                UpdateMasterCodeDisplay(profile.Id);
            }
        });
    }

    private void OnMasterCodeStateChanged(object? sender, (string ProfileId, bool Active) e)
    {
        if (_dispatcher.HasShutdownStarted) return;

        _dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
        {
            if (ActiveProfile != null && ActiveProfile.Id.Equals(e.ProfileId, StringComparison.OrdinalIgnoreCase))
            {
                UpdateMasterCodeDisplay(e.ProfileId);
            }
        });
    }

    private void UpdateMasterCodeDisplay(string profileId)
    {
        IsMasterCodeActive = _sequenceEngine.IsMasterCodeActive(profileId);
        MasterCodeStatusText = IsMasterCodeActive ? "Master: UNLOCKED" : "Master: Not Applied";
    }

    private void OnProcessStatusChanged(object? sender, TargetProcessStatus status)
    {
        if (_dispatcher.HasShutdownStarted) return;

        _dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
        {
            TargetProcessStatus = status;
            TargetProcessText = status switch
            {
                TargetProcessStatus.Focused => "Active & Focused",
                TargetProcessStatus.Running => "Running (Background)",
                _ => "Not Running"
            };
        });
    }

    private void OnControllerConnectionChanged(object? sender, bool isConnected)
    {
        if (_dispatcher.HasShutdownStarted) return;

        _dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
        {
            IsControllerConnected = isConnected;
            ControllerStatusText = _xInputService.GetControllerName(0);
        });
    }

    private void OnSequenceProgressChanged(object? sender, ExecutionProgress e)
    {
        SetExecutionState(true, e.SequenceName, e.StatusMessage, e.ProgressPercentage);
    }

    private void OnEmergencyStopTriggered(object? sender, EventArgs e)
    {
        SetExecutionState(false);
    }
}
