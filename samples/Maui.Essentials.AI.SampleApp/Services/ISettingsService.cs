namespace Maui.Essentials.AI.SampleApp.Services;

public interface ISettingsService
{
    bool UseStreaming { get; set; }
    string SystemMessage { get; set; }
    bool SimulateError { get; set; }
    bool SimulateStreamError { get; set; }
    
    event EventHandler? SettingsChanged;
}
