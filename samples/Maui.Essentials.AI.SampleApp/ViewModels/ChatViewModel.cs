using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.Essentials.AI.SampleApp.Models;
using Maui.Essentials.AI.SampleApp.Services;

namespace Maui.Essentials.AI.SampleApp.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly IChatService _chatService;

    public ChatViewModel(IChatService chatService)
    {
        _chatService = chatService;

        Messages.CollectionChanged += (_, __) => (NewChatCommand as Command)?.ChangeCanExecute();
    }

    public ObservableCollection<ChatMessage> Messages { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    [NotifyCanExecuteChangedFor(nameof(NewChatCommand))]
    private string _input = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NewChatCommand))]
    private bool _hasStarted;

    public IReadOnlyList<string> SuggestedPrompts { get; } =
    [
        "Create an image",
        "Translate into Gen Z",
        "Write a first draft",
        "Brainstorm ideas",
        "Summarize this text"
    ];

    private bool CanSendAsync() => !string.IsNullOrWhiteSpace(Input);

    [RelayCommand(CanExecute = nameof(CanSendAsync))]
    private async Task SendAsync()
    {
        var text = Input?.Trim();
        if (string.IsNullOrEmpty(text))
            return;

        HasStarted = true;
        Input = string.Empty;
        var user = new ChatMessage { Role = ChatRole.User, Text = text };
        Messages.Add(user);

        try
        {
            var reply = await _chatService.SendAsync(Messages, text);
            Messages.Add(reply);
        }
        catch (Exception ex)
        {
            Messages.Add(new ChatMessage { Role = ChatRole.System, Text = ex.Message });
        }
    }

    [RelayCommand]
    private void UsePrompt(string prompt)
    {
        Input = prompt;
    }

    private bool CanNewChat()
        => HasStarted || Messages.Count > 0 || !string.IsNullOrWhiteSpace(Input);

    [RelayCommand(CanExecute = nameof(CanNewChat))]
    private void NewChat()
    {
        Input = string.Empty;
        Messages.Clear();
        HasStarted = false;
    }
}
