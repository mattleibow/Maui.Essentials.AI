using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Foundation;

namespace Maui.Essentials.AI;

class Testing
{
    void Test()
    {
        if (OperatingSystem.IsIOSVersionAtLeast(26))
        {
            var av = SystemLanguageModel.Shared.IsAvailable;
            var opts = new Maui.Essentials.AI.GenerationOptions(
                sampling: Maui.Essentials.AI.GenerationOptionsSamplingMode.Greedy(),
                temperature: 0.5,
                maximumResponseTokens: 100);
            var sesh = new Maui.Essentials.AI.LanguageModelSession(SystemLanguageModel.Shared, "");
            sesh.Respond("Hello",
                (resp, err) =>
                {
                    Console.WriteLine(resp?.Content);
                });
            sesh.StreamResponse("Hello",
                (part) =>
                {
                    Console.WriteLine(part);
                },
                (resp, err) =>
                {
                    Console.WriteLine(resp?.Content);
                });
        }
    }
}

/// <summary>
/// Apple Intelligence implementation of ChatClient using native Apple Intelligence APIs
/// </summary>
public sealed class AppleIntelligenceChatClient : IChatClient
{
    private readonly LanguageModelSession _session;
    private readonly ChatClientMetadata _metadata;
    private bool _disposed;

    public ChatClientMetadata Metadata => _metadata;

    /// <summary>
    /// Creates a new AppleIntelligenceChatClient instance
    /// </summary>
    /// <param name="instructions">System instructions for the AI model</param>
    /// <param name="modelId">ID of the model for responses</param>
    public AppleIntelligenceChatClient(string? instructions = null, string? modelId = null)
    {
        // if (!IsAppleIntelligenceAvailable)
        // {
        //     throw new NotSupportedException("Apple Intelligence is not available on this device or operating system version.");
        // }

        // var systemInstructions = instructions ?? "You are a helpful AI assistant.";
        _session = new LanguageModelSession(SystemLanguageModel.Shared, instructions);
        
        _metadata = new ChatClientMetadata(
            providerName: "Apple Intelligence",
            defaultModelId: modelId ?? "Apple-Intelligence");
    }

    /// <summary>
    /// Gets a chat completion response from Apple Intelligence
    /// </summary>
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        try
        {
            var prompt = BuildPromptFromMessages(chatMessages);

            var options = new GenerationOptions
            {
                Sampling = options?.SamplingMode ?? GenerationOptionsSamplingMode.Greedy(),
                Temperature = options?.Temperature ?? 0.0,
                MaximumResponseTokens = options?.MaxTokens ?? 1000
            };

            var response = await _session.RespondAsync(prompt, options)

            return new ChatResponse
            {
                Messages = { new ChatMessage(ChatRole.Assistant, response) },
                ModelId = options?.ModelId ?? _metadata.DefaultModelId,
                FinishReason = ChatFinishReason.Stop
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            throw new InvalidOperationException($"Error generating content with Apple Intelligence: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets streaming chat completion updates from Apple Intelligence
    /// Note: Apple Intelligence doesn't support streaming, so this simulates streaming by yielding the complete response
    /// </summary>
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("Streaming is not implemented yet.");

        // ObjectDisposedException.ThrowIf(_disposed, this);

        // var prompt = BuildPromptFromMessages(chatMessages);
        // var response = await GetIntelligenceResponseAsync(prompt, cancellationToken);

        // // Since Apple Intelligence doesn't support true streaming, we yield the complete response
        // if (!string.IsNullOrEmpty(response))
        // {
        //     yield return new ChatResponseUpdate
        //     {
        //         Contents = [new TextContent(response)],
        //         ModelId = options?.ModelId ?? _metadata.DefaultModelId,
        //         Role = ChatRole.Assistant
        //     };
        // }

        // // Final update to indicate completion
        // yield return new ChatResponseUpdate
        // {
        //     FinishReason = ChatFinishReason.Stop,
        //     ModelId = options?.ModelId ?? _metadata.DefaultModelId,
        //     Role = ChatRole.Assistant
        // };
    }

    /// <summary>
    /// Gets a service instance from the client
    /// </summary>
    object? IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? _metadata :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _session?.Dispose();

        GC.SuppressFinalize(this);
    }
}
