using System.Text.Json;
using System.Text.Json.Nodes;
using Maui.Essentials.AI.SampleApp.ViewModels;
using Microsoft.Extensions.AI;

namespace Maui.Essentials.AI.SampleApp.AI;

public class WordSelectionTool(ChatViewModel chatViewModel) : AIFunction
{
    private const string SchemaString = """
        {
            "type": "object",
            "properties": {
                "words": {
                    "type": "array",
                    "items": {
                        "type": "string"
                    }
                }
            },
            "required": ["words"]
        }
        """;

    public override string Name =>
        "selectWord";

    public override string Description =>
        "Asks the user to select a word from a list of words.";
        
    public override JsonElement JsonSchema =>
        JsonDocument.Parse(SchemaString).RootElement;

    protected override async ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var wordsElement = arguments["words"] as JsonElement?;
        var wordsArray = wordsElement?.EnumerateArray().ToArray();
        var words = wordsArray?.Select(e => e.GetString()!).Where(s => !string.IsNullOrEmpty(s)).ToArray();

        if (words is null || words.Length == 0)
            return "No words were provided.";

        var args = new WordSelectionStartedEventArgs(words);

        chatViewModel.SelectWordCommand.Execute(args);

        try
        {
            var selectedWord = await args.Task!;

            if (selectedWord is null)
                return "The user did not want to select a word.";

            return "The user selected this word: " + selectedWord;
        }
        catch (OperationCanceledException)
        {
            return "The user canceled the word selection.";
        }
        catch (Exception ex)
        {
            return "An error occurred while selecting words: " + ex.Message;
        }
    }
}
