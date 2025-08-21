# IChatClient Implementation - Usage Examples

This document shows how to use the newly implemented IChatClient in Maui.Essentials.AI.

## Basic Setup

### 1. Register AI Services in MauiProgram.cs

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseAI(); // 🎉 Register AI services

        return builder.Build();
    }
}
```

### 2. Configure AI Options (Optional)

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseAI(options =>
            {
                // Configure echo model name for non-Android platforms
                options.EchoModelName = "MyCustomEcho";
                
                // Disable Android Gemini Nano (will use Echo instead)
                options.UseAndroidGeminiNano = false;
                
#if ANDROID
                // Configure Android Gemini Nano settings
                options.AndroidGeminiNanoConfiguration = builder =>
                {
                    builder.Temperature = new Java.Lang.Float(0.7f);
                    builder.MaxOutputTokens = new Java.Lang.Integer(256);
                };
#endif
            });

        return builder.Build();
    }
}
```

## Usage in ViewModels

### Basic Chat Implementation

```csharp
using Microsoft.Extensions.AI;

public partial class ChatViewModel : ObservableObject
{
    private readonly IChatClient _chatClient;

    public ChatViewModel(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    [RelayCommand]
    async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(UserInput))
            return;

        // Add user message to chat
        Messages.Add(new ChatMessage(ChatRole.User, UserInput));
        
        try
        {
            // Get AI response  
            var response = await _chatClient.GetResponseAsync(Messages);
            
            // Note: Access response content through the response object
            // The exact properties depend on the Microsoft.Extensions.AI version
            Messages.Add(new ChatMessage(ChatRole.Assistant, "AI Response"));
        }
        catch (Exception ex)
        {
            // Handle errors
            Messages.Add(new ChatMessage(ChatRole.System, $"Error: {ex.Message}"));
        }
        
        UserInput = string.Empty;
    }
}
```

### Streaming Chat Implementation

```csharp
[RelayCommand]
async Task SendStreamingMessageAsync()
{
    if (string.IsNullOrWhiteSpace(UserInput))
        return;

    Messages.Add(new ChatMessage(ChatRole.User, UserInput));
    
    // Add a placeholder for the AI response
    var aiMessage = new ChatMessage(ChatRole.Assistant, "");
    Messages.Add(aiMessage);
    
    try
    {
        // Get streaming response
        await foreach (var update in _chatClient.GetStreamingResponseAsync(Messages))
        {
            if (update.Contents?.Any() == true)
            {
                // Update the AI message with streaming content
                var textContent = update.Contents.OfType<TextContent>().FirstOrDefault();
                if (textContent != null)
                {
                    aiMessage.Text += textContent.Text;
                    // Notify UI of change
                    OnPropertyChanged(nameof(Messages));
                }
            }
            
            if (update.FinishReason == ChatFinishReason.Stop)
            {
                break; // Streaming complete
            }
        }
    }
    catch (Exception ex)
    {
        aiMessage.Text = $"Error: {ex.Message}";
    }
    
    UserInput = string.Empty;
}
```

## Platform-Specific Features

### Android - Gemini Nano Features

```csharp
// Inject AndroidChatClient specifically (on Android only)
#if ANDROID
[RelayCommand]
async Task PrepareModelAsync()
{
    if (_chatClient is AndroidChatClient androidClient)
    {
        try
        {
            await androidClient.PrepareInferenceEngineAsync();
            StatusMessage = "Model prepared for faster responses!";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Failed to prepare model: {ex.Message}";
        }
    }
}
#endif
```

## Advanced Configuration

### Custom Service Registration

```csharp
// In MauiProgram.cs - Register services manually
builder.Services.AddAI(options =>
{
    options.UseAndroidGeminiNano = true;
});

// Register additional services
builder.Services.AddSingleton<ChatViewModel>();
builder.Services.AddTransient<ChatPage>();
```

### Service Locator Pattern

```csharp
public class MyService
{
    private readonly IChatClient _chatClient;
    
    public MyService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }
    
    public void SomeMethod()
    {
        // Use service locator to get additional services
        var customService = _chatClient.GetService<ICustomService>();
        
        // Check if running on Android with Gemini Nano
        if (_chatClient is AndroidChatClient)
        {
            // Android-specific logic
        }
    }
}
```

## Model Information

### Current Platform Support

- **Android**: Uses Google AI Edge (Gemini Nano) when available, falls back to Echo
- **iOS/macOS**: Uses Echo client (placeholder for future Core ML integration)
- **Windows**: Uses Echo client (placeholder for future ONNX/DirectML integration)
- **All Platforms**: Echo client always available as fallback

### Echo Client Behavior

The Echo client is designed for testing and fallback scenarios:

- Responds with "You said: [your message]"
- Simulates streaming by sending words incrementally
- Configurable response delay (default 500ms)
- Always returns `ChatFinishReason.Stop`

## Error Handling

```csharp
try
{
    var response = await _chatClient.GetResponseAsync(messages);
    // Handle successful response
}
catch (OperationCanceledException)
{
    // User cancelled the operation
}
catch (InvalidOperationException ex)
{
    // Model not available or configuration error
    _logger.LogError(ex, "AI model error");
}
catch (Exception ex)
{
    // General error
    _logger.LogError(ex, "Unexpected error in AI chat");
}
```

## Best Practices

1. **Always handle exceptions** - AI models can fail or be unavailable
2. **Use cancellation tokens** - Allow users to cancel long-running operations
3. **Provide feedback** - Show loading states during AI processing
4. **Fallback gracefully** - The Echo client ensures your app always works
5. **Test on actual devices** - Gemini Nano requires specific Android versions/hardware

## Testing

The implementation includes comprehensive tests. To run them:

```bash
dotnet test tests/Maui.Essentials.AI.Tests/
```

Tests cover:
- Basic client creation
- Response generation (both sync and streaming)
- Dependency injection registration
- Error scenarios
- Configuration options