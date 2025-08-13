using Maui.Essentials.AI.SampleApp.Models;

namespace Maui.Essentials.AI.SampleApp.Services;

// Simple placeholder that echoes back with minor delay to simulate AI
public class EchoChatService : IChatService
{
    public async Task<ChatMessage> SendAsync(IEnumerable<ChatMessage> history, string input, CancellationToken ct = default)
    {
        await Task.Delay(600, ct);
        return new ChatMessage
        {
            Role = ChatRole.Assistant,
            Text = $"You said: {input}"
        };
    }
}
