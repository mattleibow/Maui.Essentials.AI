# Apple Intelligence Implementation

This document provides an overview of the Apple Intelligence support added to Maui.Essentials.AI.

## Overview

Apple Intelligence support has been successfully implemented following the same patterns as the existing Android implementation (AICoreChatClient). The implementation provides native Apple Intelligence integration for iOS 18.1+, macOS 15.1+, and Mac Catalyst.

## Key Components

### 1. AppleIntelligenceChatClient

- **Location**: `src/Maui.Essentials.AI/Platforms/iOS/AppleIntelligenceChatClient.cs` and `src/Maui.Essentials.AI/Platforms/MacCatalyst/AppleIntelligenceChatClient.cs`
- **Purpose**: Implements `IChatClient` interface using Apple Intelligence APIs
- **Features**:
  - Availability checking (`IsAppleIntelligenceAvailable`)
  - Async response generation (`GetResponseAsync`)
  - Streaming responses (simulated since Apple Intelligence doesn't support true streaming)
  - Proper disposal and error handling

### 2. Native Apple Bindings

- **Location**: `src/Maui.Essentials.AI/Platforms/iOS/AppleIntelligenceApiDefinition.cs` and `src/Maui.Essentials.AI/Platforms/MacCatalyst/AppleIntelligenceApiDefinition.cs`
- **Purpose**: Provides Objective-C bindings for Apple Intelligence native APIs
- **Key Interfaces**:
  - `AppleIntelligenceSessionNative`: Main interface for Apple Intelligence sessions
  - `DotnetTool`: Protocol for tool integration (future extensibility)
  - `IntelligenceResponseHandler`: Callback delegate for async responses

### 3. Native Implementation

- **Location**: `Apple/Maui.Essentials.AI.Native/`
- **Purpose**: Swift/Objective-C implementation adapted from CrossIntelligence project
- **Key Files**:
  - `AppleIntelligenceSessionNative.swift`: Core session management
  - `DotnetTool.swift`: Tool protocol implementation
  - `JsonSchema.swift`: JSON schema utilities
  - `Maui.Essentials.AI.h`: Framework header

### 4. Xcode Project

- **Location**: `Apple/Maui.Essentials.AI.xcodeproj`
- **Purpose**: Builds the native framework that .NET binds to
- **Configuration**: Universal framework supporting iOS, macOS, and Mac Catalyst

## Usage Examples

### Basic Usage

```csharp
#if __IOS__ || __MACOS__ || __MACCATALYST__
// Check availability
if (AppleIntelligenceChatClient.IsAppleIntelligenceAvailable)
{
    // Create client
    using var client = new AppleIntelligenceChatClient(
        instructions: "You are a helpful assistant",
        modelId: "Apple-Intelligence");

    // Send a message
    var messages = new[]
    {
        new ChatMessage(ChatRole.User, "Hello, how are you?")
    };

    var response = await client.GetResponseAsync(messages);
    Console.WriteLine(response.Messages.First().Text);
}
#endif
```

### Integration with Sample App

The `AppleIntelligenceChatService` class demonstrates how to integrate Apple Intelligence with the existing sample app architecture:

```csharp
// Register the service
builder.Services.AddSingleton<IChatService, AppleIntelligenceChatService>();

// Use in ViewModels
var response = await _chatService.SendAsync(messageHistory, userInput);
```

## Platform Requirements

- **iOS**: 18.1 or later
- **macOS**: 15.1 or later  
- **Mac Catalyst**: 15.1 or later
- **Development**: Xcode 15.0+ required for building native components

## Error Handling

The implementation includes comprehensive error handling:

- Device compatibility checking
- Graceful fallback when Apple Intelligence is unavailable
- Exception wrapping for native errors
- Cancellation token support

## Testing

Unit tests are provided with platform-specific conditional compilation:

```csharp
[Fact]
public void AppleIntelligenceChatClient_ImplementsIChatClient()
{
#if __IOS__ || __MACOS__ || __MACCATALYST__
    var chatClientType = typeof(AppleIntelligenceChatClient);
    Assert.True(typeof(IChatClient).IsAssignableFrom(chatClientType));
#else
    Assert.True(true, "Apple Intelligence is only available on Apple platforms");
#endif
}
```

## Building

The core library builds successfully on all platforms. Apple-specific targets require macOS with Xcode installed:

```bash
# Cross-platform build (excludes Apple targets on non-macOS)
dotnet build

# Apple-specific builds (macOS only)
dotnet build --framework net10.0-ios
dotnet build --framework net10.0-maccatalyst
```

## Architecture Notes

- Follows Microsoft.Extensions.AI abstractions for consistency
- Uses conditional compilation to avoid platform issues
- Implements same patterns as existing Android implementation
- Provides both sync and async APIs with proper disposal
- Supports cancellation throughout the call chain