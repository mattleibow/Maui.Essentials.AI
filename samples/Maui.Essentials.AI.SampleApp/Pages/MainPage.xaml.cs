using Maui.Essentials.AI.SampleApp.ViewModels;

namespace Maui.Essentials.AI.SampleApp.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(ChatViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

		vm.WordSelectionStarted += (s, e) =>
		{
			var tcs = new TaskCompletionSource<string?>();

			Dispatcher.Dispatch(async () =>
			{
				var result = await DisplayActionSheetAsync("Word Selector", "Cancel", null, e.Words);
				if (result != null)
					tcs.SetResult(result);
				else
					tcs.SetCanceled();
			});

			e.Task = tcs.Task;
		};
	}
}
