## Generating Files

To generate the API definitions files:

```
sharpie xcode -sdks
```

```
dotnet build src/Maui.Essentials.AI.Apple -f net10.0-ios -bl

sharpie bind \
  --output=src/Maui.Essentials.AI.Apple \
  --namespace=Maui.Essentials.AI \
  --sdk=iphoneos26.0 \
  --scope=. \
  src/Maui.Essentials.AI.Apple/obj/Debug/net10.0-ios/xcode/MauiEssentialsAI-d288f/xcframeworks/MauiEssentialsAIiOS.xcframework/ios-arm64/MauiEssentialsAI.framework/Headers/MauiEssentialsAI-Swift.h
```