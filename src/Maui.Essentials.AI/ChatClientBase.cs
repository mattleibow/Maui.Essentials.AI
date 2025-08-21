using Microsoft.Extensions.AI;

namespace Maui.Essentials.AI;

/// <summary>
/// Base abstract implementation of IChatClient for Maui.Essentials.AI
/// </summary>
public abstract class ChatClientBase : IChatClient
{
    /// <summary>
    /// Gets a chat completion response from the AI model
    /// </summary>
    public abstract Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets streaming chat completion updates from the AI model
    /// </summary>
    public abstract IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages, 
        ChatOptions? options = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a service instance from the client
    /// </summary>
    public virtual object? GetService(Type serviceType, object? serviceKey = null)
    {
        // Default implementation returns null - subclasses can override for specific services
        return serviceType.IsInstanceOfType(this) ? this : null;
    }

    /// <summary>
    /// Disposes resources used by the chat client
    /// </summary>
    public virtual void Dispose()
    {
        // Default implementation - subclasses can override for cleanup
        GC.SuppressFinalize(this);
    }
}