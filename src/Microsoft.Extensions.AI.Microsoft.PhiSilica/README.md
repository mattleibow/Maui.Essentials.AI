# Microsoft.Extensions.AI.Microsoft.PhiSilica

Windows implementation of `IChatClient` using Microsoft Phi Silica on-device models.

## Overview

This package provides a `PhiSilicaChatClient` that enables on-device AI inference on Windows using Microsoft's Phi Silica models. It integrates seamlessly with the Microsoft.Extensions.AI framework.

## Features

- ✅ On-device inference for privacy and offline capability
- ✅ Streaming and non-streaming chat completions
- ✅ Full integration with Microsoft.Extensions.AI abstractions
- ✅ Dependency injection support
- ✅ Windows 10 version 1809 (17763) and later support

## Platform Support

- **Windows 10** version 1809 (build 17763) and later
- **Windows 11** all versions

## Usage

### Basic Usage

```csharp
using Microsoft.Extensions.AI.Microsoft.PhiSilica;

// Create client directly
using var client = new PhiSilicaChatClient();

// Send a message
var messages = new[]
{
    new ChatMessage(ChatRole.User, "What is the capital of France?")
};

var response = await client.GetResponseAsync(messages);
Console.WriteLine(response.Messages[0].Text);
```

### Streaming Usage

```csharp
using var client = new PhiSilicaChatClient();

var messages = new[]
{
    new ChatMessage(ChatRole.User, "Tell me a short story")
};

await foreach (var update in client.GetStreamingResponseAsync(messages))
{
    if (update.Contents.Any())
    {
        Console.Write(update.Contents[0].Text);
    }
}
```

### Dependency Injection

```csharp
using Microsoft.Extensions.AI.Microsoft.PhiSilica;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

// Register PhiSilica as the default IChatClient
services.AddPhiSilica();

// Or register with a service key
services.AddPhiSilica("phi-silica");

var serviceProvider = services.BuildServiceProvider();
var chatClient = serviceProvider.GetRequiredService<IChatClient>();
```

### MAUI Integration

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        // Platform-specific registration
#if WINDOWS
        builder.Services.AddPhiSilica();
#elif ANDROID
        builder.Services.AddGoogleAIEdge();
#elif IOS || MACCATALYST
        builder.Services.AddAppleIntelligence();
#endif

        return builder.Build();
    }
}
```

## Model Support

Currently supports Phi-3.5-mini-instruct as the default model. The implementation uses the Phi-3 chat format:

```
<|system|>
System instructions here<|end|>
<|user|>
User message here<|end|>
<|assistant|>
Assistant response here<|end|>
```

## Implementation Notes

This is currently a placeholder implementation that demonstrates the integration pattern. The actual Phi Silica inference engine integration will be completed when the appropriate SDK is available.

The placeholder implementation:
- Accepts messages in the proper chat format
- Returns structured responses
- Supports both streaming and non-streaming modes
- Follows all Microsoft.Extensions.AI conventions

## License

This project is licensed under the MIT License - see the LICENSE file for details.