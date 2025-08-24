//
//  TEST2.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/24.
//


import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
class TEST2 {
    func TEST() async {
        let wrapper = TranscriptWrapper(entries: [
            .init(instructions: TranscriptInstructionsWrapper(segments: [
                .init(text: .init(content: "You are a asuper agent!"))
            ])),
            .init(prompt: TranscriptPromptWrapper(segments: [
                .init(text: .init(content: "Hello, can you help?"))
            ])),
            .init(response: TranscriptResponseWrapper(segments: [
                .init(text: .init(content: "How can I help?"))
            ])),
        ])

        let resp = try! await LanguageModelSession(transcript: wrapper.actual)
            .respond(to: "Please fix my code!")
        
        print(resp.content)
    }
}


//class TEST {
//
//    func convertMessagesToTranscript(_ messages: [ChatMessage]) -> [Transcript
//        .Entry]
//    {
//        var entries: [Transcript.Entry] = []
//
//        // Process all messages in order
//        for message in messages {
//            let textSegment = Transcript.TextSegment(content: message.content)
//
//            switch message.role.lowercased() {
//            case "system":
//                // Convert system messages to instructions
//                let instructions = Transcript.Instructions(
//                    segments: [.text(textSegment)],
//                    toolDefinitions: []
//                )
//                entries.append(.instructions(instructions))
//
//            case "user":
//                // Convert user messages to prompts
//                let prompt = Transcript.Prompt(
//                    segments: [.text(textSegment)]
//                )
//                entries.append(.prompt(prompt))
//
//            case "assistant":
//                // Convert assistant messages to responses
//                let response = Transcript.Response(
//                    assetIDs: [],
//                    segments: [.text(textSegment)]
//                )
//                entries.append(.response(response))
//
//            default:
//                // Treat unknown roles as user messages
//                let prompt = Transcript.Prompt(
//                    segments: [.text(textSegment)]
//                )
//                entries.append(.prompt(prompt))
//            }
//        }
//
//        return entries
//    }
//}
