namespace Maui.Essentials.AI.SampleApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{

#if IOS || MACCATALYST
var av = Essentials.AI.SystemLanguageModel.Shared.IsAvailable; // Ensure the model is available
#endif
		return new Window(new AppShell());
	}
}