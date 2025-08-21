using Maui.Essentials.AI.SampleApp.Models;
using Microsoft.Extensions.AI;

namespace Maui.Essentials.AI.SampleApp.Services;

/// <summary>
/// Apple Intelligence implementation of IChatService
/// </summary>
public class AppleIntelligenceChatService : IChatService, IDisposable
{
#if __IOS__ || __MACOS__ || __MACCATALYST__
    private readonly AppleIntelligenceChatClient _chatClient;
    private bool _disposed;

    public AppleIntelligenceChatService()
    {
        if (!AppleIntelligenceChatClient.IsAppleIntelligenceAvailable)
        {
            throw new NotSupportedException("Apple Intelligence is not available on this device or operating system version.");
        }

        _chatClient = new AppleIntelligenceChatClient(
            instructions: "You are a helpful AI assistant in a mobile app. Keep responses concise and friendly.",
            modelId: "Apple-Intelligence");
    }

    public async Task<ChatMessage> SendAsync(IEnumerable<ChatMessage> history, string input, CancellationToken ct = default)
    {
        try
        {
            // Convert our app's chat messages to Microsoft.Extensions.AI format
            var aiMessages = ConvertToAIMessages(history).Append(
                new Microsoft.Extensions.AI.ChatMessage(Microsoft.Extensions.AI.ChatRole.User, input)
            );

            // Get response from Apple Intelligence
            var response = await _chatClient.GetResponseAsync(aiMessages, cancellationToken: ct);
            
            // Convert back to our app's format
            var responseText = response.Messages.FirstOrDefault()?.Text ?? "I'm sorry, I couldn't generate a response.";
            
            return new ChatMessage
            {
                Role = ChatRole.Assistant,
                Text = responseText
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return new ChatMessage
            {
                Role = ChatRole.Assistant,
                Text = $"Sorry, I encountered an error: {ex.Message}"
            };
        }
    }

    private static IEnumerable<Microsoft.Extensions.AI.ChatMessage> ConvertToAIMessages(IEnumerable<ChatMessage> messages)
    {
        foreach (var message in messages)
        {
            var role = message.Role switch
            {
                ChatRole.User => Microsoft.Extensions.AI.ChatRole.User,
                ChatRole.Assistant => Microsoft.Extensions.AI.ChatRole.Assistant,
                _ => Microsoft.Extensions.AI.ChatRole.System
            };

            yield return new Microsoft.Extensions.AI.ChatMessage(role, message.Text);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _chatClient?.Dispose();
            _disposed = true;
        }
    }
#else
    public Task<ChatMessage> SendAsync(IEnumerable<ChatMessage> history, string input, CancellationToken ct = default)
    {
        throw new NotSupportedException("Apple Intelligence is only available on Apple platforms (iOS, macOS, Mac Catalyst).");
    }

    public void Dispose()
    {
        // No-op on non-Apple platforms
    }
#endif
}