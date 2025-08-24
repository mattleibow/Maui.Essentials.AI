using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.Versioning;

namespace Microsoft.Extensions.AI.Microsoft.PhiSilica;

/// <summary>
/// Extension methods for adding PhiSilica chat client to dependency injection
/// </summary>
public static class PhiSilicaServiceCollectionExtensions
{
    /// <summary>
    /// Adds PhiSilica chat client services to the dependency injection container
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static IServiceCollection AddPhiSilica(this IServiceCollection services)
    {
        return services.AddSingleton<IChatClient, PhiSilicaChatClient>();
    }

    /// <summary>
    /// Adds PhiSilica chat client services to the dependency injection container with a service key
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="serviceKey">The service key for keyed registration</param>
    /// <returns>The service collection for chaining</returns>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public static IServiceCollection AddPhiSilica(this IServiceCollection services, object serviceKey)
    {
        return services.AddKeyedSingleton<IChatClient, PhiSilicaChatClient>(serviceKey);
    }
}