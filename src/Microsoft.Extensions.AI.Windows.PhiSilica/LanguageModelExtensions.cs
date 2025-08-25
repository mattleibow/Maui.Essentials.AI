using Microsoft.Windows.AI.Text;
using System.Runtime.CompilerServices;
using Windows.Foundation;

namespace Microsoft.Extensions.AI.Windows.PhiSilica;

public static class LanguageModelExtensions
{
    public static async IAsyncEnumerable<string> GenerateResponseAsync(
        this LanguageModel model,
        LanguageModelContext context,
        string prompt,
        LanguageModelOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var newPartEvent = new ManualResetEventSlim(false);

        if ((ulong)prompt.Length > model.GetUsablePromptLength(context, prompt))
        {
            yield return "\nPrompt larger than context";
            yield break;
        }

        var progress = model.GenerateResponseAsync(context, prompt, options ?? new());

        string currentResponse = string.Empty;

        progress.Progress = (result, value) =>
        {
            currentResponse = value;
            newPartEvent.Set();
            if (cancellationToken.IsCancellationRequested)
            {
                progress.Cancel();
            }
        };

        while (progress.Status != AsyncStatus.Completed)
        {
            await Task.CompletedTask.ConfigureAwait(ConfigureAwaitOptions.ForceYielding);

            if (newPartEvent.Wait(10, cancellationToken))
            {
                yield return currentResponse;
                newPartEvent.Reset();
            }
        }

        var response = await progress;

        yield return response?.Status switch
        {
            LanguageModelResponseStatus.BlockedByPolicy => "\nBlocked by policy",
            LanguageModelResponseStatus.PromptBlockedByContentModeration => "\nPrompt blocked by content moderation",
            LanguageModelResponseStatus.ResponseBlockedByContentModeration => "\nResponse blocked by content moderation",
            LanguageModelResponseStatus.PromptLargerThanContext => "\nPrompt larger than context",
            LanguageModelResponseStatus.Error => "\nError",
            _ => string.Empty,
        };
    }
}
