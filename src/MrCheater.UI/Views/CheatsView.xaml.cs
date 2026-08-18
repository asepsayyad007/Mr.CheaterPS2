using System.Windows;
using System.Windows.Controls;
using MrCheater.UI.ViewModels;

namespace MrCheater.UI.Views;

public partial class CheatsView : UserControl
{
    public CheatsView()
    {
        InitializeComponent();
    }

    private void OnCategoryChipClicked(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.DataContext is string category && DataContext is CheatsViewModel vm)
        {
            vm.SelectedCategory = category;
        }
    }
}
