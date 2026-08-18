using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Interfaces;

namespace MrCheater.UI.ViewModels;

public partial class FirstRunWizardViewModel : ObservableObject
{
    private readonly ISettingsRepository _settingsRepository;
    private readonly IXInputControllerService _xInputService;

    [ObservableProperty]
    private int _currentStep = 1;

    [ObservableProperty]
    private InputDeviceType _selectedInputDevice = InputDeviceType.Both;

    [ObservableProperty]
    private string _selectedEmulator = "PCSX2";

    [ObservableProperty]
    private bool _isControllerDetected = false;

    public event EventHandler? WizardCompleted;

    public FirstRunWizardViewModel(
        ISettingsRepository settingsRepository,
        IXInputControllerService xInputService)
    {
        _settingsRepository = settingsRepository;
        _xInputService = xInputService;
        IsControllerDetected = _xInputService.IsControllerConnected(0);
    }

    [RelayCommand]
    public async Task NextStep()
    {
        if (CurrentStep < 3)
        {
            CurrentStep++;
        }
        else
        {
            await CompleteWizard();
        }
    }

    [RelayCommand]
    public void PreviousStep()
    {
        if (CurrentStep > 1)
        {
            CurrentStep--;
        }
    }

    [RelayCommand]
    public async Task CompleteWizard()
    {
        var settings = await _settingsRepository.LoadSettingsAsync();
        settings.IsFirstRunCompleted = true;
        await _settingsRepository.SaveSettingsAsync(settings);
        WizardCompleted?.Invoke(this, EventArgs.Empty);
    }
}
