using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MrCheater.Core.Services;
using MrCheater.Domain.Interfaces;
using MrCheater.Infrastructure.Input;
using MrCheater.Infrastructure.Storage;
using MrCheater.Infrastructure.System;
using MrCheater.UI.ViewModels;
using MrCheater.UI.Views;

namespace MrCheater.UI;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    private async void OnAppStartup(object sender, StartupEventArgs e)
    {
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        AppLogger.Instance.SetUiDispatcher(action =>
        {
            if (Current?.Dispatcher != null && !Current.Dispatcher.HasShutdownStarted)
            {
                Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, action);
            }
        });

        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        var settingsRepo = ServiceProvider.GetRequiredService<ISettingsRepository>();
        var settings = await settingsRepo.LoadSettingsAsync();

        var xinput = ServiceProvider.GetRequiredService<IXInputControllerService>();

        if (!settings.IsFirstRunCompleted)
        {
            var wizardVm = new FirstRunWizardViewModel(settingsRepo, xinput);
            var wizard = new FirstRunWizardWindow(wizardVm);
            wizard.ShowDialog();
        }

        var mainVm = ServiceProvider.GetRequiredService<MainViewModel>();
        await mainVm.InitializeAsync();

        var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
        MainWindow = mainWindow;
        ShutdownMode = ShutdownMode.OnMainWindowClose;
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(AppLogger.Instance);
        services.AddSingleton<PrerequisiteResolver>();

        services.AddSingleton<IProfileRepository, JsonProfileRepository>();
        services.AddSingleton<ISettingsRepository, JsonSettingsRepository>();
        services.AddSingleton<IEmulatorRepository, JsonEmulatorRepository>();

        services.AddSingleton<IInputProvider, WindowsSendInputProvider>();
        services.AddSingleton<IXInputControllerService, XInputControllerService>();
        services.AddSingleton<IProcessMonitor, WindowsProcessMonitor>();

        var hotkeyManager = new WindowsGlobalHotkeyManager();
        services.AddSingleton(hotkeyManager);
        services.AddSingleton<IGlobalHotkeyService>(hotkeyManager);

        services.AddSingleton<EmergencyStopManager>();
        services.AddSingleton<SequenceEngine>();
        services.AddSingleton<ProfileService>();
        services.AddSingleton<InputRecordingService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
    }
}
