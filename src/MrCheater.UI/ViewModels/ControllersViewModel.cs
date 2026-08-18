using System.Collections.ObjectModel;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Core.Services;
using MrCheater.Domain.Interfaces;
using MrCheater.Domain.Models;

namespace MrCheater.UI.ViewModels;

public partial class ControllersViewModel : ObservableObject
{
    private readonly MainViewModel _mainVM;
    private readonly ProfileService _profileService;
    private readonly IXInputControllerService _xInputService;
    private readonly AppLogger _logger;

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _controllerName = "No Controller Connected";

    [ObservableProperty]
    private string _activeButtonsDisplay = "None";

    // Live controller state bindings
    [ObservableProperty] private bool _padUp;
    [ObservableProperty] private bool _padDown;
    [ObservableProperty] private bool _padLeft;
    [ObservableProperty] private bool _padRight;
    [ObservableProperty] private bool _btnA;
    [ObservableProperty] private bool _btnB;
    [ObservableProperty] private bool _btnX;
    [ObservableProperty] private bool _btnY;
    [ObservableProperty] private bool _btnLb;
    [ObservableProperty] private bool _btnRb;
    [ObservableProperty] private bool _btnStart;
    [ObservableProperty] private bool _btnBack;
    [ObservableProperty] private bool _btnLs;
    [ObservableProperty] private bool _btnRs;

    public ObservableCollection<InputMappingEntry> MappingEntries { get; } = new();

    public ControllersViewModel(
        MainViewModel mainVM,
        ProfileService profileService,
        IXInputControllerService xInputService,
        AppLogger logger)
    {
        _mainVM = mainVM;
        _profileService = profileService;
        _xInputService = xInputService;
        _logger = logger;

        _xInputService.StatePolled += OnControllerPolled;
        _profileService.ActiveProfileChanged += (s, p) => RefreshMappings();
        RefreshMappings();
    }

    public void RefreshMappings()
    {
        MappingEntries.Clear();
        var profile = _profileService.ActiveProfile;
        if (profile != null)
        {
            foreach (var kvp in profile.InputMapping.Mappings)
            {
                MappingEntries.Add(kvp.Value);
            }
        }
    }

    private void OnControllerPolled(object? sender, ControllerStateSnapshot state)
    {
        var app = System.Windows.Application.Current;
        if (app?.Dispatcher == null || app.Dispatcher.HasShutdownStarted)
            return;

        app.Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
        {
            IsConnected = state.IsConnected;
            ControllerName = state.IsConnected ? "XInput Controller 1 (Xbox Standard)" : "No Controller Detected";

            PadUp = state.DPadUp;
            PadDown = state.DPadDown;
            PadLeft = state.DPadLeft;
            PadRight = state.DPadRight;

            BtnA = state.A;
            BtnB = state.B;
            BtnX = state.X;
            BtnY = state.Y;

            BtnLb = state.LeftShoulder;
            BtnRb = state.RightShoulder;
            BtnStart = state.Start;
            BtnBack = state.Back;
            BtnLs = state.LeftThumb;
            BtnRs = state.RightThumb;

            var active = state.GetActiveButtonNames();
            ActiveButtonsDisplay = active.Count > 0 ? string.Join(" + ", active) : "None";
        });
    }

    [RelayCommand]
    public async Task SaveMappings()
    {
        var profile = _profileService.ActiveProfile;
        if (profile == null) return;

        await _profileService.SaveProfileAsync(profile);
        _mainVM.ShowToast("Saved input mappings.", "Success");
    }

    [RelayCommand]
    public async Task ResetToDefaultPs2()
    {
        var profile = _profileService.ActiveProfile;
        if (profile == null) return;

        profile.InputMapping = InputMappingProfile.CreateDefaultPs2Mapping();
        await _profileService.SaveProfileAsync(profile);
        RefreshMappings();
        _mainVM.ShowToast("Reset to standard PS2 keyboard & controller mappings.", "Info");
    }
}
