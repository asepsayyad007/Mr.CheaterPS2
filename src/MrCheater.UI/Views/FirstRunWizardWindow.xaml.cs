using System.Windows;
using MrCheater.UI.ViewModels;

namespace MrCheater.UI.Views;

public partial class FirstRunWizardWindow : Window
{
    public FirstRunWizardWindow(FirstRunWizardViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.WizardCompleted += (s, e) => Close();
    }
}
