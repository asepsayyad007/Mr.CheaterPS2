using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using MrCheater.Infrastructure.System;
using MrCheater.UI.ViewModels;

namespace MrCheater.UI;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly WindowsGlobalHotkeyManager _hotkeyManager;
    private HwndSource? _hwndSource;

    public MainWindow(MainViewModel viewModel, WindowsGlobalHotkeyManager hotkeyManager)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _hotkeyManager = hotkeyManager;
        DataContext = _viewModel;

        Loaded += OnWindowLoaded;
        Closing += OnWindowClosing;
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        var helper = new WindowInteropHelper(this);
        var hwnd = helper.Handle;

        _hwndSource = HwndSource.FromHwnd(hwnd);
        _hwndSource?.AddHook(WndProc);

        _hotkeyManager.SetHwnd(hwnd);
        _viewModel.SettingsVM.RegisterConfiguredHotkeys();
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (_hotkeyManager.ProcessWindowMessage(msg, wParam, lParam))
        {
            handled = true;
            return IntPtr.Zero;
        }
        return IntPtr.Zero;
    }

    private void OnWindowClosing(object? sender, CancelEventArgs e)
    {
        if (_viewModel.SettingsVM.MinimizeToTray)
        {
            e.Cancel = true;
            Hide();
            _viewModel.ShowToast("Minimized to System Tray", "Info");
        }
    }
}