using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace Maui.Essentials.AI;

/// <summary>
/// Simple echo implementation of ChatClient for testing and fallback scenarios
/// </summary>
public sealed class EchoChatClient : ChatClientBase
{
    private readonly string _modelName;
    private readonly TimeSpan _responseDelay;

    /// <summary>
    /// Creates a new EchoChatClient instance
    /// </summary>
    /// <param name="modelName">Name to use for the model in responses</param>
    /// <param name="responseDelay">Delay to simulate AI processing time</param>
    public EchoChatClient(string modelName = "Echo", TimeSpan? responseDelay = null)
    {
        _modelName = modelName;
        _responseDelay = responseDelay ?? TimeSpan.FromMilliseconds(500);
    }

    /// <summary>
    /// Gets a chat completion response that echoes the user's input
    /// </summary>
    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(_responseDelay, cancellationToken);

        var messages = chatMessages.ToList();
        var lastUserMessage = messages.LastOrDefault(m => m.Role == ChatRole.User);
        
        var responseText = lastUserMessage?.Text?.Length > 0 
            ? $"You said: {lastUserMessage.Text}"
            : "I didn't understand your message.";

        var chatMessage = new ChatMessage(ChatRole.Assistant, responseText);
        return new ChatResponse(chatMessage)
        {
            ModelId = _modelName,
            FinishReason = ChatFinishReason.Stop
        };
    }

    /// <summary>
    /// Gets streaming chat completion updates that echo the user's input
    /// </summary>
    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // Simulate processing delay
        await Task.Delay(_responseDelay, cancellationToken);

        var messages = chatMessages.ToList();
        var lastUserMessage = messages.LastOrDefault(m => m.Role == ChatRole.User);
        
        var responseText = lastUserMessage?.Text?.Length > 0 
            ? $"You said: {lastUserMessage.Text}"
            : "I didn't understand your message.";

        // Simulate streaming by yielding text in chunks
        var words = responseText.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var word = i == 0 ? words[i] : $" {words[i]}";
            
            yield return new ChatResponseUpdate
            {
                Contents = [new TextContent(word)],
                ModelId = _modelName,
                Role = ChatRole.Assistant
            };

            // Small delay between words to simulate streaming
            if (i < words.Length - 1)
            {
                await Task.Delay(50, cancellationToken);
            }
        }

        // Final update to indicate completion
        yield return new ChatResponseUpdate
        {
            FinishReason = ChatFinishReason.Stop,
            ModelId = _modelName,
            Role = ChatRole.Assistant
        };
    }
}