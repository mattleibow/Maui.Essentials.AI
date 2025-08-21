using Maui.Essentials.AI.SampleApp.ViewModels;

namespace Maui.Essentials.AI.SampleApp.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(ChatViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}
