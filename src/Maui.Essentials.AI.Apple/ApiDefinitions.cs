using System;
using Foundation;
using ObjCRuntime;

namespace Maui.Essentials.AI;

// @interface LanguageModelSessionStringResponse
[Introduced(PlatformName.iOS, 26, 0)]
[Introduced(PlatformName.MacCatalyst, 26, 0)]
[Introduced(PlatformName.MacOSX, 26, 0)]
[DisableDefaultCtor]
[BaseType(typeof(NSObject))]
interface LanguageModelSessionStringResponse
{
	// @property (readonly, copy, nonatomic) NSString * _Nonnull content;
	[Export("content")]
	string Content { get; }
}

// @interface LanguageModelSession
[Introduced(PlatformName.iOS, 26, 0)]
[Introduced(PlatformName.MacCatalyst, 26, 0)]
[Introduced(PlatformName.MacOSX, 26, 0)]
[BaseType(typeof(NSObject))]
interface LanguageModelSession
{
	// -(instancetype _Nonnull)initWithInstructions:(NSString * _Nullable)instructions __attribute__((objc_designated_initializer));
	[Export("initWithInstructions:")]
	[DesignatedInitializer]
	NativeHandle Constructor([NullAllowed] string instructions);

	// -(instancetype _Nonnull)initWithModel:(SystemLanguageModel * _Nullable)model instructions:(NSString * _Nullable)instructions __attribute__((objc_designated_initializer));
	[Export("initWithModel:instructions:")]
	[DesignatedInitializer]
	NativeHandle Constructor([NullAllowed] SystemLanguageModel model, [NullAllowed] string instructions);

	// @property (readonly, nonatomic) int isResponding;
	[Export("isResponding")]
	bool IsResponding { get; }

	// -(void)respondTo:(NSString * _Nonnull)prompt onComplete:(void (^ _Nonnull)(LanguageModelSessionStringResponse * _Nullable, NSError * _Nullable))onComplete;
	[Async]
	[Export("respondTo:onComplete:")]
	void RespondTo(string prompt, Action<LanguageModelSessionStringResponse, NSError> onComplete);
}

// @interface SystemLanguageModel
[Introduced(PlatformName.iOS, 26, 0)]
[Introduced(PlatformName.MacCatalyst, 26, 0)]
[Introduced(PlatformName.MacOSX, 26, 0)]
[DisableDefaultCtor]
[BaseType(typeof(NSObject))]
interface SystemLanguageModel
{
	// @property (readonly, nonatomic, strong, class) SystemLanguageModel * _Nonnull shared;
	[Static]
	[Export("shared", ArgumentSemantic.Strong)]
	SystemLanguageModel Shared { get; }

	// @property (readonly, nonatomic) int isAvailable;
	[Export("isAvailable")]
	bool IsAvailable { get; }

	// @property (readonly, nonatomic) enum SystemLanguageModelAvailability availability;
	[Export("availability")]
	SystemLanguageModelAvailability Availability { get; }
}
