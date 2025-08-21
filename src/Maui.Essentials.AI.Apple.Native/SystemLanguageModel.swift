//
//  SystemLanguageModel.swift
//  MauiEssentialsAI
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(SystemLanguageModel)
public class SystemLanguageModelWrapper: NSObject {

    let actual: SystemLanguageModel

    @objc
    public static let shared = SystemLanguageModelWrapper()

    private override init() {
        actual = SystemLanguageModel.default
        super.init()
    }

    @objc
    public var isAvailable: Bool {
        return actual.isAvailable
    }

    @objc
    public var availability: SystemLanguageModelAvailability {
        switch actual.availability {
        case .available: return .available
        case .unavailable(let unavailableReason):
            switch unavailableReason {
            case .appleIntelligenceNotEnabled:
                return .unavailableAppleIntelligenceNotEnabled
            case .deviceNotEligible: return .unavailableDeviceNotEligible
            case .modelNotReady: return .unavailableModelNotReady
            @unknown default: return .unavailable
            }
        }
    }

}
