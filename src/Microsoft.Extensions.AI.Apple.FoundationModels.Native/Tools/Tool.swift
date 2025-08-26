//
//  Tool.swift
//  MicrosoftExtensionsAIAppleFoundationModels
//
//  Created by Matthew Leibowitz on 2025/08/26.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, macCatalyst 26.0, visionOS 26.0, *)
@objc(Tool)
public protocol ToolWrapper: Sendable {
    var name: String { get }
    var description: String { get }
    var argumentsSchema: String { get }
    var outputSchema: String { get }
    func call(arguments: String) async -> String
}
