using Maui.Essentials.AI.SampleApp.Pages;

namespace Maui.Essentials.AI.SampleApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// BUG: Infinite loop bug in .NET MAUI 10 Preview 7
		if (OperatingSystem.IsIOS() && !OperatingSystem.IsMacCatalyst())
			return new Window(new NavigationPage(activationState?.Context?.Services.GetRequiredService<MainPage>()));

		return new Window(new AppShell());
	}
}