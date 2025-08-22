//
//  GenerationOptionsSamplingMode.swift
//  MauiEssentialsAI
//
//  Created by Matthew Leibowitz on 2025/08/22.
//

import Foundation
import FoundationModels

@available(iOS 26.0, macOS 26.0, *)
@available(tvOS, unavailable)
@available(watchOS, unavailable)
@objc(GenerationOptionsSamplingMode)
public class GenerationOptionsSamplingModeWrapper: NSObject {

    let actual: GenerationOptions.SamplingMode

    public init(actual: GenerationOptions.SamplingMode) {
        self.actual = actual
        super.init()
    }

    @objc
    public static func greedy() -> GenerationOptionsSamplingModeWrapper {
        return GenerationOptionsSamplingModeWrapper(actual: .greedy)
    }

    @objc
    public static func random(top k: Int, seed: UInt64)
        -> GenerationOptionsSamplingModeWrapper
    {
        return GenerationOptionsSamplingModeWrapper(
            actual: .random(top: k, seed: seed)
        )
    }

    @objc
    public static func randomTopK(top k: Int)
        -> GenerationOptionsSamplingModeWrapper
    {
        return GenerationOptionsSamplingModeWrapper(
            actual: .random(top: k, seed: nil)
        )
    }

    @objc
    public static func random(probability threshold: Double, seed: UInt64)
        -> GenerationOptionsSamplingModeWrapper
    {
        return GenerationOptionsSamplingModeWrapper(
            actual: .random(
                probabilityThreshold: threshold,
                seed: seed
            )
        )
    }

    @objc
    public static func random(probability threshold: Double)
        -> GenerationOptionsSamplingModeWrapper
    {
        return GenerationOptionsSamplingModeWrapper(
            actual: .random(
                probabilityThreshold: threshold,
                seed: nil
            )
        )
    }

}
