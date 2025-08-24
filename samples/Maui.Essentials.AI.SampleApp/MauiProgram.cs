using Microsoft.Extensions.Logging;
using Maui.Essentials.AI.SampleApp.Services;
using Maui.Essentials.AI.SampleApp.ViewModels;
using Maui.Essentials.AI.SampleApp.Pages;
using Microsoft.Extensions.AI;

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
		builder.Services.AddSingleton<ISettingsService, SettingsService>();
		#if IOS || MACCATALYST
		builder.Services.AddSingleton<IChatClient,  Microsoft.Extensions.AI.Apple.FoundationModels.AppleIntelligenceChatClient>();
		#else
		builder.Services.AddSingleton<IChatClient, EchoChatClient>();
		#endif
		builder.Services.AddSingleton<ChatViewModel>();
		builder.Services.AddSingleton<SettingsViewModel>();
		builder.Services.AddSingleton<MainPage>();
		builder.Services.AddSingleton<SettingsPage>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
