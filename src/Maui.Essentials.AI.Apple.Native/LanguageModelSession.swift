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
        super.init()
    }

    @objc
    public init(instructions: String?) {
        actual = LanguageModelSession(instructions: instructions)
        super.init()
    }

    @objc
    public init(model: SystemLanguageModelWrapper?, instructions: String?) {
        actual = LanguageModelSession(
            model: model?.actual ?? SystemLanguageModel.default,
            // TODO: guardrails: .default,
            // TODO: tools: [],
            instructions: instructions
        )
        super.init()
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
