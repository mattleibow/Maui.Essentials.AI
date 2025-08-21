using Maui.Essentials.AI.SampleApp.ViewModels;

namespace Maui.Essentials.AI.SampleApp.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
