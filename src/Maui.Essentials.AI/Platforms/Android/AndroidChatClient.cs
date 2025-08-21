using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Google.AI.Edge.AICore;

namespace Maui.Essentials.AI;

/// <summary>
/// Android implementation of ChatClient using Google AI Edge (Gemini Nano)
/// </summary>
public sealed class AICoreChatClient : IChatClient
{
    private readonly GenerativeModel _model;
    private readonly ChatClientMetadata _metadata;

    private bool _disposed;

    public ChatClientMetadata Metadata => _metadata;

    /// <summary>
    /// Creates a new AICoreChatClient instance
    /// </summary>
    /// <param name="model">The GenerativeModel instance to use</param>
    /// <param name="modelId">ID of the model for responses</param>
    public AICoreChatClient(GenerativeModel model, string? modelId = null)
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));

        _metadata = new ChatClientMetadata(
            providerName: "Google AI Edge AICore",
            defaultModelId: modelId ?? "Gemini-Nano");
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

        try
        {
            await PrepareInferenceEngineAsync(cancellationToken);

            var contents = ConvertToContent(chatMessages).ToArray();

            var response = await _model.GenerateContentAsync(cancellationToken, contents);

            var textContent = ConvertToTextContent(response);

            return new ChatResponse()
            {
                Messages = { new ChatMessage(ChatRole.Assistant, [textContent]) },
                ModelId = options?.ModelId ?? _metadata.DefaultModelId,
                FinishReason = ChatFinishReason.Stop
            };
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Convert to a more appropriate exception type if needed
            throw new InvalidOperationException($"Error generating content: {ex.Message}", ex);
        }
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

        await PrepareInferenceEngineAsync(cancellationToken);

        var contents = ConvertToContent(chatMessages).ToArray();

        await foreach (var response in _model.GenerateContentStreamAsync(cancellationToken, contents))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var textContent = ConvertToTextContent(response);

            if (string.IsNullOrEmpty(textContent.Text))
                continue;

            yield return new ChatResponseUpdate
            {
                Contents = [textContent],
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
    /// Prepare the inference engine for faster responses
    /// </summary>
    public async Task PrepareInferenceEngineAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        await _model.PrepareInferenceEngineAsync(cancellationToken);
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

    private static IEnumerable<Content> ConvertToContent(IEnumerable<ChatMessage> chatMessages)
    {
        foreach (var message in chatMessages)
        {
            if (!string.IsNullOrEmpty(message.Text))
            {
                yield return ConvertToContent(message);
            }
        }
    }

    private static Content ConvertToContent(ChatMessage message) =>
        new Content.Builder()
            .AddText(message.Text)
            .SetRole(ConvertToContentRole(message.Role))
            .Build();

    private static ContentRole ConvertToContentRole(ChatRole role) =>
        role == ChatRole.User ? ContentRole.User : ContentRole.Model;

    private static TextContent ConvertToTextContent(GenerateContentResponse response) =>
        new TextContent(response.Text ?? string.Empty);

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _model?.Close();

        GC.SuppressFinalize(this);
    }
}
