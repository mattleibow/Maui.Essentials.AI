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
    let name: String
    let description: String
    let parameters: GenerationSchema
    let output: GenerationSchema

    init(tool: ToolWrapper) {
        actual = tool
        name = actual.name
        description = actual.description
        parameters =
            (try? JsonSchemaDecoder.parse(actual.argumentsSchema))
            ?? JsonSchemaDecoder.StringGenerationSchema
        output =
            (try? JsonSchemaDecoder.parse(actual.outputSchema))
            ?? JsonSchemaDecoder.StringGenerationSchema
    }

    func call(arguments: Arguments) async throws -> Output {
        let result = await actual.call(arguments: arguments.json)
        return try .init(json: result, schema: output)
    }

    struct Arguments: ConvertibleFromGeneratedContent {

        let id: GenerationID?
        let json: String
        let generatedContent: GeneratedContent

        init(_ content: GeneratedContent) throws {
            id = content.id
            json = content.jsonString
            generatedContent = content.generatedContent
        }

    }

    struct Output: ConvertibleToGeneratedContent {

        let promptRepresentation: Prompt
        let generatedContent: GeneratedContent

        init(json: String, schema: GenerationSchema) throws {
            promptRepresentation = .init({
                json
            })
            do {
                generatedContent = try .init(json: json)
            } catch {
                generatedContent = .init(json)
            }
        }

    }
}
