using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.Essentials.AI.SampleApp.Services;

namespace Maui.Essentials.AI.SampleApp.ViewModels;

public partial class SettingsViewModel(ISettingsService settingsService) : ObservableObject
{
    [ObservableProperty]
    private bool _useStreaming = settingsService.UseStreaming;

    [ObservableProperty]
    private string _systemMessage = settingsService.SystemMessage;

    partial void OnUseStreamingChanged(bool value) =>
        settingsService.UseStreaming = value;

    partial void OnSystemMessageChanged(string value) =>
        settingsService.SystemMessage = value;

    [RelayCommand]
    private void ResetSystemMessage() =>
        SystemMessage = "You are a helpful AI assistant.";
}
