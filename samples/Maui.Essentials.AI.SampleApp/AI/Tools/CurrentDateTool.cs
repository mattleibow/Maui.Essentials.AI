using Microsoft.Extensions.AI;

namespace Maui.Essentials.AI.SampleApp.AI;

public class CurrentDateTool : AIFunction
{
    public override string Name =>
        "getCurrentDate";

    public override string Description =>
        "Gets the current date";

    protected override ValueTask<object?> InvokeCoreAsync(AIFunctionArguments arguments, CancellationToken cancellationToken)
    {
        var currentDate = DateTimeOffset.Now;
        var dateString = currentDate.ToString("d MMMM yyyy");
        return ValueTask.FromResult<object?>(dateString);
    }
}
