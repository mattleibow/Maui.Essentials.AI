package com.microsoft.com.microsoft.extensions.ai.google.aicore

interface InferenceEnginePreparationListener {
    fun onSuccess()
    fun onFailure(error: Throwable)
}
