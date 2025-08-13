using Maui.Essentials.AI.SampleApp.Models;

namespace Maui.Essentials.AI.SampleApp.Services;

public interface IChatService
{
    Task<ChatMessage> SendAsync(IEnumerable<ChatMessage> history, string input, CancellationToken ct = default);
}
