using System;
using Foundation;
using MauiEssentialsAI;
using ObjCRuntime;

namespace Maui.Essentials.AI
{
	// @interface LanguageModelSessionStringResponse
	[NoWatch, NoTV, Mac (26,0), iOS (26,0)]
	[DisableDefaultCtor]
	interface LanguageModelSessionStringResponse
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull content;
		[Export ("content")]
		string Content { get; }
	}

	// @interface LanguageModelSession
	[NoWatch, NoTV, Mac (26,0), iOS (26,0)]
	interface LanguageModelSession
	{
		// -(instancetype _Nonnull)initWithInstructions:(NSString * _Nullable)instructions __attribute__((objc_designated_initializer));
		[Export ("initWithInstructions:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] string instructions);

		// -(instancetype _Nonnull)initWithModel:(SystemLanguageModel * _Nullable)model instructions:(NSString * _Nullable)instructions __attribute__((objc_designated_initializer));
		[Export ("initWithModel:instructions:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] SystemLanguageModel model, [NullAllowed] string instructions);

		// @property (readonly, nonatomic) int isResponding;
		[Export ("isResponding")]
		int IsResponding { get; }

		// -(void)respondTo:(NSString * _Nonnull)prompt onComplete:(void (^ _Nonnull)(LanguageModelSessionStringResponse * _Nullable, NSError * _Nullable))onComplete;
		[Export ("respondTo:onComplete:")]
		void RespondTo (string prompt, Action<LanguageModelSessionStringResponse, NSError> onComplete);
	}

	// @interface SystemLanguageModel
	[NoWatch, NoTV, Mac (26,0), iOS (26,0)]
	[DisableDefaultCtor]
	interface SystemLanguageModel
	{
		// @property (readonly, nonatomic, strong, class) SystemLanguageModel * _Nonnull shared;
		[Static]
		[Export ("shared", ArgumentSemantic.Strong)]
		SystemLanguageModel Shared { get; }

		// @property (readonly, nonatomic) int isAvailable;
		[Export ("isAvailable")]
		int IsAvailable { get; }

		// @property (readonly, nonatomic) enum SystemLanguageModelAvailability availability;
		[Export ("availability")]
		SystemLanguageModelAvailability Availability { get; }
	}
}
