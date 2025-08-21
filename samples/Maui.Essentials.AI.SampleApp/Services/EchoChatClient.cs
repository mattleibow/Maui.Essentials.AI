using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

namespace Maui.Essentials.AI.SampleApp.Services;

/// <summary>
/// Simple echo implementation of ChatClient for testing and fallback scenarios
/// </summary>
public sealed class EchoChatClient : IChatClient
{
    private readonly TimeSpan _responseDelay;
    private readonly ChatClientMetadata _metadata;

    public ChatClientMetadata Metadata => _metadata;

    /// <summary>
    /// Creates a new EchoChatClient instance
    /// </summary>
    /// <param name="modelId">ID to use for the model in responses</param>
    public EchoChatClient(string modelId = "Echo")
    {
        _responseDelay = TimeSpan.FromMilliseconds(500);
        _metadata = new ChatClientMetadata(modelId);
    }

    /// <summary>
    /// Gets a chat completion response that echoes the user's input
    /// </summary>
    public async Task<ChatResponse> GetResponseAsync(
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

        return new ChatResponse()
        {
            Messages = { new ChatMessage(ChatRole.Assistant, responseText) },
            ModelId = _metadata.DefaultModelId,
            FinishReason = ChatFinishReason.Stop
        };
    }

    /// <summary>
    /// Gets streaming chat completion updates that echo the user's input
    /// </summary>
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
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
                ModelId = _metadata.DefaultModelId,
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
            ModelId = _metadata.DefaultModelId,
            Role = ChatRole.Assistant
        };
    }

    object? IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? _metadata :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    void IDisposable.Dispose()
    {
        // Nothing to dispose. Implementation required for the IChatClient interface.
    }
}
