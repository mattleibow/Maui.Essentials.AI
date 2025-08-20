package com.microsoft.maui.essentials.ai

import com.google.ai.edge.aicore.GenerateContentResponse

interface ContentGenerationListener {
    fun onSuccess(response: GenerateContentResponse)
    fun onFailure(error: Throwable)
}

