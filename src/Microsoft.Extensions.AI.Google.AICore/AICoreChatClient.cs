using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Google.AI.Edge.AICore;

namespace Microsoft.Extensions.AI.Google.AICore;

/// <summary>
/// Android implementation of ChatClient using Google AI Edge (Gemini Nano)
/// </summary>
public sealed class AICoreChatClient : IChatClient
{
    private readonly ChatClientMetadata _metadata;

    private bool _disposed;

    public ChatClientMetadata Metadata => _metadata;

    /// <summary>
    /// Creates a new AICoreChatClient instance
    /// </summary>
    public AICoreChatClient()
    {
        _metadata = new ChatClientMetadata(
            providerName: "Google AI Edge AICore",
            defaultModelId: "Gemini-Nano");
    }

    /// <summary>
    /// Gets a chat completion response from Gemini Nano
    /// </summary>
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var model = await CreateModel(options, cancellationToken);

        var contents = ConvertToContent(chatMessages);

        var response = await model.GenerateContentAsync(cancellationToken, contents);

        var aiContents = ConvertToAIContent(response);

        return new ChatResponse()
        {
            Messages = { new ChatMessage(ChatRole.Assistant, aiContents) },
            ModelId = options?.ModelId ?? _metadata.DefaultModelId,
            FinishReason = ChatFinishReason.Stop
        };
    }

    /// <summary>
    /// Gets streaming chat completion updates from Gemini Nano
    /// </summary>
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var model = await CreateModel(options, cancellationToken);

        var contents = ConvertToContent(chatMessages);

        await foreach (var response in model.GenerateContentStreamAsync(cancellationToken, contents))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var aiContents = ConvertToAIContent(response);

            yield return new ChatResponseUpdate
            {
                Contents = aiContents,
                ModelId = options?.ModelId ?? _metadata.DefaultModelId,
                Role = ChatRole.Assistant
            };
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
        ArgumentNullException.ThrowIfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? _metadata :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    private static async Task<GenerativeModel> CreateModel(ChatOptions? options, CancellationToken cancellationToken)
    {
        var config = new GenerationConfig.Builder()
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
        }.Build();

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
