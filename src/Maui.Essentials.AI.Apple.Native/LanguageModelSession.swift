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

    let actual: LanguageModelSession?

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
        return actual!.isResponding
    }

    @objc
    public func respond(
        to prompt: String,
        onComplete:
            @escaping (LanguageModelSessionStringResponseWrapper?, NSError?) ->
            Void
    ) {
        Task.detached {
            do {
                let response = try await self.actual!.respond(
                    to: prompt,
                    options: GenerationOptions()
                )  // TODO: expose GenerationOptions
                let responseWrapper =
                    LanguageModelSessionStringResponseWrapper.create(
                        from: response
                    )
                onComplete(responseWrapper, nil)
            } catch let error as LanguageModelSession.GenerationError {
                let nserror = NSError(
                    domain: "LanguageModelSessionErrorDomain",
                    code: 0,
                    userInfo: [
                        NSUnderlyingErrorKey:
                            error.errorDescription ?? "",
                        NSLocalizedRecoverySuggestionErrorKey:
                            error.recoverySuggestion ?? "",
                        NSLocalizedFailureReasonErrorKey:
                            error.failureReason ?? "",
                        NSLocalizedDescriptionKey:
                            error.localizedDescription,
                    ]
                )
                onComplete(nil, nserror)
            } catch {
                // TODO: ToolCallError

                let nserror = NSError(
                    domain: "LanguageModelSessionErrorDomain",
                    code: 0,
                    userInfo: [
                        NSLocalizedDescriptionKey:
                            "Failed to respond to prompt: \(error.localizedDescription)"
                    ]
                )
                onComplete(nil, nserror)
            }
        }
    }

}
