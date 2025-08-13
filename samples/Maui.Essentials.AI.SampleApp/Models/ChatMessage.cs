namespace Maui.Essentials.AI.SampleApp.Models;

public class ChatMessage
{
    public required ChatRole Role { get; init; }

    public required string Text { get; set; }

    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.Now;

    public bool IsUser => Role == ChatRole.User;
}
