//
//  LanguageModelSession.swift
//  MauiEssentialsAI
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(LanguageModelSession)
public class LanguageModelSessionWrapper: NSObject {

    let actual: LanguageModelSession

    @objc
    public override init() {
        actual = LanguageModelSession()
    }

    @objc
    public init(instructions: String?) {
        actual = LanguageModelSession(instructions: instructions)
    }

    @objc
    public init(model: SystemLanguageModelWrapper?, instructions: String?) {
        actual = LanguageModelSession(
            model: model?.actual ?? SystemLanguageModel.default,
            // TODO: guardrails: .default,
            // TODO: tools: [],
            instructions: instructions
        )
    }

    @objc
    public var isResponding: Bool {
        return actual.isResponding
    }

    @objc
    public func respond(
        to prompt: String,
        onComplete:
            @escaping (LanguageModelSessionStringResponseWrapper?, NSError?) ->
            Void
    ) {
        self.respond(to: prompt, options: nil, onComplete: onComplete)
    }

    @objc
    public func respond(
        to prompt: String,
        options: GenerationOptionsWrapper?,
        onComplete:
            @escaping (LanguageModelSessionStringResponseWrapper?, NSError?) ->
            Void
    ) {
        Task.detached {
            do {
                let generationOptions = options?.actual ?? GenerationOptions()
                let response = try await self.actual.respond(
                    to: prompt,
                    options: generationOptions
                )
                onComplete(.create(from: response), nil)
            } catch let error as LanguageModelSession.GenerationError {
                onComplete(nil, error.toNSError())
            } catch let error as LanguageModelSession.ToolCallError {
                onComplete(nil, error.toNSError())
            } catch {
                onComplete(nil, error.toNSError())
            }
        }
    }

    @objc
    public func streamResponse(
        to prompt: String,
        onNext: @escaping (String) -> Void,
        onComplete:
            @escaping (LanguageModelSessionStringResponseWrapper?, NSError?) ->
            Void
    ) {
        self.respond(to: prompt, options: nil, onComplete: onComplete)
    }

    @objc
    public func streamResponse(
        to prompt: String,
        options: GenerationOptionsWrapper?,
        onNext: @escaping (String) -> Void,
        onComplete:
            @escaping (LanguageModelSessionStringResponseWrapper?, NSError?) ->
            Void
    ) {
        Task.detached {
            do {
                let generationOptions = options?.actual ?? GenerationOptions()
                let stream = self.actual.streamResponse(
                    to: prompt,
                    options: generationOptions
                )
                for try await partial in stream {
                    onNext(partial)
                }
                let response = try await stream.collect()
                onComplete(.create(from: response), nil)
            } catch let error as LanguageModelSession.GenerationError {
                onComplete(nil, error.toNSError())
            } catch let error as LanguageModelSession.ToolCallError {
                onComplete(nil, error.toNSError())
            } catch {
                onComplete(nil, error.toNSError())
            }
        }
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(Transcript)
public class TranscriptWrapper: NSObject {

    let actual: Transcript

    @objc
    public init(entries: [TranscriptEntryWrapper]) {
        actual = Transcript(entries: entries.map { $0.actual })
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptEntry)
public class TranscriptEntryWrapper: NSObject {

    let actual: Transcript.Entry

    @objc
    public init(instructions: TranscriptInstructionsWrapper) {
        actual = .instructions(instructions.actual)
    }

    @objc
    public init(prompt: TranscriptPromptWrapper) {
        actual = .prompt(prompt.actual)
    }

    @objc
    public init(response: TranscriptResponseWrapper) {
        actual = .response(response.actual)
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptInstructions)
public class TranscriptInstructionsWrapper: NSObject {

    let actual: Transcript.Instructions

    @objc
    public var id: String { actual.id }

    @objc
    public init(id: String, segments: [TranscriptSegmentWrapper]) {
        actual = .init(
            id: id,
            segments: segments.map { $0.actual },
            toolDefinitions: []  // TODO: expose tools
        )
    }

    @objc
    public init(segments: [TranscriptSegmentWrapper]) {
        actual = .init(
            segments: segments.map { $0.actual },
            toolDefinitions: []  // TODO: expose tools
        )
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptPrompt)
public class TranscriptPromptWrapper: NSObject {

    let actual: Transcript.Prompt

    @objc
    public var id: String { actual.id }

    let options: GenerationOptionsWrapper

    @objc
    public init(
        id: String,
        segments: [TranscriptSegmentWrapper],
        options: GenerationOptionsWrapper? = nil
            // TODO: responseFormat
    ) {
        self.options = options ?? GenerationOptionsWrapper()

        actual = .init(
            id: id,
            segments: segments.map { $0.actual },
            options: self.options.actual,
            responseFormat: nil
        )
    }

    @objc
    public init(segments: [TranscriptSegmentWrapper]) {
        options = GenerationOptionsWrapper()
        actual = .init(
            segments: segments.map { $0.actual },
            options: options.actual
        )
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptResponse)
public class TranscriptResponseWrapper: NSObject {

    let actual: Transcript.Response

    @objc
    public var id: String { actual.id }

    @objc
    public var assetIDs: [String] { actual.assetIDs }

    @objc
    public init(
        id: String,
        assetIDs: [String],
        segments: [TranscriptSegmentWrapper]
    ) {
        actual = .init(
            id: id,
            assetIDs: assetIDs,
            segments: segments.map { $0.actual }
        )
    }

    @objc
    public init(segments: [TranscriptSegmentWrapper]) {
        actual = .init(
            assetIDs: [],
            segments: segments.map { $0.actual }
        )
    }
}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptSegment)
public class TranscriptSegmentWrapper: NSObject {

    let actual: Transcript.Segment

    @objc
    public init(segments: [TranscriptSegmentWrapper]) {
        super.init()
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
class TEST2 {
    func TEST() async {
        let wrapper = TranscriptWrapper(entries: [
            .init(instructions: TranscriptInstructionsWrapper(segments: [])),
            .init(prompt: TranscriptPromptWrapper(segments: [])),
            .init(response: TranscriptResponseWrapper(segments: [])),
        ])

        let resp = try! await LanguageModelSession(transcript: wrapper.actual)
            .respond(to: "Hello")
        print(resp.content)
    }
}

//class TEST {
//
//    func convertMessagesToTranscript(_ messages: [ChatMessage]) -> [Transcript
//        .Entry]
//    {
//        var entries: [Transcript.Entry] = []
//
//        // Process all messages in order
//        for message in messages {
//            let textSegment = Transcript.TextSegment(content: message.content)
//
//            switch message.role.lowercased() {
//            case "system":
//                // Convert system messages to instructions
//                let instructions = Transcript.Instructions(
//                    segments: [.text(textSegment)],
//                    toolDefinitions: []
//                )
//                entries.append(.instructions(instructions))
//
//            case "user":
//                // Convert user messages to prompts
//                let prompt = Transcript.Prompt(
//                    segments: [.text(textSegment)]
//                )
//                entries.append(.prompt(prompt))
//
//            case "assistant":
//                // Convert assistant messages to responses
//                let response = Transcript.Response(
//                    assetIDs: [],
//                    segments: [.text(textSegment)]
//                )
//                entries.append(.response(response))
//
//            default:
//                // Treat unknown roles as user messages
//                let prompt = Transcript.Prompt(
//                    segments: [.text(textSegment)]
//                )
//                entries.append(.prompt(prompt))
//            }
//        }
//
//        return entries
//    }
//}
