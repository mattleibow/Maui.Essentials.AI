using Microsoft.Windows.AI.Text;
using System.Threading.Channels;

namespace Microsoft.Extensions.AI.Windows.PhiSilica;

public static class LanguageModelExtensions
{
    public static IAsyncEnumerable<string> GenerateResponseAsync(
        this LanguageModel model,
        LanguageModelContext context,
        string prompt,
        LanguageModelOptions options,
        CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<string>();
        var writer = channel.Writer;
        var reader = channel.Reader;

        var progress = new Progress<string>(response => writer.TryWrite(response));

        var operation = model.GenerateResponseAsync(context, prompt, options);

        _ = operation.AsTask(cancellationToken, progress).ContinueWithWriterComplete(writer);

        return reader.ReadAllAsync(cancellationToken);
    }

    private static async Task ContinueWithWriterComplete(
        this Task<LanguageModelResponseResult> task,
        ChannelWriter<string> writer)
    {
        try
        {
            var result = await task;

            if (result.Status != LanguageModelResponseStatus.Complete)
                throw new InvalidOperationException($"Model was not able to complete successfully: {result.Status}");

            writer.TryComplete();
        }
        catch (Exception ex)
        {
            writer.TryComplete(ex);
        }
    }
}
