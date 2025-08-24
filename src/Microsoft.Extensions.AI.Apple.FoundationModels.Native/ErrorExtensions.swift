//
//  ErrorExtensions.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/22.
//


import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
extension LanguageModelSession.GenerationError {

    public func toNSError()
        -> NSError
    {
        return NSError(
            domain: "LanguageModelSessionGenerationErrorErrorDomain",
            code: 0,
            userInfo: [
                NSUnderlyingErrorKey: self.errorDescription ?? "",
                NSLocalizedRecoverySuggestionErrorKey: self.recoverySuggestion
                    ?? "",
                NSLocalizedFailureReasonErrorKey: self.failureReason ?? "",
                NSLocalizedDescriptionKey: self.localizedDescription,
            ]
        )
    }

}

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
extension LanguageModelSession.ToolCallError {

    public func toNSError()
        -> NSError
    {
        return NSError(
            domain: "LanguageModelSessionToolCallErrorErrorDomain",
            code: 0,
            userInfo: [
                NSUnderlyingErrorKey: self.errorDescription ?? "",
                NSLocalizedRecoverySuggestionErrorKey: self.recoverySuggestion
                    ?? "",
                NSLocalizedFailureReasonErrorKey: self.failureReason ?? "",
                NSLocalizedDescriptionKey: self.localizedDescription,
            ]
        )
    }

}

extension LocalizedError {

    public func toNSError()
        -> NSError
    {
        return NSError(
            domain: "UnknownErrorDomain",
            code: 0,
            userInfo: [
                NSUnderlyingErrorKey: self.errorDescription ?? "",
                NSLocalizedRecoverySuggestionErrorKey: self.recoverySuggestion
                    ?? "",
                NSLocalizedFailureReasonErrorKey: self.failureReason ?? "",
                NSLocalizedDescriptionKey: self.localizedDescription,
            ]
        )
    }
}

extension Error {

    public func toNSError()
        -> NSError
    {
        return NSError(
            domain: "UnknownErrorDomain",
            code: 0,
            userInfo: [
                NSLocalizedDescriptionKey: self.localizedDescription,
            ]
        )
    }
}
