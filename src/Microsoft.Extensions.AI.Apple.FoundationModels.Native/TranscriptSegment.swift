//
//  TranscriptSegment.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(TranscriptSegment)
public class TranscriptSegmentWrapper: NSObject {

    let actual: Transcript.Segment

    @objc
    public init(text: TranscriptTextSegmentWrapper) {
        actual = .text(text.actual)
    }

}
