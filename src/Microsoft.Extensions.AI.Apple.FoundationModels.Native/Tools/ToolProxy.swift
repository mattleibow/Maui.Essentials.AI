//
//  ToolProxy.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/26.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, macCatalyst 26.0, visionOS 26.0, *)
struct ToolProxy: Tool {

    let actual: any ToolWrapper

    public var name: String { actual.toolName }

    public var description: String { actual.toolDescription }

    public let parameters: GenerationSchema

    public init(tool: ToolWrapper) {
        actual = tool
        parameters =
            JsonSchemaDecoder.parseGenerationSchema(actual.toolArgumentsSchema)
            ?? JsonSchemaDecoder.StringGenerationSchema
    }

    public func call(arguments: Arguments) async throws -> Output {
        let result = await actual.call(json: arguments.json)
        return .init(json: result)
    }

    public class Output: PromptRepresentable {

        public var promptRepresentation: Prompt

        public init(json: String) {
            promptRepresentation = .init({
                json
            })
        }

    }

    public class Arguments: ConvertibleFromGeneratedContent {

        let id: GenerationID?
        let json: String
        let generatedContent: GeneratedContent

        public required init(_ content: GeneratedContent) throws {
            id = content.id
            json = content.jsonString
            generatedContent = content.generatedContent
        }

    }
}
