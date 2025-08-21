package com.microsoft.maui.essentials.ai

import com.google.ai.edge.aicore.GenerateContentResponse

interface StreamContentGenerationListener {
    fun onComplete(error: Throwable?)
    fun onResponse(response: GenerateContentResponse)
}
