package com.microsoft.com.microsoft.extensions.ai.google.aicore

import com.google.ai.edge.aicore.GenerateContentResponse

interface StreamContentGenerationListener {
    fun onComplete(error: Throwable?)
    fun onResponse(response: GenerateContentResponse)
}
