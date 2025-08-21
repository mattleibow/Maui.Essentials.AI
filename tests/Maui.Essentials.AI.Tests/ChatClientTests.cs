using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace Maui.Essentials.AI.Tests;

public class ChatClientTests
{
    [Fact]
    public void EchoChatClient_CanBeCreated()
    {
        // Arrange & Act
        var client = new EchoChatClient("TestEcho");

        // Assert
        Assert.NotNull(client);
    }

    [Fact]
    public async Task EchoChatClient_GetResponseAsync_ReturnsExpectedResponse()
    {
        // Arrange
        var client = new EchoChatClient("TestEcho", TimeSpan.FromMilliseconds(10));
        var messages = new[]
        {
            new ChatMessage(ChatRole.User, "Hello World")
        };

        // Act
        var response = await client.GetResponseAsync(messages);

        // Assert
        Assert.NotNull(response);
        Assert.Equal("TestEcho", response.ModelId);
        Assert.Equal(ChatFinishReason.Stop, response.FinishReason);
        // For now, just verify we get a response - we can expand tests later
    }

    [Fact]
    public async Task EchoChatClient_GetStreamingResponseAsync_ReturnsExpectedUpdates()
    {
        // Arrange
        var client = new EchoChatClient("TestEcho", TimeSpan.FromMilliseconds(10));
        var messages = new[]
        {
            new ChatMessage(ChatRole.User, "Hello")
        };

        // Act
        var updates = new List<ChatResponseUpdate>();
        await foreach (var update in client.GetStreamingResponseAsync(messages))
        {
            updates.Add(update);
        }

        // Assert
        Assert.NotEmpty(updates);
        // Should have at least the text updates and a final completion update
        Assert.True(updates.Count >= 2);
        
        // Check that we get a completion update
        var completionUpdate = updates.LastOrDefault(u => u.FinishReason == ChatFinishReason.Stop);
        Assert.NotNull(completionUpdate);
        Assert.Equal("TestEcho", completionUpdate.ModelId);
    }

    [Fact]
    public void ServiceCollection_AddAI_RegistersIChatClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAI();
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<IChatClient>();
        Assert.NotNull(chatClient);
        Assert.IsType<EchoChatClient>(chatClient);
    }

    [Fact]
    public void ServiceCollection_AddAI_WithConfiguration_RegistersIChatClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddAI(options =>
        {
            options.EchoModelName = "CustomEcho";
        });
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var chatClient = serviceProvider.GetService<IChatClient>();
        Assert.NotNull(chatClient);
        Assert.IsType<EchoChatClient>(chatClient);
    }

    [Fact]
    public void MauiAppBuilder_UseAI_CanBeCalled()
    {
        // This is a simplified test since we can't easily test the full MAUI app builder
        // in a unit test context, but we can at least verify the extension method works
        
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert - should not throw
        services.AddAI();
        var serviceProvider = services.BuildServiceProvider();
        var chatClient = serviceProvider.GetService<IChatClient>();
        
        Assert.NotNull(chatClient);
    }
}
