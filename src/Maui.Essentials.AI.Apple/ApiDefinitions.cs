using System;
using Foundation;
using ObjCRuntime;

namespace Maui.Essentials.AI
{
	// @interface GenerationOptionsSamplingMode
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface GenerationOptionsSamplingMode
	{
		// +(GenerationOptionsSamplingMode * _Nonnull)greedy __attribute__((warn_unused_result("")));
		[Static]
		[Export ("greedy")]
		GenerationOptionsSamplingMode Greedy ();

		// +(GenerationOptionsSamplingMode * _Nonnull)randomWithTop:(id)k seed:(id)seed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("randomWithTop:seed:")]
		GenerationOptionsSamplingMode Random (int topK, [NullAllowed] ulong seed);

		// +(GenerationOptionsSamplingMode * _Nonnull)randomTopKWithTop:(id)k __attribute__((warn_unused_result("")));
		[Static]
		[Export ("randomTopKWithTop:")]
		GenerationOptionsSamplingMode Random (int topK);

		// +(GenerationOptionsSamplingMode * _Nonnull)randomWithProbability:(double)threshold seed:(id)seed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("randomWithProbability:seed:")]
		GenerationOptionsSamplingMode Random (double probabilityThreshold, [NullAllowed] ulong seed);

		// +(GenerationOptionsSamplingMode * _Nonnull)randomWithProbability:(double)threshold __attribute__((warn_unused_result("")));
		[Static]
		[Export ("randomWithProbability:")]
		GenerationOptionsSamplingMode Random (double probabilityThreshold);
	}

	// @interface GenerationOptions
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	interface GenerationOptions
	{
		// -(instancetype _Nonnull)initWithSampling:(GenerationOptionsSamplingMode * _Nullable)sampling temperature:(NSNumber * _Nullable)temperature maximumResponseTokens:(NSNumber * _Nullable)maximumResponseTokens __attribute__((objc_designated_initializer));
		[Export ("initWithSampling:temperature:maximumResponseTokens:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] GenerationOptionsSamplingMode sampling, [NullAllowed] NSNumber temperature, [NullAllowed] NSNumber maximumResponseTokens);

		// @property (readonly, nonatomic, strong) GenerationOptionsSamplingMode * _Nullable sampling;
		[NullAllowed, Export ("sampling", ArgumentSemantic.Strong)]
		GenerationOptionsSamplingMode Sampling { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable temperature;
		[NullAllowed, Export ("temperature", ArgumentSemantic.Strong)]
		NSNumber Temperature { get; }

		// @property (readonly, nonatomic, strong) NSNumber * _Nullable maximumResponseTokens;
		[NullAllowed, Export ("maximumResponseTokens", ArgumentSemantic.Strong)]
		NSNumber MaximumResponseTokens { get; }
	}

	// @interface LanguageModelSessionStringResponse
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface LanguageModelSessionStringResponse
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull content;
		[Export ("content")]
		string Content { get; }
	}

	// @interface LanguageModelSession
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
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
		bool IsResponding { get; }

		// -(void)respondTo:(NSString * _Nonnull)prompt onComplete:(void (^ _Nonnull)(LanguageModelSessionStringResponse * _Nullable, NSError * _Nullable))onComplete;
		[Async]
		[Export ("respondTo:onComplete:")]
		void Respond (string prompt, Action<LanguageModelSessionStringResponse, NSError> onComplete);

		// -(void)respondTo:(NSString * _Nonnull)prompt options:(GenerationOptions * _Nullable)options onComplete:(void (^ _Nonnull)(LanguageModelSessionStringResponse * _Nullable, NSError * _Nullable))onComplete;
		[Async]
		[Export ("respondTo:options:onComplete:")]
		void Respond (string prompt, [NullAllowed] GenerationOptions options, Action<LanguageModelSessionStringResponse, NSError> onComplete);

		// -(void)streamResponseTo:(NSString * _Nonnull)prompt onNext:(void (^ _Nonnull)(NSString * _Nonnull))onNext onComplete:(void (^ _Nonnull)(LanguageModelSessionStringResponse * _Nullable, NSError * _Nullable))onComplete;
		[Export ("streamResponseTo:onNext:onComplete:")]
		void StreamResponse (string prompt, Action<string> onNext, Action<LanguageModelSessionStringResponse, NSError> onComplete);

		// -(void)streamResponseTo:(NSString * _Nonnull)prompt options:(GenerationOptions * _Nullable)options onNext:(void (^ _Nonnull)(NSString * _Nonnull))onNext onComplete:(void (^ _Nonnull)(LanguageModelSessionStringResponse * _Nullable, NSError * _Nullable))onComplete;
		[Export ("streamResponseTo:options:onNext:onComplete:")]
		void StreamResponse (string prompt, [NullAllowed] GenerationOptions options, Action<string> onNext, Action<LanguageModelSessionStringResponse, NSError> onComplete);
	}

	// @interface SystemLanguageModel
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface SystemLanguageModel
	{
		// @property (readonly, nonatomic, strong, class) SystemLanguageModel * _Nonnull shared;
		[Static]
		[Export ("shared", ArgumentSemantic.Strong)]
		SystemLanguageModel Shared { get; }

		// @property (readonly, nonatomic) int isAvailable;
		[Export ("isAvailable")]
		bool IsAvailable { get; }

		// @property (readonly, nonatomic) enum SystemLanguageModelAvailability availability;
		[Export ("availability")]
		SystemLanguageModelAvailability Availability { get; }
	}
}
