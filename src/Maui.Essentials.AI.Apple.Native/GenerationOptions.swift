//
//  GenerationOptions.swift
//  MauiEssentialsAI
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(GenerationOptions)
public class GenerationOptionsWrapper: NSObject {

    let actual: GenerationOptions

    @objc
    public override init() {
        actual = GenerationOptions()
        super.init()
    }

    @objc
    public init(
        sampling: GenerationOptionsSamplingModeWrapper?,
        temperature: NSNumber?,
        maximumResponseTokens: NSNumber?
    ) {
        actual = GenerationOptions(
            sampling: sampling?.actual,
            temperature: temperature?.doubleValue,
            maximumResponseTokens: maximumResponseTokens?.intValue
        )
        super.init()
    }

    @objc
    public var sampling: GenerationOptionsSamplingModeWrapper? {
        guard let actualSampling = actual.sampling else { return nil }
        return GenerationOptionsSamplingModeWrapper(actual: actualSampling)
    }

    @objc
    public var temperature: NSNumber? {
        guard let actualTemperature = actual.temperature else { return nil }
        return NSNumber(value: actualTemperature)
    }

    @objc
    public var maximumResponseTokens: NSNumber? {
        guard let actualMaxTokens = actual.maximumResponseTokens else {
            return nil
        }
        return NSNumber(value: actualMaxTokens)
    }

}
