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

    [ObservableProperty]
    private bool _simulateError = settingsService.SimulateError;

    [ObservableProperty]
    private bool _simulateStreamError = settingsService.SimulateStreamError;

    partial void OnUseStreamingChanged(bool value) =>
        settingsService.UseStreaming = value;

    partial void OnSystemMessageChanged(string value) =>
        settingsService.SystemMessage = value;

    partial void OnSimulateErrorChanged(bool value) =>
        settingsService.SimulateError = value;

    partial void OnSimulateStreamErrorChanged(bool value) =>
        settingsService.SimulateStreamError = value;

    [RelayCommand]
    private void ResetSystemMessage() =>
        SystemMessage = "You are a helpful AI assistant.";
}
