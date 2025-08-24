using System.Runtime.Versioning;
using System.Threading.Channels;

namespace Microsoft.Extensions.AI.Apple.FoundationModels;

[SupportedOSPlatform ("ios26.0")]
[SupportedOSPlatform ("maccatalyst26.0")]
[SupportedOSPlatform ("macos26.0")]
public static class LanguageModelSessionExtensions
{

    public static IAsyncEnumerable<string> StreamResponseAsync(this LanguageModelSession session, string prompt, GenerationOptions? options = null, CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<string>();
        var writer = channel.Writer;
        var reader = channel.Reader;

        session.StreamResponse(
            prompt,
            options,
            onNext: response => writer.TryWrite(response),
            onComplete: (response, error) => writer.TryComplete(error is null ? null : new NSErrorException(error)));

        return reader.ReadAllAsync(cancellationToken);
    }

}
