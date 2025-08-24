using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.AI.Microsoft.PhiSilica;

/// <summary>
/// Windows implementation of ChatClient using Phi Silica on-device models
/// </summary>
[SupportedOSPlatform("windows10.0.17763.0")]
public sealed class PhiSilicaChatClient : IChatClient
{
    private readonly ChatClientMetadata _metadata;
    private bool _disposed;

    public ChatClientMetadata Metadata => _metadata;

    /// <summary>
    /// Creates a new PhiSilicaChatClient instance
    /// </summary>
    public PhiSilicaChatClient()
    {
        _metadata = new ChatClientMetadata(
            providerName: "Microsoft Phi Silica",
            defaultModelId: "Phi-3.5-mini-instruct");
    }

    /// <summary>
    /// Gets a chat completion response from Phi Silica
    /// </summary>
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Convert chat messages to format expected by Phi Silica
        var prompt = ConvertMessagesToPrompt(chatMessages, options);
        
        // Generate response using Phi Silica
        var responseText = await GenerateResponseAsync(prompt, options, cancellationToken);

        return new ChatResponse
        {
            Messages = { new ChatMessage(ChatRole.Assistant, responseText) },
            ModelId = options?.ModelId ?? _metadata.DefaultModelId,
            FinishReason = ChatFinishReason.Stop
        };
    }

    /// <summary>
    /// Gets streaming chat completion updates from Phi Silica
    /// </summary>
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        // Convert chat messages to format expected by Phi Silica
        var prompt = ConvertMessagesToPrompt(chatMessages, options);

        // Generate streaming response using Phi Silica
        await foreach (var chunk in GenerateStreamingResponseAsync(prompt, options, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!string.IsNullOrEmpty(chunk))
            {
                yield return new ChatResponseUpdate
                {
                    Contents = [new TextContent(chunk)],
                    ModelId = options?.ModelId ?? _metadata.DefaultModelId,
                    Role = ChatRole.Assistant
                };
            }
        }

        // Final update to indicate completion
        yield return new ChatResponseUpdate
        {
            FinishReason = ChatFinishReason.Stop,
            ModelId = options?.ModelId ?? _metadata.DefaultModelId,
            Role = ChatRole.Assistant
        };
    }

    /// <summary>
    /// Gets a service instance from the client
    /// </summary>
    object? IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        return serviceType.IsInstanceOfType(this) ? this : null;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            // Clean up any resources here
            _disposed = true;
        }
    }

    /// <summary>
    /// Converts chat messages to a prompt string for Phi Silica
    /// </summary>
    private static string ConvertMessagesToPrompt(IEnumerable<ChatMessage> chatMessages, ChatOptions? options)
    {
        var messages = chatMessages.ToList();
        var promptBuilder = new System.Text.StringBuilder();

        // Add system instructions if provided
        if (options?.Instructions is not null)
        {
            promptBuilder.AppendLine($"<|system|>\n{options.Instructions}<|end|>");
        }

        // Convert messages to Phi-3 chat format
        foreach (var message in messages)
        {
            var role = message.Role switch
            {
                ChatRole.System => "system",
                ChatRole.User => "user",
                ChatRole.Assistant => "assistant",
                _ => "user"
            };

            promptBuilder.AppendLine($"<|{role}|>\n{message.Text}<|end|>");
        }

        // Add assistant prompt to start generation
        promptBuilder.Append("<|assistant|>\n");

        return promptBuilder.ToString();
    }

    /// <summary>
    /// Generates a response using Phi Silica (placeholder implementation)
    /// </summary>
    private static async Task<string> GenerateResponseAsync(string prompt, ChatOptions? options, CancellationToken cancellationToken)
    {
        // TODO: Replace with actual Phi Silica inference
        // This is a placeholder implementation for now
        await Task.Delay(100, cancellationToken);
        
        return "I'm a Phi Silica model running on Windows. " +
               $"You said: {prompt.Split('\n').LastOrDefault(line => !string.IsNullOrWhiteSpace(line) && !line.Contains("<|"))?.Trim() ?? "something"}";
    }

    /// <summary>
    /// Generates a streaming response using Phi Silica (placeholder implementation)
    /// </summary>
    private static async IAsyncEnumerable<string> GenerateStreamingResponseAsync(
        string prompt, 
        ChatOptions? options, 
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // TODO: Replace with actual Phi Silica streaming inference
        // This is a placeholder implementation for now
        var response = await GenerateResponseAsync(prompt, options, cancellationToken);
        var words = response.Split(' ');

        foreach (var word in words)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return word + " ";
            await Task.Delay(50, cancellationToken); // Simulate streaming delay
        }
    }
}