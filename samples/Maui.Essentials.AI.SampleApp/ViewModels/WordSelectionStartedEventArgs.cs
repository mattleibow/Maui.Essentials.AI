namespace Maui.Essentials.AI.SampleApp.ViewModels;

public class WordSelectionStartedEventArgs(string[] words) : EventArgs
{
    public string[] Words { get; } = words;

    public Task<string?>? Task { get; set; }
}
