using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Foundation;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.AI.Apple.FoundationModels;

/// <summary>
/// Apple Intelligence implementation of ChatClient using native Apple Intelligence APIs
/// </summary>
[SupportedOSPlatform("ios26.0")]
[SupportedOSPlatform("maccatalyst26.0")]
[SupportedOSPlatform("macos26.0")]
public sealed class AppleIntelligenceChatClient : IChatClient
{
    private bool _disposed;

    public ChatClientMetadata Metadata { get; }

    /// <summary>
    /// Creates a new AppleIntelligenceChatClient instance
    /// </summary>
    public AppleIntelligenceChatClient()
    {
        Metadata = new ChatClientMetadata(
            providerName: "apple",
            defaultModelId: "apple-intelligence");
    }

    /// <inheritdoc />
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        EnsureModelAvailability();

        var (history, lastMessage) = NormalizeChatMessages(chatMessages, options);
        using var session = CreateSession(history);

        var prompt = ConvertToPrompt(lastMessage);

        using var generationOptions = ConvertToGenerationOptions(options);

        var response = await session.RespondAsync(prompt, generationOptions);

        return new ChatResponse
        {
            Messages = { new ChatMessage(ChatRole.Assistant, response.Content) },
            ModelId = Metadata.DefaultModelId,
            CreatedAt = DateTimeOffset.Now,
            FinishReason = ChatFinishReason.Stop
        };
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        EnsureModelAvailability();

        var (history, lastMessage) = NormalizeChatMessages(chatMessages, options);
        using var session = CreateSession(history);

        var prompt = ConvertToPrompt(lastMessage);

        using var generationOptions = ConvertToGenerationOptions(options);

        var responseId = Guid.NewGuid().ToString("N");

        var lastResponse = "";
        await foreach (var response in session.StreamResponseAsync(prompt, generationOptions, cancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(response))
                continue;

            // Get the new content generated since the last response
            // The Apple models return the full response each time
            var updateResponse = response.Substring(lastResponse.Length);
            lastResponse = response;

            var aiContents = ConvertToAIContent(updateResponse);

            yield return new ChatResponseUpdate
            {
                Contents = aiContents,
                ModelId = options?.ModelId ?? Metadata.DefaultModelId,
                Role = ChatRole.Assistant,
                ResponseId = responseId
            };
        }

        // Final update to indicate completion
        yield return new ChatResponseUpdate
        {
            FinishReason = ChatFinishReason.Stop,
            ModelId = options?.ModelId ?? Metadata.DefaultModelId,
            Role = ChatRole.Assistant,
            ResponseId = responseId
        };
    }

    /// <inheritdoc />
    object? IChatClient.GetService(Type serviceType, object? serviceKey)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? Metadata :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    private GenerationOptions ConvertToGenerationOptions(ChatOptions? options)
    {
        // If the user provided a raw representation factory, use it
        if (options?.RawRepresentationFactory?.Invoke(this) is GenerationOptions rawOptions)
            return rawOptions;

        // Otherwise, build it up from the ChatOptions
        var sampling = options?.TopK is int topK
            ? GenerationOptionsSamplingMode.Random(topK: topK, seed: options.Seed)
            : null;

        var generationOptions = new GenerationOptions(
            sampling: sampling,
            temperature: options?.Temperature,
            maximumResponseTokens: options?.MaxOutputTokens);

        return generationOptions;
    }

    private static LanguageModelSession CreateSession(List<ChatMessage> history)
    {
        // Convert chat messages to transcript
        var transcript = ConvertToTranscript(history);

        // Start a session by rehydrating from a transcript
        var session = new LanguageModelSession(SystemLanguageModel.Shared, transcript);

        return session;
    }

    private static (List<ChatMessage> History, ChatMessage Prompt) NormalizeChatMessages(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null)
    {
        var messages = chatMessages.ToList();

        // Extract the last message as the prompt
        var lastMessage = messages.LastOrDefault() ?? throw new InvalidOperationException("No messages available.");
        messages.RemoveAt(messages.Count - 1);

        // Add system instructions if provided
        if (options?.Instructions is not null)
            messages.Insert(0, new ChatMessage(ChatRole.System, options.Instructions));

        return (messages, lastMessage);
    }

    private static Transcript ConvertToTranscript(IEnumerable<ChatMessage> chatMessages)
    {
        var entries = chatMessages.Select(ConvertToTranscriptEntry).ToArray();
        return new Transcript(entries);
    }

    private static TranscriptEntry ConvertToTranscriptEntry(ChatMessage chatMessage)
    {
        var segments = chatMessage.Contents.Select(ConvertTranscriptSegment).ToArray();

        if (chatMessage.Role == ChatRole.User)
            return new TranscriptEntry(new TranscriptPrompt(segments));

        if (chatMessage.Role == ChatRole.Assistant)
            return new TranscriptEntry(new TranscriptResponse(segments));

        if (chatMessage.Role == ChatRole.System)
            return new TranscriptEntry(new TranscriptInstructions(segments));

        throw new ArgumentOutOfRangeException(nameof(chatMessage.Role), $"Unknown chat role: {chatMessage.Role}");
    }

    private static TranscriptSegment ConvertTranscriptSegment(AIContent content)
    {
        if (content is TextContent textContent)
            return new TranscriptSegment(new TranscriptTextSegment(textContent.Text));

        // TODO: handle other content types
        throw new ArgumentOutOfRangeException(nameof(content), $"Unsupported content type: {content.GetType().Name}");
    }

    private static AIContent[] ConvertToAIContent(string updateResponse) =>
        [new TextContent(updateResponse ?? string.Empty)];

    private static string ConvertToPrompt(ChatMessage chatMessage) =>
        // TODO: handle other content types
        chatMessage.Text;

    private void EnsureModelAvailability()
    {
        switch (SystemLanguageModel.Shared.Availability)
        {
            case SystemLanguageModelAvailability.Available:
                return;
            case SystemLanguageModelAvailability.UnavailableAppleIntelligenceNotEnabled:
                throw new InvalidOperationException("Apple Intelligence is not enabled on this device. Please enable it in Settings > Apple Intelligence & Siri > Apple Intelligence.");
            case SystemLanguageModelAvailability.UnavailableDeviceNotEligible:
                throw new InvalidOperationException("This device is not eligible for Apple Intelligence.");
            case SystemLanguageModelAvailability.UnavailableModelNotReady:
                throw new InvalidOperationException("Apple Intelligence model is not ready.");
            case SystemLanguageModelAvailability.Unavailable:
            default:
                throw new InvalidOperationException("Apple Intelligence is unavailable.");
        }
    }
}
