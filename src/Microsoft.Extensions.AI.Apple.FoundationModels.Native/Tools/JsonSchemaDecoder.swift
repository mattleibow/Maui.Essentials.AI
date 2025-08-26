//
//  JsonSchemaDecoder.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/26.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, macCatalyst 26.0, visionOS 26.0, *)
class JsonSchemaDecoder {

    /// Simple JSON Schema representation
    private class JsonSchema: Codable {
        var type: String
        var title: String?
        var description: String?
        var properties: [String: JsonSchema]?
        var required: [String]?
        var items: JsonSchema?

        enum CodingKeys: String, CodingKey {
            case type, title, properties, required, items
        }
    }

    /// Some common JSON Schemas
    public static let StringGenerationSchema: GenerationSchema =
        try! GenerationSchema(
            root: DynamicGenerationSchema(type: String.self),
            dependencies: []
        )

    /// Parse a string into a GenerationSchema
    public static func parseGenerationSchema(_ jsonString: String)
        -> GenerationSchema?
    {
        // Parse JSON Schema
        guard
            let data = jsonString.data(using: .utf8),
            let jsonSchema = try? JSONDecoder().decode(
                JsonSchema.self,
                from: data
            ),
            let dynamicSchema = parseDynamicJsonSchema(jsonSchema)
        else {
            return nil
        }

        // Convert to GenerationSchema
        return try? GenerationSchema(root: dynamicSchema, dependencies: [])
    }

    /// Convert the object representation of a JSON schema into a DynamicGenerationSchema
    private static func parseDynamicJsonSchema(_ schema: JsonSchema)
        -> DynamicGenerationSchema?
    {
        // Handle object type
        if schema.type == "object", let properties = schema.properties {
            let props = properties.compactMap {
                (name, value) -> DynamicGenerationSchema.Property? in
                return parseJsonProperty(name, value, schema)
            }
            return DynamicGenerationSchema(
                name: schema.title ?? "Object",
                description: schema.description,
                properties: props
            )
        }
        // Handle array type
        else if schema.type == "array", let items = schema.items {
            if let itemSchema = parseDynamicJsonSchema(items) {
                return DynamicGenerationSchema(arrayOf: itemSchema)
            }
        }
        // Handle primitive types
        else {
            switch schema.type {
            case "string": return DynamicGenerationSchema(type: String.self)
            case "integer": return DynamicGenerationSchema(type: Int.self)
            case "number": return DynamicGenerationSchema(type: Double.self)
            case "boolean": return DynamicGenerationSchema(type: Bool.self)
            default: return nil
            }
        }
        return nil
    }

    private static func parseJsonProperty(
        _ propertyName: String,
        _ value: JsonSchema,
        _ parentSchema: JsonSchema
    ) -> DynamicGenerationSchema.Property? {
        if let nestedSchema = parseDynamicJsonSchema(value) {
            let isRequired = parentSchema.required?.contains(propertyName)
            return DynamicGenerationSchema.Property(
                name: propertyName,
                description: value.description,
                schema: nestedSchema,
                isOptional: isRequired != true
            )
        } else {
            return nil
        }
    }

}
