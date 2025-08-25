using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Google.AI.Edge.AICore;

namespace Microsoft.Extensions.AI.Google.AICore;

/// <summary>
/// Android implementation of ChatClient using Google AI Edge (Gemini Nano)
/// </summary>
public sealed class AICoreChatClient : IChatClient
{
    private bool _disposed;

    public ChatClientMetadata Metadata { get; }

    /// <summary>
    /// Creates a new AICoreChatClient instance
    /// </summary>
    public AICoreChatClient()
    {
        Metadata = new ChatClientMetadata(
            providerName: "google",
            defaultModelId: "gemini-nano");
    }

    /// <inheritdoc />
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        using var model = await CreateModelAsync(options, cancellationToken);

        var contents = ConvertToContent(chatMessages);

        var response = await model.GenerateContentAsync(cancellationToken, contents);

        var aiContents = ConvertToAIContent(response);

        return new ChatResponse()
        {
            Messages = { new ChatMessage(ChatRole.Assistant, aiContents) },
            ModelId = options?.ModelId ?? Metadata.DefaultModelId,
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

        using var model = await CreateModelAsync(options, cancellationToken);

        var contents = ConvertToContent(chatMessages);

        var responseId = Guid.NewGuid().ToString("N");

        await foreach (var response in model.GenerateContentStreamAsync(cancellationToken, contents))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var aiContents = ConvertToAIContent(response);

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

    private GenerationConfig ConvertToGenerationConfig(ChatOptions? options)
    {
        // If the user provided a raw representation factory, use it
        if (options?.RawRepresentationFactory?.Invoke(this) is GenerationConfig rawOptions)
            return rawOptions;

        // Otherwise, build it up from the ChatOptions
        var builder = new GenerationConfig.Builder()
        {
            Context = global::Android.App.Application.Context,
            MaxOutputTokens = options?.MaxOutputTokens is int maxTokens
                ? Java.Lang.Integer.ValueOf(maxTokens)
                : null,
            Temperature = options?.Temperature is float temperature
                ? Java.Lang.Float.ValueOf(temperature)
                : null,
            TopK = options?.TopK is int topK
                ? Java.Lang.Integer.ValueOf(topK)
                : null
        };

        var config = builder.Build();

        return config;
    }

    private async Task<GenerativeModel> CreateModelAsync(ChatOptions? options, CancellationToken cancellationToken)
    {
        var config = ConvertToGenerationConfig(options);

        // TODO: handle the download callback

        var model = new GenerativeModel(config);

        await model.PrepareInferenceEngineAsync(cancellationToken);

        return model;
    }

    private static Content[] ConvertToContent(IEnumerable<ChatMessage> chatMessages)
    {
        var contents = chatMessages.Select(ConvertToContent).ToArray();

        return contents;
    }

    private static Content ConvertToContent(ChatMessage chatMessage)
    {
        var parts = chatMessage.Contents.Select(ConvertToPart).ToArray();

        var content = new Content.Builder()
            .SetRole(ConvertToContentRole(chatMessage.Role))
            .SetParts(parts)
            .Build();

        return content;
    }

    private static IPart ConvertToPart(AIContent content)
    {
        if (content is TextContent textContent)
            return new TextPart(textContent.Text);

        // TODO: handle other content types
        throw new ArgumentOutOfRangeException(nameof(content), $"Unsupported content type: {content.GetType().Name}");
    }

    private static ContentRole ConvertToContentRole(ChatRole role) =>
        role == ChatRole.User ? ContentRole.User : ContentRole.Model;

    private static AIContent[] ConvertToAIContent(GenerateContentResponse response) =>
        // TODO: handle message parts correctly
        [new TextContent(response.Text ?? string.Empty)];

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
