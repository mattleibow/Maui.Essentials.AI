package com.microsoft.com.microsoft.extensions.ai.google.aicore

import android.os.CancellationSignal
import androidx.lifecycle.LifecycleOwner
import androidx.lifecycle.lifecycleScope
import com.google.ai.edge.aicore.Content
import com.google.ai.edge.aicore.GenerativeModel
import kotlinx.coroutines.DelicateCoroutinesApi
import kotlinx.coroutines.GlobalScope
import kotlinx.coroutines.flow.onCompletion
import kotlinx.coroutines.future.future
import kotlinx.coroutines.launch
import java.lang.Exception

class GenerativeModelFunctions {
    companion object {

        // prepareInferenceEngine

        fun GenerativeModel.prepareInferenceEngine(
            lifecycleOwner: LifecycleOwner,
            listener: InferenceEnginePreparationListener
        ): CancellationSignal {
            val model = this
            val job = lifecycleOwner.lifecycleScope.launch {
                try {
                    model.prepareInferenceEngine()
                    listener.onSuccess()
                } catch (ex: Exception) {
                    listener.onFailure(ex)
                }
            }
            val cancellationSignal = CancellationSignal()
            cancellationSignal.setOnCancelListener {
                job.cancel()
            }
            return cancellationSignal
        }

        @OptIn(DelicateCoroutinesApi::class)
        fun GenerativeModel.prepareInferenceEngine(
            listener: InferenceEnginePreparationListener
        ): CancellationSignal {
            val model = this
            val job = GlobalScope.launch {
                try {
                    model.prepareInferenceEngine()
                    listener.onSuccess()
                } catch (ex: Exception) {
                    listener.onFailure(ex)
                }
            }
            val cancellationSignal = CancellationSignal()
            cancellationSignal.setOnCancelListener {
                job.cancel()
            }
            return cancellationSignal
        }


        // generateContent

        fun GenerativeModel.generateContent(
            lifecycleOwner: LifecycleOwner,
            listener: ContentGenerationListener,
            vararg prompt: Content
        ): CancellationSignal {
            val model = this
            val job = lifecycleOwner.lifecycleScope.future {
                try {
                    val response = model.generateContent(*prompt)
                    listener.onSuccess(response)
                } catch (ex: Exception) {
                    listener.onFailure(ex)
                }
            }
            val cancellationSignal = CancellationSignal()
            cancellationSignal.setOnCancelListener {
                job.cancel(true)
            }
            return cancellationSignal
        }

        @OptIn(DelicateCoroutinesApi::class)
        fun GenerativeModel.generateContent(
            listener: ContentGenerationListener,
            vararg prompt: Content
        ): CancellationSignal {
            val model = this
            val job = GlobalScope.future {
                try {
                    val response = model.generateContent(*prompt)
                    listener.onSuccess(response)
                } catch (ex: Exception) {
                    listener.onFailure(ex)
                }
            }
            val cancellationSignal = CancellationSignal()
            cancellationSignal.setOnCancelListener {
                job.cancel(true)
            }
            return cancellationSignal
        }


        // generateContentStream

        fun GenerativeModel.generateContentStream(
            lifecycleOwner: LifecycleOwner,
            listener: StreamContentGenerationListener,
            vararg prompt: Content
        ): CancellationSignal {
            val model = this
            val job = lifecycleOwner.lifecycleScope.future {
                model
                    .generateContentStream(*prompt)
                    .onCompletion { maybeError ->
                        listener.onComplete(maybeError)
                    }
                    .collect { response ->
                        run {
                            listener.onResponse(response)
                        }
                    }
            }
            val cancellationSignal = CancellationSignal()
            cancellationSignal.setOnCancelListener {
                job.cancel(true)
            }
            return cancellationSignal
        }

        @OptIn(DelicateCoroutinesApi::class)
        fun GenerativeModel.generateContentStream(
            listener: StreamContentGenerationListener,
            vararg prompt: Content
        ): CancellationSignal {
            val model = this
            val job = GlobalScope.future {
                model
                    .generateContentStream(*prompt)
                    .onCompletion { maybeError ->
                        listener.onComplete(maybeError)
                    }
                    .collect { response ->
                        run {
                            listener.onResponse(response)
                        }
                    }
            }
            val cancellationSignal = CancellationSignal()
            cancellationSignal.setOnCancelListener {
                job.cancel(true)
            }
            return cancellationSignal
        }

    }
}

