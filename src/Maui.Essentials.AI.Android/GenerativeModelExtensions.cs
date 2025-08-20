using AndroidX.Lifecycle;
using Google.AI.Edge.AICore;

namespace Maui.Essentials.AI;

public static class GenerativeModelExtensions
{
    public static Task PrepareInferenceEngineAsync(this GenerativeModel model)
    {
        var listener = new InferenceEnginePreparationListener();

        GenerativeModelFunctions.Companion.PrepareInferenceEngine(model, listener);

        return listener.Task;
    }

    public static Task PrepareInferenceEngineAsync(this GenerativeModel model, ILifecycleOwner lifecycleOwner)
    {
        var listener = new InferenceEnginePreparationListener();

        GenerativeModelFunctions.Companion.PrepareInferenceEngine(model, lifecycleOwner, listener);

        return listener.Task;
    }

    class InferenceEnginePreparationListener : Java.Lang.Object, IInferenceEnginePreparationListener
    {
        private readonly TaskCompletionSource _taskCompletionSource = new();

        public Task Task => _taskCompletionSource.Task;

        public void OnFailure(Java.Lang.Exception exception) =>
            _taskCompletionSource.TrySetException(exception);

        public void OnSuccess() =>
            _taskCompletionSource.TrySetResult();
    }
}
