//
//  TranscriptInstructions.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

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
