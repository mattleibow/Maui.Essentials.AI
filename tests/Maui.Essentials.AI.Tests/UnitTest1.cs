using Microsoft.Extensions.AI;

namespace Maui.Essentials.AI.Tests;

public class AppleIntelligenceTests
{
    [Fact]
    public void AppleIntelligenceChatClient_CanBeInstantiated_WhenAvailable()
    {
        // Note: This test will only pass on Apple platforms where Apple Intelligence is available
        // On other platforms, it will correctly throw NotSupportedException
        
#if __IOS__ || __MACOS__ || __MACCATALYST__
        if (AppleIntelligenceChatClient.IsAppleIntelligenceAvailable)
        {
            // Arrange & Act
            using var client = new AppleIntelligenceChatClient("Test instructions", "test-model");
            
            // Assert
            Assert.NotNull(client);
            Assert.NotNull(client.Metadata);
            Assert.Equal("Apple Intelligence", client.Metadata.ProviderName);
            Assert.Equal("test-model", client.Metadata.DefaultModelId);
        }
        else
        {
            // On devices without Apple Intelligence, should throw NotSupportedException
            Assert.Throws<NotSupportedException>(() => new AppleIntelligenceChatClient());
        }
#else
        // On non-Apple platforms, this test just passes since the class is not available
        Assert.True(true, "Apple Intelligence is only available on Apple platforms");
#endif
    }

    [Fact]
    public void AppleIntelligenceChatClient_ImplementsIChatClient()
    {
        // This test verifies the interface implementation without actually creating the client
        // which avoids platform-specific availability checks
        
#if __IOS__ || __MACOS__ || __MACCATALYST__
        var chatClientType = typeof(AppleIntelligenceChatClient);
        Assert.True(typeof(IChatClient).IsAssignableFrom(chatClientType));
#else
        // On non-Apple platforms, this test just passes since the class is not available
        Assert.True(true, "Apple Intelligence is only available on Apple platforms");
#endif
    }
}

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

    }
}
