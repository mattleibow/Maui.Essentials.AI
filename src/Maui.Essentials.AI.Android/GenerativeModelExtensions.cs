using System.Threading.Channels;
using AndroidX.Lifecycle;
using Google.AI.Edge.AICore;

namespace Maui.Essentials.AI;

public static class GenerativeModelExtensions
{
    public static Task PrepareInferenceEngineAsync(this GenerativeModel model, CancellationToken cancellationToken = default)
    {
        var listener = new InferenceEnginePreparationListener();

        var signal = GenerativeModelFunctions.Companion.PrepareInferenceEngine(model, listener);

        cancellationToken.Register(signal.Cancel);

        return listener.Task;
    }

    public static Task PrepareInferenceEngineAsync(this GenerativeModel model, ILifecycleOwner lifecycleOwner, CancellationToken cancellationToken = default)
    {
        var listener = new InferenceEnginePreparationListener();

        var signal = GenerativeModelFunctions.Companion.PrepareInferenceEngine(model, lifecycleOwner, listener);

        cancellationToken.Register(signal.Cancel);

        return listener.Task;
    }

    public static Task<GenerateContentResponse> GenerateContentAsync(this GenerativeModel model, params Content[] contents) =>
        GenerateContentAsync(model, CancellationToken.None, contents);

    public static Task<GenerateContentResponse> GenerateContentAsync(this GenerativeModel model, CancellationToken cancellationToken, params Content[] contents)
    {
        var listener = new ContentGenerationListener();

        var signal = GenerativeModelFunctions.Companion.GenerateContent(model, listener, contents);

        cancellationToken.Register(signal.Cancel);

        return listener.Task;
    }

    public static Task<GenerateContentResponse> GenerateContentAsync(this GenerativeModel model, ILifecycleOwner lifecycleOwner, params Content[] contents) =>
        GenerateContentAsync(model, lifecycleOwner, CancellationToken.None, contents);

    public static Task<GenerateContentResponse> GenerateContentAsync(this GenerativeModel model, ILifecycleOwner lifecycleOwner, CancellationToken cancellationToken, params Content[] contents)
    {
        var listener = new ContentGenerationListener();

        var signal = GenerativeModelFunctions.Companion.GenerateContent(model, lifecycleOwner, listener, contents);

        cancellationToken.Register(signal.Cancel);

        return listener.Task;
    }

    public static IAsyncEnumerable<GenerateContentResponse> GenerateContentStreamAsync(this GenerativeModel model, params Content[] contents) =>
        GenerateContentStreamAsync(model, CancellationToken.None, contents);

    public static IAsyncEnumerable<GenerateContentResponse> GenerateContentStreamAsync(this GenerativeModel model, CancellationToken cancellationToken, params Content[] contents)
    {
        var listener = new StreamContentGenerationListener();

        var signal = GenerativeModelFunctions.Companion.GenerateContentStream(model, listener, contents);

        cancellationToken.Register(signal.Cancel);

        return listener.ReadAllAsync(cancellationToken);
    }

    public static IAsyncEnumerable<GenerateContentResponse> GenerateContentStreamAsync(this GenerativeModel model, ILifecycleOwner lifecycleOwner, params Content[] contents) =>
        GenerateContentStreamAsync(model, lifecycleOwner, CancellationToken.None, contents);

    public static IAsyncEnumerable<GenerateContentResponse> GenerateContentStreamAsync(this GenerativeModel model, ILifecycleOwner lifecycleOwner, CancellationToken cancellationToken, params Content[] contents)
    {
        var listener = new StreamContentGenerationListener();

        var signal = GenerativeModelFunctions.Companion.GenerateContentStream(model, lifecycleOwner, listener, contents);

        cancellationToken.Register(signal.Cancel);

        return listener.ReadAllAsync(cancellationToken);
    }

    class StreamContentGenerationListener : Java.Lang.Object, IStreamContentGenerationListener
    {
        private readonly Channel<GenerateContentResponse> _channel;
        private readonly ChannelWriter<GenerateContentResponse> _writer;
        private readonly ChannelReader<GenerateContentResponse> _reader;

        public StreamContentGenerationListener()
        {
            _channel = Channel.CreateUnbounded<GenerateContentResponse>();
            _writer = _channel.Writer;
            _reader = _channel.Reader;
        }

        public IAsyncEnumerable<GenerateContentResponse> ReadAllAsync(CancellationToken cancellationToken = default) =>
            _reader.ReadAllAsync(cancellationToken);

        public void OnComplete(Java.Lang.Throwable? error) =>
            _writer.TryComplete(error);

        public void OnResponse(GenerateContentResponse response) =>
            _writer.TryWrite(response);
    }

    class ContentGenerationListener : Java.Lang.Object, IContentGenerationListener
    {
        private readonly TaskCompletionSource<GenerateContentResponse> _taskCompletionSource = new();

        public Task<GenerateContentResponse> Task => _taskCompletionSource.Task;

        public void OnFailure(Java.Lang.Throwable error) =>
            _taskCompletionSource.TrySetException(error);

        public void OnSuccess(GenerateContentResponse response) =>
            _taskCompletionSource.TrySetResult(response);
    }

    class InferenceEnginePreparationListener : Java.Lang.Object, IInferenceEnginePreparationListener
    {
        private readonly TaskCompletionSource _taskCompletionSource = new();

        public Task Task => _taskCompletionSource.Task;

        public void OnFailure(Java.Lang.Throwable error) =>
            _taskCompletionSource.TrySetException(error);

        public void OnSuccess() =>
            _taskCompletionSource.TrySetResult();
    }
}
