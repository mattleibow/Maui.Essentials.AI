using System.Threading.Channels;

namespace Microsoft.Extensions.AI.Apple.FoundationModels;

public static class LanguageModelSessionExtensions
{

    public static IAsyncEnumerable<string> GenerateContentStreamAsync(this LanguageModelSession session, string prompt) =>
        GenerateContentStreamAsync(session, CancellationToken.None, prompt);

    public static IAsyncEnumerable<string> GenerateContentStreamAsync(this LanguageModelSession session, CancellationToken cancellationToken, string prompt)
    {
        var channel = Channel.CreateUnbounded<string>();
        var writer = channel.Writer;
        var reader = channel.Reader;

        session.StreamResponse(
            prompt,
            onNext: response => writer.TryWrite(response),
            onComplete: (response, error) => writer.TryComplete(error is null ? null : new NSErrorException(error)));

        return reader.ReadAllAsync(cancellationToken);
    }

}
