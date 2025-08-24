//
//  TranscriptPrompt.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

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
