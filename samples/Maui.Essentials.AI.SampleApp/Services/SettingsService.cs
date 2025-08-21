namespace Maui.Essentials.AI.SampleApp.Services;

public class SettingsService : ISettingsService
{
    private bool _useStreaming = true;
    private string _systemMessage = "You are a helpful AI assistant.";

    public bool UseStreaming
    {
        get => _useStreaming;
        set
        {
            if (_useStreaming != value)
            {
                _useStreaming = value;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public string SystemMessage
    {
        get => _systemMessage;
        set
        {
            if (_systemMessage != value)
            {
                _systemMessage = value ?? "You are a helpful AI assistant.";
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? SettingsChanged;
}
