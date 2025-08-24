using System;
using Foundation;
using ObjCRuntime;

namespace Microsoft.Extensions.AI.Apple.FoundationModels
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

		// +(GenerationOptionsSamplingMode * _Nonnull)randomWithTop:(id)k seed:(NSNumber * _Nullable)seed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("randomWithTop:seed:")]
		GenerationOptionsSamplingMode Random (int topK, [NullAllowed] NSNumber seed);

		// +(GenerationOptionsSamplingMode * _Nonnull)randomWithProbability:(double)threshold seed:(NSNumber * _Nullable)seed __attribute__((warn_unused_result("")));
		[Static]
		[Export ("randomWithProbability:seed:")]
		GenerationOptionsSamplingMode RandomWithProbability (double threshold, [NullAllowed] NSNumber seed);
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

		// -(instancetype _Nonnull)initWithModel:(SystemLanguageModel * _Nullable)model transcript:(Transcript * _Nonnull)transcript __attribute__((objc_designated_initializer));
		[Export ("initWithModel:transcript:")]
		[DesignatedInitializer]
		NativeHandle Constructor ([NullAllowed] SystemLanguageModel model, Transcript transcript);

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

	// @interface TranscriptEntry
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TranscriptEntry
	{
		// -(instancetype _Nonnull)initWithInstructions:(TranscriptInstructions * _Nonnull)instructions __attribute__((objc_designated_initializer));
		[Export ("initWithInstructions:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptInstructions instructions);

		// -(instancetype _Nonnull)initWithPrompt:(TranscriptPrompt * _Nonnull)prompt __attribute__((objc_designated_initializer));
		[Export ("initWithPrompt:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptPrompt prompt);

		// -(instancetype _Nonnull)initWithResponse:(TranscriptResponse * _Nonnull)response __attribute__((objc_designated_initializer));
		[Export ("initWithResponse:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptResponse response);
	}

	// @interface TranscriptInstructions
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TranscriptInstructions
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nonnull)id segments:(id)segments __attribute__((objc_designated_initializer));
		[Export ("initWithId:segments:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string id, TranscriptSegment[] segments);

		// -(instancetype _Nonnull)initWithSegments:(id)segments __attribute__((objc_designated_initializer));
		[Export ("initWithSegments:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptSegment[] segments);
	}

	// @interface TranscriptPrompt
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TranscriptPrompt
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nonnull)id segments:(id)segments options:(GenerationOptions * _Nullable)options __attribute__((objc_designated_initializer));
		[Export ("initWithId:segments:options:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string id, TranscriptSegment[] segments, [NullAllowed] GenerationOptions options);

		// -(instancetype _Nonnull)initWithSegments:(id)segments __attribute__((objc_designated_initializer));
		[Export ("initWithSegments:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptSegment[] segments);
	}

	// @interface TranscriptResponse
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TranscriptResponse
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nonnull)id assetIDs:(id)assetIDs segments:(id)segments __attribute__((objc_designated_initializer));
		[Export ("initWithId:assetIDs:segments:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string id, string[] assetIDs, TranscriptSegment[] segments);

		// -(instancetype _Nonnull)initWithSegments:(id)segments __attribute__((objc_designated_initializer));
		[Export ("initWithSegments:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptSegment[] segments);
	}

	// @interface TranscriptSegment
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TranscriptSegment
	{
		// -(instancetype _Nonnull)initWithText:(TranscriptTextSegment * _Nonnull)text __attribute__((objc_designated_initializer));
		[Export ("initWithText:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptTextSegment text);
	}

	// @interface TranscriptTextSegment
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface TranscriptTextSegment
	{
		// @property (readonly, copy, nonatomic) NSString * _Nonnull id;
		[Export ("id")]
		string Id { get; }

		// @property (readonly, copy, nonatomic) NSString * _Nonnull content;
		[Export ("content")]
		string Content { get; }

		// -(instancetype _Nonnull)initWithId:(NSString * _Nonnull)id content:(NSString * _Nonnull)content __attribute__((objc_designated_initializer));
		[Export ("initWithId:content:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string id, string content);

		// -(instancetype _Nonnull)initWithContent:(NSString * _Nonnull)content __attribute__((objc_designated_initializer));
		[Export ("initWithContent:")]
		[DesignatedInitializer]
		NativeHandle Constructor (string content);
	}

	// @interface Transcript
	[Introduced (PlatformName.iOS, 26, 0)]
	[Introduced (PlatformName.MacCatalyst, 26, 0)]
	[Introduced (PlatformName.MacOSX, 26, 0)]
	[BaseType (typeof(NSObject))]
	[DisableDefaultCtor]
	interface Transcript
	{
		// -(instancetype _Nonnull)initWithEntries:(id)entries __attribute__((objc_designated_initializer));
		[Export ("initWithEntries:")]
		[DesignatedInitializer]
		NativeHandle Constructor (TranscriptEntry[] entries);
	}
}
