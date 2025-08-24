//
//  Transcript.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(Transcript)
public class TranscriptWrapper: NSObject {

    let actual: Transcript

    @objc
    public init(entries: [TranscriptEntryWrapper]) {
        actual = Transcript(entries: entries.map { $0.actual })
    }

}
