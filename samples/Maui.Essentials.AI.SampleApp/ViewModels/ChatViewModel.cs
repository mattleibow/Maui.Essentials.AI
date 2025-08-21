using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
using Maui.Essentials.AI.SampleApp.Services;

namespace Maui.Essentials.AI.SampleApp.ViewModels;

public partial class ChatViewModel : ObservableObject
{
    private readonly IChatClient _chatClient;
    private readonly ISettingsService _settingsService;

    public ChatViewModel(IChatClient chatClient, ISettingsService settingsService)
    {
        _chatClient = chatClient;
        _settingsService = settingsService;

        Messages.CollectionChanged += (_, __) => (NewChatCommand as Command)?.ChangeCanExecute();

        // Initialize settings
        UseStreaming = _settingsService.UseStreaming;
        SystemMessage = _settingsService.SystemMessage;

        // Subscribe to settings changes
        _settingsService.SettingsChanged += OnSettingsChanged;
    }

    public ObservableCollection<ChatMessageViewModel> Messages { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    [NotifyCanExecuteChangedFor(nameof(NewChatCommand))]
    private string _input = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NewChatCommand))]
    private bool _hasStarted;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    [NotifyCanExecuteChangedFor(nameof(NewChatCommand))]
    private bool _isGenerating;

    [ObservableProperty]
    private bool _useStreaming = true;

    [ObservableProperty]
    private string _systemMessage = "You are a helpful AI assistant.";

    public IReadOnlyList<string> SuggestedPrompts { get; } =
    [
        "Create an image",
        "Translate into Gen Z",
        "Write a first draft",
        "Brainstorm ideas",
        "Summarize this text"
    ];

    private void OnSettingsChanged(object? sender, EventArgs e)
    {
        UseStreaming = _settingsService.UseStreaming;
        SystemMessage = _settingsService.SystemMessage;
    }

    [RelayCommand]
    private void UsePrompt(string prompt)
    {
        Input = prompt;
        SendCommand.Execute(null);
    }

    private bool CanNewChat() =>
        !IsGenerating && (HasStarted || Messages.Count > 0 || !string.IsNullOrWhiteSpace(Input));

    [RelayCommand(CanExecute = nameof(CanNewChat))]
    private void NewChat()
    {
        if (IsGenerating)
            return;
            
        Input = string.Empty;
        Messages.Clear();
        HasStarted = false;
    }

    private bool CanSendAsync() =>
        !string.IsNullOrWhiteSpace(Input) && !IsGenerating;

    [RelayCommand(CanExecute = nameof(CanSendAsync))]
    private async Task SendAsync()
    {
        var text = Input?.Trim();
        if (string.IsNullOrEmpty(text) || IsGenerating)
            return;

        HasStarted = true;
        IsGenerating = true;
        Input = string.Empty;

        // Add user message
        var userMessage = new ChatMessageViewModel(ChatMessageRole.User, text);
        Messages.Add(userMessage);

        // Send the request to AI
        try
        {
            if (UseStreaming)
            {
                await SendStreamingAsync();
            }
            else
            {
                await SendNonStreamingAsync();
            }
        }
        catch (Exception ex)
        {
            var errorMessage = new ChatMessageViewModel(ChatMessageRole.System, ex.Message);
            Messages.Add(errorMessage);
        }
        finally
        {
            IsGenerating = false;
        }
    }

    private async Task SendStreamingAsync()
    {
        // Create assistant message placeholder for streaming updates
        var assistantMessage = new ChatMessageViewModel(ChatMessageRole.Assistant, "");
        Messages.Add(assistantMessage);

        var responseBuilder = new StringBuilder();

        try
        {
            // Convert UI messages to AI client messages
            var aiMessages = ConvertToAIMessages(Messages.Take(Messages.Count - 1));
            
            // Send the request
            await foreach (var update in _chatClient.GetStreamingResponseAsync(aiMessages))
            {
                if (update.Contents is not { } contents)
                    continue;

                foreach (var content in contents)
                {
                    // Skip empty messages
                    if (content is not TextContent textContent || string.IsNullOrEmpty(textContent.Text))
                        continue;

                    responseBuilder.Append(textContent.Text);

                    // Update the last message in real-time
                    assistantMessage.Text = responseBuilder.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            // If streaming fails, replace the assistant message with error
            assistantMessage.Role = ChatMessageRole.System;
            assistantMessage.Text = $"Streaming error: {ex.Message}";

            throw;
        }
    }

    private async Task SendNonStreamingAsync()
    {
        // Convert UI messages to AI client messages
        var aiMessages = ConvertToAIMessages(Messages);
        
        // Send the request
        var response = await _chatClient.GetResponseAsync(aiMessages);
        
        if (response.Messages?.FirstOrDefault() is { } responseMessage)
        {
            var text = GetTextFromAIMessage(responseMessage);
            Messages.Add(new ChatMessageViewModel(ChatMessageRole.Assistant, text));
        }
        else
        {
            Messages.Add(new ChatMessageViewModel(ChatMessageRole.System, "No response received"));
        }
    }

    private List<ChatMessage> ConvertToAIMessages(IEnumerable<ChatMessageViewModel> uiMessages)
    {
        var messages = new List<ChatMessage>();
        
        // Add system message first if it's not empty
        if (!string.IsNullOrWhiteSpace(SystemMessage))
            messages.Add(new ChatMessage(ChatRole.System, SystemMessage));
        
        // Add conversation history (exclude any system messages from UI as they should come from settings)
        var conversationMessages = uiMessages
            .Where(m => m.Role != ChatMessageRole.System)
            .Select(ConvertToAIMessage);
            
        messages.AddRange(conversationMessages);
        
        return messages;
    }

    private ChatMessage ConvertToAIMessage(ChatMessageViewModel uiMessage)
    {
        var aiRole = uiMessage.Role switch
        {
            ChatMessageRole.User => ChatRole.User,
            ChatMessageRole.Assistant => ChatRole.Assistant,
            ChatMessageRole.System => ChatRole.System,
            _ => ChatRole.User
        };

        return new ChatMessage(aiRole, uiMessage.Text);
    }

    private string GetTextFromAIMessage(ChatMessage aiMessage)
    {
        if (aiMessage.Contents?.FirstOrDefault() is TextContent textContent)
            return textContent.Text ?? "";
        return aiMessage.Text ?? "";
    }
}
