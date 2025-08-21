namespace Maui.Essentials.AI.SampleApp.Services;

public class SettingsService : ISettingsService
{
    private bool _useStreaming = true;
    private string _systemMessage = "You are a helpful AI assistant.";
    private bool _simulateError = false;
    private bool _simulateStreamError = false;

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

    public bool SimulateError
    {
        get => _simulateError;
        set
        {
            if (_simulateError != value)
            {
                _simulateError = value;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public bool SimulateStreamError
    {
        get => _simulateStreamError;
        set
        {
            if (_simulateStreamError != value)
            {
                _simulateStreamError = value;
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? SettingsChanged;
}
