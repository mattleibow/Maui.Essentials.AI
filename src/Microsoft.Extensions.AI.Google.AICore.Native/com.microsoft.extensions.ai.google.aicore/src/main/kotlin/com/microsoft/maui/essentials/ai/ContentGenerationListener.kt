package com.microsoft.com.microsoft.extensions.ai.google.aicore

import com.google.ai.edge.aicore.GenerateContentResponse

interface ContentGenerationListener {
    fun onSuccess(response: GenerateContentResponse)
    fun onFailure(error: Throwable)
}

