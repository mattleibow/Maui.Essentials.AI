//
//  LanguageModelSessionStringResponse.swift
//  MauiEssentialsAI
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(LanguageModelSessionStringResponse)
public class LanguageModelSessionStringResponseWrapper: NSObject {

    @objc
    public let content: String

    // TODO: public let transcriptEntries: ArraySlice<Transcript.Entry>

    private override init() {
        // Do not use
        content = ""
        super.init()
    }

    private init(content: String) {
        self.content = content
        super.init()
    }

    public static func create(
        from response: LanguageModelSession.Response<String>
    ) -> LanguageModelSessionStringResponseWrapper {
        return LanguageModelSessionStringResponseWrapper(
            content: response.content
        )
    }
    
}
