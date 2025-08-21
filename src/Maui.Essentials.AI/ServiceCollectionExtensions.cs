using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

#if ANDROID
using Google.AI.Edge.AICore;
using Android.Content;
#endif

namespace Maui.Essentials.AI;

/// <summary>
/// Extension methods for configuring AI services in MAUI applications
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Adds AI services to the MAUI application, automatically selecting the best available implementation for the platform
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <returns>The same MAUI app builder for chaining</returns>
    public static MauiAppBuilder UseAI(this MauiAppBuilder builder)
    {
        builder.Services.AddAI();
        return builder;
    }

    /// <summary>
    /// Adds AI services to the MAUI application with custom configuration
    /// </summary>
    /// <param name="builder">The MAUI app builder</param>
    /// <param name="configure">Configuration action for AI options</param>
    /// <returns>The same MAUI app builder for chaining</returns>
    public static MauiAppBuilder UseAI(this MauiAppBuilder builder, Action<AIOptions> configure)
    {
        builder.Services.AddAI(configure);
        return builder;
    }
}

/// <summary>
/// Extension methods for configuring AI services in dependency injection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds AI services to the service collection, automatically selecting the best available implementation for the platform
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The same service collection for chaining</returns>
    public static IServiceCollection AddAI(this IServiceCollection services)
    {
        return services.AddAI(_ => { });
    }

    /// <summary>
    /// Adds AI services to the service collection with custom configuration
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configure">Configuration action for AI options</param>
    /// <returns>The same service collection for chaining</returns>
    public static IServiceCollection AddAI(this IServiceCollection services, Action<AIOptions> configure)
    {
        var options = new AIOptions();
        configure(options);

        // Register the options
        services.AddSingleton(options);

        // Register the appropriate IChatClient implementation for the platform
#if ANDROID
        services.AddSingleton<IChatClient>(serviceProvider =>
        {
            var aiOptions = serviceProvider.GetRequiredService<AIOptions>();
            
            if (aiOptions.UseAndroidGeminiNano)
            {
                try
                {
                    // Try to create GenerativeModel for Gemini Nano
                    var configBuilder = new GenerationConfig.Builder()
                    {
                        Context = Android.App.Application.Context
                    };
                    
                    // Apply any configuration from options
                    if (aiOptions.AndroidGeminiNanoConfiguration != null)
                    {
                        aiOptions.AndroidGeminiNanoConfiguration(configBuilder);
                    }
                    
                    var model = new GenerativeModel(configBuilder.Build());
                    return new AndroidChatClient(model, "Gemini-Nano");
                }
                catch
                {
                    // Fall back to echo client if Gemini Nano is not available
                    return new EchoChatClient("Echo-Fallback");
                }
            }
            
            return new EchoChatClient("Echo");
        });
#else
        // For other platforms, use the echo client as a placeholder
        services.AddSingleton<IChatClient>(serviceProvider =>
        {
            var aiOptions = serviceProvider.GetRequiredService<AIOptions>();
            return new EchoChatClient(aiOptions.EchoModelName ?? "Echo");
        });
#endif

        return services;
    }
}

/// <summary>
/// Configuration options for AI services
/// </summary>
public class AIOptions
{
    /// <summary>
    /// Whether to use Android Gemini Nano when available (Android only)
    /// </summary>
    public bool UseAndroidGeminiNano { get; set; } = true;

    /// <summary>
    /// Model name to use for echo client on platforms without native AI
    /// </summary>
    public string? EchoModelName { get; set; }

#if ANDROID
    /// <summary>
    /// Configuration action for Android Gemini Nano generation config builder
    /// </summary>
    public Action<GenerationConfig.Builder>? AndroidGeminiNanoConfiguration { get; set; }
#endif
}