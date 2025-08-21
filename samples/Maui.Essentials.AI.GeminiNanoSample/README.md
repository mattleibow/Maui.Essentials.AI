# Maui.Essentials.AI.GeminiNanoSample

This is a .NET for Android conversion of the Google AI Edge SDK Gemini Nano sample application. 

## Overview

This sample demonstrates how to integrate Google AI Edge SDK with Gemini Nano in a .NET for Android application, converted from the original Java/Kotlin Android sample.

## Features

- **Model Download**: Automatic download of the Gemini Nano model with progress tracking
- **Text Generation**: Both streaming and non-streaming text generation modes
- **Configuration Dialog**: Adjustable AI parameters (temperature, topK, max output tokens)
- **Chat Interface**: RecyclerView-based chat interface for conversation history
- **C# Implementation**: Full conversion from Java/Kotlin to C#

## Project Structure

```
├── MainActivity.cs                 # Main activity combining entry choice and chat functionality
├── ContentAdapter.cs              # RecyclerView adapter for chat messages
├── GenerationConfigDialog.cs      # Configuration dialog for AI parameters
├── GenerationConfigUtils.cs       # Utility class for managing AI configuration
├── Platforms/Android/
│   └── AndroidManifest.xml       # Android manifest
└── Resources/                     # Android resources (layouts, strings, etc.)
    ├── layout/
    ├── values/
    └── drawable/
```

## Dependencies

The project uses the following key dependencies:
- `AndroidX.AppCompat` - For modern Android UI components
- `AndroidX.RecyclerView` - For the chat interface
- `Google.Android.Material` - For Material Design components
- `AndroidX.PreferenceKtx` - For configuration persistence
- `AndroidX.Fragment` - For dialog fragments

## Google AI Edge SDK Integration

⚠️ **Important Note**: This conversion currently contains placeholder implementations for Google AI Edge SDK calls, as the SDK may not have official .NET bindings. The following would need to be implemented:

1. **Model Initialization**: Replace placeholder `_model` object with actual Google AI Edge SDK integration
2. **Model Download**: Implement real model download with `DownloadConfig` and `DownloadCallback`
3. **Content Generation**: Replace `SimulateGeneration()` and `SimulateStreamingGeneration()` with actual SDK calls
4. **Streaming**: Implement proper streaming with reactive patterns

## Building and Running

1. Ensure you have .NET 8 and Android SDK installed
2. Build the project:
   ```bash
   dotnet build
   ```
3. Deploy to an Android device with API level 31 or higher:
   ```bash
   dotnet build -t:Run
   ```

## Converting from Original

This project was converted from the original Google AI Edge SDK sample with the following key changes:

### Language Conversion
- **Java/Kotlin → C#**: All source files converted to C# with .NET idioms
- **Async Patterns**: Java Futures and Kotlin coroutines converted to C# async/await
- **Event Handling**: Java/Kotlin listeners converted to C# events
- **Null Safety**: Kotlin null-safety features converted to C# nullable reference types

### Architecture Changes
- **Combined Activities**: `EntryChoiceActivity` and `MainActivity` combined into single `MainActivity.cs`
- **Resource Access**: Android resource access adapted for .NET Android patterns
- **Dependency Injection**: Ready for integration with .NET dependency injection if needed

### Placeholder Implementations
- **AI SDK Calls**: All Google AI Edge SDK calls are currently simulated
- **Streaming**: Mock streaming implementation that would need real SDK integration
- **Model Management**: Placeholder model lifecycle management

## Next Steps for Full Implementation

To complete the conversion, you would need to:

1. **Create or obtain .NET bindings** for Google AI Edge SDK (`com.google.ai.edge.aicore`)
2. **Replace placeholders** with actual SDK calls in:
   - `EnsureModelDownloaded()`
   - `InitGenerativeModel()`
   - `GenerateContent()`
   - `SimulateGeneration()` / `SimulateStreamingGeneration()`
3. **Handle native dependencies** that the Google AI Edge SDK requires
4. **Test on devices** with AICore installed

## License

This project maintains the same Apache 2.0 license as the original Google sample.