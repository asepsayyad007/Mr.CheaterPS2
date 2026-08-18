using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using MrCheater.Domain.Enums;
using MrCheater.Domain.Models;

namespace MrCheater.UI.Converters;

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrWhiteSpace(value as string) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw freshNotImplemented();
    private static NotImplementedException freshNotImplemented() => new();
}

public class BooleanToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool b = value is bool flag && flag;
        if (Invert) b = !b;
        return b ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class TargetStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is TargetProcessStatus status)
        {
            return status switch
            {
                TargetProcessStatus.Focused => new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50)), // Green
                TargetProcessStatus.Running => new SolidColorBrush(Color.FromRgb(0x00, 0xB4, 0xD8)), // Cyan
                _ => new SolidColorBrush(Color.FromRgb(0x6E, 0x76, 0x81))                            // Gray
            };
        }
        return new SolidColorBrush(Color.FromRgb(0x6E, 0x76, 0x81));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class ControllerStatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool connected = value is bool b && b;
        return connected
            ? new SolidColorBrush(Color.FromRgb(0x3F, 0xB9, 0x50)) // Connected green
            : new SolidColorBrush(Color.FromRgb(0x6E, 0x76, 0x81)); // Disconnected gray
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public class SequenceToPreviewConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IEnumerable<SequenceStep> steps)
        {
            var formatted = steps.Select(s => FormatStep(s.ActionName));
            return string.Join("  ", formatted);
        }
        return string.Empty;
    }

    private static string FormatStep(string action)
    {
        return action.ToUpperInvariant() switch
        {
            "UP" => "[↑]",
            "DOWN" => "[↓]",
            "LEFT" => "[←]",
            "RIGHT" => "[→]",
            "TRIANGLE" => "[△]",
            "CIRCLE" => "[◯]",
            "CROSS" => "[✕]",
            "SQUARE" => "[▢]",
            "L1" => "[L1]",
            "R1" => "[R1]",
            "L2" => "[L2]",
            "R2" => "[R2]",
            _ => $"[{action}]"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
