using Microsoft.Extensions.AI;

#if WINDOWS
using Microsoft.Extensions.AI.Microsoft.PhiSilica;
#endif

namespace Maui.Essentials.AI.Tests;

public class PhiSilicaChatClientTests
{
#if WINDOWS
    [Fact]
    public void Constructor_SetsMetadata()
    {
        // Arrange & Act
        using var client = new PhiSilicaChatClient();

        // Assert
        Assert.NotNull(client.Metadata);
        Assert.Equal("Microsoft Phi Silica", client.Metadata.ProviderName);
        Assert.Equal("Phi-3.5-mini-instruct", client.Metadata.DefaultModelId);
    }

    [Fact]
    public async Task GetResponseAsync_ReturnsValidResponse()
    {
        // Arrange
        using var client = new PhiSilicaChatClient();
        var messages = new[]
        {
            new ChatMessage(ChatRole.User, "Hello, how are you?")
        };

        // Act
        var response = await client.GetResponseAsync(messages);

        // Assert
        Assert.NotNull(response);
        Assert.Single(response.Messages);
        Assert.Equal(ChatRole.Assistant, response.Messages[0].Role);
        Assert.NotEmpty(response.Messages[0].Text);
        Assert.Equal("Phi-3.5-mini-instruct", response.ModelId);
        Assert.Equal(ChatFinishReason.Stop, response.FinishReason);
    }

    [Fact]
    public async Task GetStreamingResponseAsync_ReturnsValidUpdates()
    {
        // Arrange
        using var client = new PhiSilicaChatClient();
        var messages = new[]
        {
            new ChatMessage(ChatRole.User, "Tell me a short story")
        };

        // Act
        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in client.GetStreamingResponseAsync(messages))
        {
            updates.Add(update);
        }

        // Assert
        Assert.NotEmpty(updates);
        
        // Check that we have at least some content updates and a final completion update
        var contentUpdates = updates.Where(u => u.Contents.Any()).ToList();
        var completionUpdate = updates.LastOrDefault(u => u.FinishReason == ChatFinishReason.Stop);
        
        Assert.NotEmpty(contentUpdates);
        Assert.NotNull(completionUpdate);
        Assert.Equal("Phi-3.5-mini-instruct", completionUpdate.ModelId);
        Assert.Equal(ChatRole.Assistant, completionUpdate.Role);
    }

    [Fact]
    public void GetService_ReturnsClientForCorrectType()
    {
        // Arrange
        using var client = new PhiSilicaChatClient();

        // Act
        var service = client.GetService(typeof(PhiSilicaChatClient), null);

        // Assert
        Assert.Same(client, service);
    }

    [Fact]
    public void GetService_ReturnsNullForIncorrectType()
    {
        // Arrange
        using var client = new PhiSilicaChatClient();

        // Act
        var service = client.GetService(typeof(string), null);

        // Assert
        Assert.Null(service);
    }

    [Fact]
    public void Dispose_DoesNotThrow()
    {
        // Arrange
        var client = new PhiSilicaChatClient();

        // Act & Assert
        client.Dispose();
        
        // Should not throw when disposed multiple times
        client.Dispose();
    }

    [Fact]
    public async Task GetResponseAsync_ThrowsWhenDisposed()
    {
        // Arrange
        var client = new PhiSilicaChatClient();
        client.Dispose();
        var messages = new[] { new ChatMessage(ChatRole.User, "Hello") };

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => 
            client.GetResponseAsync(messages));
    }

    [Fact]
    public async Task GetStreamingResponseAsync_ThrowsWhenDisposed()
    {
        // Arrange
        var client = new PhiSilicaChatClient();
        client.Dispose();
        var messages = new[] { new ChatMessage(ChatRole.User, "Hello") };

        // Act & Assert
        await Assert.ThrowsAsync<ObjectDisposedException>(() => 
            client.GetStreamingResponseAsync(messages).GetAsyncEnumerator().MoveNextAsync().AsTask());
    }
#else
    [Fact]
    public void PhiSilica_NotAvailable_OnNonWindowsPlatform()
    {
        // This test ensures the project compiles on non-Windows platforms
        // The actual PhiSilica functionality is only available on Windows
        Assert.True(true, "PhiSilica is only available on Windows platforms");
    }
#endif
}