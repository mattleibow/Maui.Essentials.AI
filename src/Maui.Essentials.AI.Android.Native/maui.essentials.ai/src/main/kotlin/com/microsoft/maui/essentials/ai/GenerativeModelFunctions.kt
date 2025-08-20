package com.microsoft.maui.essentials.ai

import androidx.lifecycle.LifecycleOwner
import androidx.lifecycle.lifecycleScope
import com.google.ai.edge.aicore.GenerativeModel
import kotlinx.coroutines.DelicateCoroutinesApi
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.launch
import java.lang.Exception

class GenerativeModelFunctions {
    companion object {
        fun GenerativeModel.prepareInferenceEngine(
            lifecycleOwner: LifecycleOwner,
            listener: InferenceEnginePreparationListener
        ) {
            val model = this
            lifecycleOwner.lifecycleScope.launch {
                try {
                    model.prepareInferenceEngine()
                    listener.onSuccess()
                } catch (ex: Exception) {
                    listener.onFailure(ex)
                }
            }
        }

        @OptIn(DelicateCoroutinesApi::class)
        fun GenerativeModel.prepareInferenceEngine(
            listener: InferenceEnginePreparationListener
        ) {
            val model = this
            GlobalScope.launch {
                try {
                    model.prepareInferenceEngine()
                    listener.onSuccess()
                } catch (ex: Exception) {
                    listener.onFailure(ex)
                }
            }
        }
    }
}
