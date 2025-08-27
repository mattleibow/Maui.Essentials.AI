//
//  LanguageModelSession.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, macCatalyst 26.0, visionOS 26.0, *)
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
    public init(transcript: TranscriptWrapper) {
        actual = LanguageModelSession(transcript: transcript.actual)
    }

    @objc
    public init(
        model: SystemLanguageModelWrapper?,
        tools: [ToolWrapper]?,
        instructions: String?
    ) {
        actual = LanguageModelSession(
            model: model?.actual ?? SystemLanguageModel.default,
            tools: tools?.map { ToolProxy(tool: $0) } ?? [],
            instructions: instructions
        )
    }

    @objc
    public init(
        model: SystemLanguageModelWrapper?,
        tools: [ToolWrapper]?,
        transcript: TranscriptWrapper
    ) {
        actual = LanguageModelSession(
            model: model?.actual ?? SystemLanguageModel.default,
            tools: tools?.map { ToolProxy(tool: $0) } ?? [],
            transcript: transcript.actual
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
                    onNext(partial.content)
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
