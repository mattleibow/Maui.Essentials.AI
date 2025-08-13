using Microsoft.Extensions.Logging;
using Maui.Essentials.AI.SampleApp.Services;
using Maui.Essentials.AI.SampleApp.ViewModels;

namespace Maui.Essentials.AI.SampleApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		// Services and VMs
		builder.Services.AddSingleton<IChatService, EchoChatService>();
		builder.Services.AddTransient<ChatViewModel>();
		builder.Services.AddTransient<MainPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
