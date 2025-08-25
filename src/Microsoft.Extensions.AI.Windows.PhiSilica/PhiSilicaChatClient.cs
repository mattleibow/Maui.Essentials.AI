using Microsoft.Windows.AI;
using Microsoft.Windows.AI.ContentSafety;
using Microsoft.Windows.AI.Text;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.Extensions.AI.Windows.PhiSilica;

public class PhiSilicaChatClient : IChatClient
{
    private bool _disposed;

    public ChatClientMetadata Metadata { get; }

    public PhiSilicaChatClient()
    {
        Metadata = new ChatClientMetadata(
            providerName: "windows",
            defaultModelId: "phi-silica");
    }

    public Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default) =>
        GetStreamingResponseAsync(chatMessages, options, cancellationToken).ToChatResponseAsync(cancellationToken: cancellationToken);

    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        using var model = await CreateModelAsync(cancellationToken);

        var (systemPrompt, history) = NormalizeChatMessages(chatMessages, options);

        using var context = string.IsNullOrEmpty(systemPrompt)
            ? model.CreateContext()
            : model.CreateContext(systemPrompt, new ContentFilterOptions());

        var prompt = ConvertToPrompt(history);

        var modelOptions = ConvertToLanguageModelOptions(options);

        var responseId = Guid.NewGuid().ToString("N");

        await foreach (var response in model.GenerateResponseAsync(context, prompt, modelOptions, cancellationToken))
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

    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return
            serviceKey is not null ? null :
            serviceType == typeof(ChatClientMetadata) ? Metadata :
            serviceType.IsInstanceOfType(this) ? this :
            null;
    }

    private static AIContent[] ConvertToAIContent(string response) =>
        [new TextContent(response ?? string.Empty)];

    private static (string SystemPrompt, List<ChatMessage> History) NormalizeChatMessages(IEnumerable<ChatMessage> chatMessages, ChatOptions? options = null)
    {
        var messages = chatMessages.ToList();

        // Use system instructions as the system prompt if provided
        if (options?.Instructions is { } system)
            return (system, messages);

        // Extract the first system message as the system prompt
        if (messages.Count > 0 && messages[0].Role == ChatRole.System)
        {
            var systemPrompt = messages[0].Text;
            messages.RemoveAt(0);

            return (systemPrompt, messages);
        }

        return ("", messages);
    }

    private static string ConvertToPrompt(IEnumerable<ChatMessage> history)
    {
        // TODO: this looks suspiciously like it should be adding roles or separators

        var promptBuilder = new StringBuilder();

        foreach (var message in history)
        {
            foreach (var content in message.Contents)
            {
                if (content is TextContent textContent)
                    promptBuilder.AppendLine(textContent.Text);

                // TODO: handle other content types
                throw new ArgumentOutOfRangeException(nameof(content), $"Unsupported content type: {content.GetType().Name}");
            }
        }

        var prompt = promptBuilder.ToString();

        return prompt;
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        GC.SuppressFinalize(this);
    }

    private LanguageModelOptions ConvertToLanguageModelOptions(ChatOptions? options)
    {
        if (options is null)
            return new();

        var languageModelOptions = options.RawRepresentationFactory?.Invoke(this) as LanguageModelOptions ?? new();

        if (options.Temperature is { } temp)
            languageModelOptions.Temperature = temp;

        if (options.TopK is { } topK)
            languageModelOptions.TopK = (uint)topK;

        if (options.TopP is { } topP)
            languageModelOptions.TopP = (uint)topP;

        return languageModelOptions;
    }

    private static async Task<LanguageModel> CreateModelAsync(CancellationToken cancellationToken)
    {
        var readyState = LanguageModel.GetReadyState();

        if (readyState is AIFeatureReadyState.DisabledByUser or AIFeatureReadyState.NotSupportedOnCurrentSystem)
        {
            var message = readyState switch
            {
                AIFeatureReadyState.NotSupportedOnCurrentSystem => "Not supported",
                AIFeatureReadyState.DisabledByUser => "Disabled by user",
                _ => "Unknown reason"
            };
            throw new NotSupportedException($"PhiSilica is not available: {message}");
        }

        if (readyState is AIFeatureReadyState.NotReady)
        {
            var operation = await LanguageModel.EnsureReadyAsync();

            if (operation.Status is not AIFeatureReadyResultState.Success)
                throw new NotSupportedException($"PhiSilica is not available");
        }

        if (LanguageModel.GetReadyState() is not AIFeatureReadyState.Ready)
        {
            throw new NotSupportedException("PhiSilica is not available");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var languageModel = await LanguageModel.CreateAsync();

        return languageModel;
    }
}
