package com.microsoft.maui.essentials.ai

interface InferenceEnginePreparationListener {
    fun onSuccess()
    fun onFailure(exception: Exception)
}
