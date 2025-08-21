using Microsoft.Extensions.AI;
using System.Runtime.CompilerServices;
using Foundation;

namespace Maui.Essentials.AI;

class Testing
{
    void Test()
    {
        var av = SystemLanguageModel.Shared.IsAvailable;
    }
}

// /// <summary>
// /// Apple Intelligence implementation of ChatClient using native Apple Intelligence APIs
// /// </summary>
// public sealed class AppleIntelligenceChatClient : IChatClient
// {
//     private readonly AppleIntelligenceSessionNative _session;
//     private readonly ChatClientMetadata _metadata;
//     private bool _disposed;

//     public ChatClientMetadata Metadata => _metadata;

//     /// <summary>
//     /// Gets whether Apple Intelligence is available on this device
//     /// </summary>
//     public static bool IsAppleIntelligenceAvailable => AppleIntelligenceSessionNative.IsAppleIntelligenceAvailable;

//     /// <summary>
//     /// Creates a new AppleIntelligenceChatClient instance
//     /// </summary>
//     /// <param name="instructions">System instructions for the AI model</param>
//     /// <param name="modelId">ID of the model for responses</param>
//     public AppleIntelligenceChatClient(string? instructions = null, string? modelId = null)
//     {
//         if (!IsAppleIntelligenceAvailable)
//         {
//             throw new NotSupportedException("Apple Intelligence is not available on this device or operating system version.");
//         }

//         var systemInstructions = instructions ?? "You are a helpful AI assistant.";
//         _session = new AppleIntelligenceSessionNative(systemInstructions, Array.Empty<NSObject>());
        
//         _metadata = new ChatClientMetadata(
//             providerName: "Apple Intelligence",
//             defaultModelId: modelId ?? "Apple-Intelligence");
//     }

//     /// <summary>
//     /// Gets a chat completion response from Apple Intelligence
//     /// </summary>
//     public async Task<ChatResponse> GetResponseAsync(
//         IEnumerable<ChatMessage> chatMessages,
//         ChatOptions? options = null,
//         CancellationToken cancellationToken = default)
//     {
//         ObjectDisposedException.ThrowIf(_disposed, this);

//         try
//         {
//             var prompt = BuildPromptFromMessages(chatMessages);
//             var response = await GetIntelligenceResponseAsync(prompt, cancellationToken);

//             return new ChatResponse
//             {
//                 Messages = { new ChatMessage(ChatRole.Assistant, response) },
//                 ModelId = options?.ModelId ?? _metadata.DefaultModelId,
//                 FinishReason = ChatFinishReason.Stop
//             };
//         }
//         catch (Exception ex) when (ex is not OperationCanceledException)
//         {
//             throw new InvalidOperationException($"Error generating content with Apple Intelligence: {ex.Message}", ex);
//         }
//     }

//     /// <summary>
//     /// Gets streaming chat completion updates from Apple Intelligence
//     /// Note: Apple Intelligence doesn't support streaming, so this simulates streaming by yielding the complete response
//     /// </summary>
//     public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
//         IEnumerable<ChatMessage> chatMessages,
//         ChatOptions? options = null,
//         [EnumeratorCancellation] CancellationToken cancellationToken = default)
//     {
//         ObjectDisposedException.ThrowIf(_disposed, this);

//         var prompt = BuildPromptFromMessages(chatMessages);
//         var response = await GetIntelligenceResponseAsync(prompt, cancellationToken);

//         // Since Apple Intelligence doesn't support true streaming, we yield the complete response
//         if (!string.IsNullOrEmpty(response))
//         {
//             yield return new ChatResponseUpdate
//             {
//                 Contents = [new TextContent(response)],
//                 ModelId = options?.ModelId ?? _metadata.DefaultModelId,
//                 Role = ChatRole.Assistant
//             };
//         }

//         // Final update to indicate completion
//         yield return new ChatResponseUpdate
//         {
//             FinishReason = ChatFinishReason.Stop,
//             ModelId = options?.ModelId ?? _metadata.DefaultModelId,
//             Role = ChatRole.Assistant
//         };
//     }

//     /// <summary>
//     /// Gets a service instance from the client
//     /// </summary>
//     object? IChatClient.GetService(Type serviceType, object? serviceKey)
//     {
//         ArgumentNullException.ThrowIfNull(serviceType);

//         return
//             serviceKey is not null ? null :
//             serviceType == typeof(ChatClientMetadata) ? _metadata :
//             serviceType.IsInstanceOfType(this) ? this :
//             null;
//     }

//     private static string BuildPromptFromMessages(IEnumerable<ChatMessage> chatMessages)
//     {
//         var messages = chatMessages.ToList();
//         if (!messages.Any())
//         {
//             return string.Empty;
//         }

//         // For Apple Intelligence, we'll concatenate all messages into a single prompt
//         // In a more sophisticated implementation, we might maintain conversation context differently
//         var lastUserMessage = messages.LastOrDefault(m => m.Role == ChatRole.User);
//         return lastUserMessage?.Text ?? string.Empty;
//     }

//     private async Task<string> GetIntelligenceResponseAsync(string prompt, CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrWhiteSpace(prompt))
//         {
//             return string.Empty;
//         }

//         var tcs = new TaskCompletionSource<string>();
        
//         // Register cancellation callback
//         using var cancellationRegistration = cancellationToken.Register(() => 
//         {
//             tcs.TrySetCanceled(cancellationToken);
//         });

//         _session.Respond(prompt, (response, error) =>
//         {
//             if (error != null)
//             {
//                 tcs.TrySetException(new Exception($"Apple Intelligence error: {error.LocalizedDescription}"));
//             }
//             else
//             {
//                 tcs.TrySetResult(response ?? string.Empty);
//             }
//         });

//         return await tcs.Task;
//     }

//     public void Dispose()
//     {
//         if (_disposed)
//             return;

//         _disposed = true;
//         _session?.FreeTools();
//         _session?.Dispose();

//         GC.SuppressFinalize(this);
//     }
// }
// #endif