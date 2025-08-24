//
//  TranscriptResponse.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

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
