## Generating Files

To generate the API definitions files:

```
dotnet build src/Microsoft.Extensions.AI.Apple.FoundationModels -f net10.0-ios

sharpie bind \
  --output=src/Microsoft.Extensions.AI.Apple.FoundationModels \
  --namespace=Microsoft.Extensions.AI.Apple.FoundationModels \
  --sdk=iphoneos26.0 \
  --scope=. \
  src/Microsoft.Extensions.AI.Apple.FoundationModels/obj/Debug/net10.0-ios/xcode/MicrosoftExtensionsAIAppleFoundationModels-01323/xcframeworks/MicrosoftExtensionsAIAppleFoundationModelsiOS.xcframework/ios-arm64/MicrosoftExtensionsAIAppleFoundationModels.framework/Headers/MicrosoftExtensionsAIAppleFoundationModels-Swift.h
```