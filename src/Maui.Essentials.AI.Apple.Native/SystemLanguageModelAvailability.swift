//
//  SystemLanguageModelAvailability.swift
//  MauiEssentialsAI
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc
public enum SystemLanguageModelAvailability: Int {
    case available
    case unavailableAppleIntelligenceNotEnabled
    case unavailableDeviceNotEligible
    case unavailableModelNotReady
    case unavailable
}
