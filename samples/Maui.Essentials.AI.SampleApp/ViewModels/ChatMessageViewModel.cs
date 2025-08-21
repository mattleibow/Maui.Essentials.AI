using CommunityToolkit.Mvvm.ComponentModel;

namespace Maui.Essentials.AI.SampleApp.ViewModels;

public partial class ChatMessageViewModel(ChatMessageRole role, string text) : ObservableObject
{
    public ChatMessageViewModel()
        : this(ChatMessageRole.User, "")
    {
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUser), nameof(IsAssistant), nameof(IsSystem))]
    private ChatMessageRole _role = role;

    [ObservableProperty]
    private string _text = text ?? "";

    [ObservableProperty]
    private DateTimeOffset _timestamp = DateTimeOffset.Now;

    public bool IsUser => Role == ChatMessageRole.User;

    public bool IsAssistant => Role == ChatMessageRole.Assistant;

    public bool IsSystem => Role == ChatMessageRole.System;
}
