//
//  TranscriptEntry.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, macCatalyst 26.0, visionOS 26.0, *)
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
