using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.AI;

namespace Maui.Essentials.AI;

/// <summary>
/// Extensions for adding AI services to Maui applications
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Adds AI services to the Maui application
    /// </summary>
    /// <param name="builder">The MauiAppBuilder</param>
    /// <returns>The MauiAppBuilder for chaining</returns>
    public static MauiAppBuilder UseAI(this MauiAppBuilder builder)
    {
#if ANDROID
        builder.Services.AddGoogleAIEdge();
#elif WINDOWS
        builder.Services.AddPhiSilica();
#elif IOS || MACCATALYST
        // builder.Services.AddFoundationModels(); // Future implementation
        throw new PlatformNotSupportedException("AI services not yet implemented for iOS/macOS");
#else
        throw new PlatformNotSupportedException("AI services not available on this platform");
#endif
        
        return builder;
    }
}

/// <summary>
/// Extensions for adding platform-specific AI services
/// </summary>
public static class ServiceCollectionExtensions
{
#if ANDROID
    /// <summary>
    /// Adds Google AI Edge (Gemini Nano) services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddGoogleAIEdge(this IServiceCollection services)
    {
        services.AddScoped<IChatClient, AICoreChatClient>();
        return services;
    }
#endif

#if WINDOWS
    /// <summary>
    /// Adds Phi Silica AI services to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPhiSilica(this IServiceCollection services)
    {
        services.AddScoped<IChatClient>(provider => 
        {
            var client = PhiSilicaClient.CreateAsync().GetAwaiter().GetResult();
            return client ?? throw new InvalidOperationException("Failed to create Phi Silica client");
        });

        return services;
    }

    /// <summary>
    /// Adds Phi Silica AI services to the service collection with a custom factory
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="factory">Custom factory function for creating the client</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPhiSilica(this IServiceCollection services, 
        Func<IServiceProvider, PhiSilicaClient> factory)
    {
        services.AddScoped<IChatClient>(factory);
        return services;
    }
#endif

#if IOS || MACCATALYST
    /// <summary>
    /// Adds Foundation Models services to the service collection (iOS/macOS)
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddFoundationModels(this IServiceCollection services)
    {
        // Future implementation for iOS Foundation models
        throw new NotImplementedException("Foundation Models not yet implemented");
    }
#endif
}