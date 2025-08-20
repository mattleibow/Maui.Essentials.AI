# Building `maui.essentials.ai.aar`

The `@(AndroidGradleProject)` element declared in `src/Core/src/Core.csproj`
will create and pack `maui.essentials.ai.aar` when building that project.

## Glide
NOTE: The binding nuget package version for glide specified in `eng/Version.props`
must be kept in sync with the maven artifact specified in this project
in the `maui.essentials.ai/build.gradle`!

## Building

To build the maui.aar from the command line, there's no need to install Android Studio or Eclipse. It works on Windows and macOS. To build a new .aar just navigate to the AndroidNative folder and run:
- Windows PowerShell: .\gradlew maui:assembleRelease --rerun-tasks
- macOS Terminal: ./gradlew maui:assembleRelease --rerun-tasks

The resulting maui.aar will be output to src/Core/AndroidNative/maui/build/outputs/aar/maui.aar.

Before the first time you run it, you'll need to create a local.properties file in the AndroidNative folder so Gradle can find your Android SDK. It needs one line to set the sdk.dir property. For example:
- Windows: sdk.dir=C\:\\Users\\cfinley\\AppData\\Local\\Android\\Sdk
- macOS: sdk.dir=/Users/cfinley/Library/Developer/Xamarin/android-sdk-macosx

It may download a bunch of stuff the first time it runs; after that it'll be much faster.

If you don't already have Gradle installed:
- Windows: choco install gradle
- macOS: brew install gradle
