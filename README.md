# 🧠 Maui.Essentials.AI

**AI made effortless for .NET MAUI** - Bringing the power of artificial intelligence to your mobile and desktop applications with zero complexity.

[![.NET MAUI](https://img.shields.io/badge/.NET%20MAUI-supported-purple.svg)](https://dotnet.microsoft.com/apps/maui)
[![Microsoft.Extensions.AI](https://img.shields.io/badge/Microsoft.Extensions.AI-9.8.0-blue.svg)](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)
[![Cross Platform](https://img.shields.io/badge/platform-iOS%20%7C%20Android%20%7C%20Windows%20%7C%20macOS-lightgrey.svg)](#supported-platforms)

## 🚀 Vision

Maui.Essentials.AI transforms AI from a complex infrastructure challenge into something as simple as using a button or label in your MAUI app. Built on [Microsoft.Extensions.AI](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai), this library provides a unified, cross-platform AI experience that automatically adapts to your device's capabilities.

**Imagine using AI like any other UI control:**
```csharp
// As simple as this
var response = await _aiService.ChatAsync("Summarize this document");
```

## ✨ Key Features

### 🎯 **Zero Configuration AI**
- Drop-in AI capabilities for any MAUI app
- Automatic platform detection and optimization  
- No complex setup or model management required

### 🔄 **Two-Tier On-Device AI System**
Our revolutionary approach ensures your app always has AI capabilities while keeping everything on-device:

1. **🏠 On-Device First** - Leverage native, OS-provided models
   - iOS: Foundation Models & Core ML
   - Android: Google AI Edge, TensorFlow Lite
   - Windows: Phi Silica, DirectML, ONNX Runtime
   - macOS: Core ML, Metal Performance Shaders

2. **📦 Custom Models** - Your own trained models
   - Core ML integration
   - ONNX runtime support  
   - TensorFlow Lite models
   - Custom inference engines

### 🛡️ **Privacy & Performance First**
- On-device processing keeps data private
- Automatic performance optimization
- Battery-efficient inference
- Offline-capable AI features

## 🏗️ Architecture

Built on the robust foundation of **Microsoft.Extensions.AI**, Maui.Essentials.AI provides:

- **Unified Abstractions**: One API across all platforms and AI providers
- **Dependency Injection**: Seamlessly integrates with .NET DI container
- **Extensible Design**: Easy to add custom AI providers and models
- **Testing Support**: Mock AI services for unit testing
- **Observability**: Built-in logging, metrics, and tracing
- **On-Device Focus**: All processing happens locally for privacy and offline capability
- **IChatClient Compatibility**: Easy to swap with cloud providers if needed via the standard interface

## 🚀 Quick Start

### Installation

```bash
dotnet add package Maui.Essentials.AI
```

### Basic Setup

```csharp
// MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseAI(); // 🎉 That's it! AI is ready

        return builder.Build();
    }
}
```

### Simple Chat Integration

```csharp
// ChatViewModel.cs
public class ChatViewModel : ObservableObject
{
    private readonly IChatClient _chatClient;

    public ChatViewModel(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    [RelayCommand]
    async Task SendMessageAsync()
    {
        var response = await _chatClient.CompleteAsync(
            [new ChatMessage(ChatRole.User, UserInput)]);
        
        Messages.Add(new ChatMessage(ChatRole.Assistant, response.Message.Text));
    }
}
```

## 💡 Advanced Scenarios

### Custom Model Integration
```csharp
builder.Services.AddAI(options =>
{
    // Prefer on-device models first
    options.PreferOnDevice = true;
    
    // Add custom ONNX model as secondary option
    options.AddCustomModel("my-model.onnx", ModelType.ONNX);
    
    // Add additional custom models for different scenarios
    options.AddCustomModel("sentiment-model.onnx", ModelType.ONNX, useFor: "sentiment");
});
```

### Multi-Modal AI
```csharp
// Image analysis with automatic model selection
var result = await _aiService.AnalyzeImageAsync(imageBytes, 
    "Describe what you see in this image");

// Text-to-speech with device-optimized voices  
await _aiService.SpeakAsync("Hello from Maui.Essentials.AI!");

// Sentiment analysis on user feedback
var sentiment = await _aiService.AnalyzeSentimentAsync(userReview);
```

### Platform-Specific Optimizations
```csharp
#if IOS
// Leverage iOS-specific Foundation models
builder.Services.AddFoundationModels();
#elif ANDROID  
// Use Google AI Edge on Android
builder.Services.AddGoogleAIEdge();
#elif WINDOWS
// Take advantage of Phi Silica on Windows
builder.Services.AddPhiSilica();
#endif
```

## 🎯 Supported Platforms

| Platform | On-Device Models | Custom Models |
|----------|------------------|---------------|
| **iOS** | ✅ Foundation Models, Core ML | ✅ Core ML, ONNX |
| **Android** | ✅ Google AI Edge, TF Lite | ✅ TensorFlow Lite, ONNX |
| **Windows** | ✅ Phi Silica, DirectML | ✅ ONNX Runtime, DirectML |
| **macOS** | ✅ Core ML, Metal | ✅ Core ML, ONNX |

## 📚 Documentation & Resources

- 📖 **[Microsoft.Extensions.AI Documentation](https://learn.microsoft.com/dotnet/ai/microsoft-extensions-ai)**
- 🔧 **[Sample Implementations Guide](https://learn.microsoft.com/dotnet/ai/advanced/sample-implementations)**
- 💻 **[Sample App](./samples/Maui.Essentials.AI.SampleApp)** - Complete chat application example
- 🎥 **[Video Tutorials](#)** - Coming soon!
- 🧪 **[API Reference](#)** - Comprehensive API documentation

## 🌟 What Makes This Special?

Unlike traditional AI integrations that require you to:
- ❌ Choose between different AI providers upfront
- ❌ Handle platform-specific implementations  
- ❌ Manage model downloads and storage
- ❌ Deal with network connectivity issues
- ❌ Write different code for different platforms

**Maui.Essentials.AI lets you:**
- ✅ Write once, run everywhere with optimal on-device AI
- ✅ Automatically use the best available AI for each device
- ✅ Work completely offline with local processing
- ✅ Focus on your app's features, not AI infrastructure
- ✅ Get privacy-first AI with on-device processing
- ✅ Easy to extend with custom models or swap implementations via IChatClient

## 🚧 Development Status

This project is in active development. Current focus areas:

- 🔄 **Core Abstractions** - Building the foundation layer
- 🏗️ **Platform Implementations** - Native model integrations  
- 📱 **Sample Applications** - Comprehensive examples
- 📚 **Documentation** - Guides and tutorials
- 🧪 **Testing Framework** - Comprehensive test coverage

## 🤝 Contributing

We welcome contributions! Whether you're:
- 💡 Suggesting new features
- 🐛 Reporting bugs  
- 📝 Improving documentation
- 🔧 Contributing code
- 🧪 Adding tests

Check out our [contribution guidelines](CONTRIBUTING.md) to get started.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

<div align="center">

**Ready to make AI effortless in your MAUI apps?**

[Get Started](#quick-start) • [View Examples](./samples) • [Join Discussions](https://github.com/mattleibow/Maui.Essentials.AI/discussions)

Made with ❤️ by the .NET MAUI community

</div>
