using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;

#if ANDROID
using Google.AI.Edge.AICore;
using AndroidX.Lifecycle;
#endif

namespace Maui.Essentials.AI;

#if ANDROID
/// <summary>
/// Android implementation of ChatClient using Google AI Edge (Gemini Nano)
/// </summary>
public sealed class AndroidChatClient : ChatClientBase
{
    private readonly GenerativeModel _model;
    private readonly string _modelName;
    private bool _disposed;

    /// <summary>
    /// Creates a new AndroidChatClient instance
    /// </summary>
    /// <param name="model">The GenerativeModel instance to use</param>
    /// <param name="modelName">Name of the model for responses</param>
    public AndroidChatClient(GenerativeModel model, string modelName = "Gemini-Nano")
    {
        _model = model ?? throw new ArgumentNullException(nameof(model));
        _modelName = modelName;
    }

    /// <summary>
    /// Gets a chat completion response from Gemini Nano
    /// </summary>
    public override async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AndroidChatClient));
        
        try
        {
            var contents = ConvertMessagesToContents(chatMessages);
            
            var response = await _model.GenerateContentAsync(cancellationToken, contents);
            
            var responseText = ExtractTextFromResponse(response);
            
            var chatMessage = new ChatMessage(ChatRole.Assistant, responseText);
            return new ChatResponse(chatMessage)
            {
                ModelId = _modelName,
                FinishReason = ChatFinishReason.Stop
            };
        }
        catch (Exception ex) when (!(ex is OperationCanceledException))
        {
            // Convert to a more appropriate exception type if needed
            throw new InvalidOperationException($"Error generating content: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Gets streaming chat completion updates from Gemini Nano
    /// </summary>
    public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AndroidChatClient));
        
        var contents = ConvertMessagesToContents(chatMessages);
        
        await foreach (var response in _model.GenerateContentStreamAsync(cancellationToken, contents))
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var text = ExtractTextFromResponse(response);
            
            if (!string.IsNullOrEmpty(text))
            {
                yield return new ChatResponseUpdate
                {
                    Contents = [new TextContent(text)],
                    ModelId = _modelName,
                    Role = ChatRole.Assistant
                };
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

    /// <summary>
    /// Prepare the inference engine for faster responses
    /// </summary>
    public async Task PrepareInferenceEngineAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AndroidChatClient));
        await _model.PrepareInferenceEngineAsync(cancellationToken);
    }

    /// <summary>
    /// Prepare the inference engine with lifecycle management
    /// </summary>
    public async Task PrepareInferenceEngineAsync(ILifecycleOwner lifecycleOwner, CancellationToken cancellationToken = default)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(AndroidChatClient));
        await _model.PrepareInferenceEngineAsync(lifecycleOwner, cancellationToken);
    }

    private static Content[] ConvertMessagesToContents(IEnumerable<ChatMessage> chatMessages)
    {
        var contents = new List<Content>();
        
        foreach (var message in chatMessages)
        {
            if (message.Role == ChatRole.User && !string.IsNullOrEmpty(message.Text))
            {
                // For Gemini Nano, we typically only send the user message
                // System messages and assistant messages are handled differently
                var content = new Content.Builder()
                    .AddText(message.Text)
                    .Build();
                contents.Add(content);
            }
        }
        
        // If no user messages found, create a default content
        if (contents.Count == 0)
        {
            var content = new Content.Builder()
                .AddText("Hello")
                .Build();
            contents.Add(content);
        }
        
        return contents.ToArray();
    }

    private static string ExtractTextFromResponse(GenerateContentResponse response)
    {
        // Extract text from the response using the Text property
        return response.Text ?? string.Empty;
    }

    public override void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            // GenerativeModel doesn't implement IDisposable, so no cleanup needed
            base.Dispose();
        }
    }
}
#endif