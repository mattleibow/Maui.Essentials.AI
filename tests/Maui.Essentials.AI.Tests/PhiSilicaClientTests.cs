using Microsoft.Extensions.AI;
using Xunit;

namespace Maui.Essentials.AI.Tests;

public class PhiSilicaClientTests
{
#if WINDOWS
    [Fact]
    public void PhiSilicaClient_HasCorrectMetadata()
    {
        // This test verifies that the PhiSilicaClient class is properly defined
        // without requiring the actual Windows AI packages to be available
        
        var clientType = typeof(PhiSilicaClient);
        
        Assert.NotNull(clientType);
        Assert.True(clientType.IsClass);
        Assert.True(clientType.IsSealed);
        Assert.True(typeof(IChatClient).IsAssignableFrom(clientType));
        Assert.True(typeof(IDisposable).IsAssignableFrom(clientType));
    }

    [Fact]
    public void ServiceCollectionExtensions_HasAddPhiSilicaMethod()
    {
        // Verify that the AddPhiSilica extension method exists for Windows
        var extensionType = typeof(ServiceCollectionExtensions);
        var addPhiSilicaMethod = extensionType.GetMethods()
            .FirstOrDefault(m => m.Name == "AddPhiSilica" && m.GetParameters().Length == 1);
        
        Assert.NotNull(addPhiSilicaMethod);
        Assert.True(addPhiSilicaMethod.IsStatic);
        Assert.True(addPhiSilicaMethod.IsPublic);
    }
#endif

    [Fact]
    public void AIExtensions_HasUseAIMethod()
    {
        // Verify that the UseAI extension method exists
        var extensionType = typeof(MauiAppBuilderExtensions);
        var useAIMethod = extensionType.GetMethod("UseAI");
        
        Assert.NotNull(useAIMethod);
        Assert.True(useAIMethod.IsStatic);
        Assert.True(useAIMethod.IsPublic);
    }
}