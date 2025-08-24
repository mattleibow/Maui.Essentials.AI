//
//  TranscriptTextSegment.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptTextSegment)
public class TranscriptTextSegmentWrapper: NSObject {

    let actual: Transcript.TextSegment

    @objc
    public var id: String { actual.id }

    @objc
    public var content: String { actual.content }

    @objc
    public init(id: String, content: String) {
        actual = .init(id: id, content: content)
    }

    @objc
    public init(content: String) {
        actual = .init(content: content)
    }
}
