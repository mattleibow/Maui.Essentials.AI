// using System.Runtime.CompilerServices;
// using Google.AI.Edge.AICore;
// using Google.Common.Util.Concurrent;
// using Java.Util.Concurrent;
// using Kotlin.Coroutines;
// using ReactiveStreams;

// namespace Maui.Essentials.AI.GeminiNanoSample;

// static class GenerativeModelExtensions
// {
//     public static Task PrepareInferenceEngineAsync(this GenerativeModel model)
//     {
//         var tcs = new TaskCompletionSource();

//         var complete = new OnComplete(tcs.SetResult, tcs.SetException);

//         model.PrepareInferenceEngine(complete);

//         return tcs.Task;
//     }

//     class OnComplete : Java.Lang.Object, IContinuation
//     {
//         private readonly Action onComplete;
//         private readonly Action<Exception> onError;

//         public OnComplete(Action onComplete, Action<Exception> onError)
//         {
//             this.onComplete = onComplete;
//             this.onError = onError;
//         }

//         public ICoroutineContext Context => EmptyCoroutineContext.Instance;

//         public void ResumeWith(Java.Lang.Object result)
//         {
//             var r = (Kotlin.Result)result;
//             // if (result is Kotlin.Result exception)
//             //     onError(exception);
//             // else
//             //     onComplete();
//         }
//     }
// }

// class DownloadCallbackImpl : Java.Lang.Object, IDownloadCallback
// {
//     private readonly Action<long> onDownloadStartedAction;
//     private readonly Action<string, GenerativeAIException> onDownloadFailedAction;
//     private readonly Action<long> onDownloadProgressAction;
//     private readonly Action onDownloadCompletedAction;

//     public DownloadCallbackImpl(
//         Action<long> onDownloadStarted,
//         Action<string, GenerativeAIException> onDownloadFailed,
//         Action<long> onDownloadProgress,
//         Action onDownloadCompleted)
//     {
//         onDownloadStartedAction = onDownloadStarted;
//         onDownloadFailedAction = onDownloadFailed;
//         onDownloadProgressAction = onDownloadProgress;
//         onDownloadCompletedAction = onDownloadCompleted;
//     }

//     public void OnDownloadStarted(long bytesToDownload)
//     {
//         onDownloadStartedAction?.Invoke(bytesToDownload);
//     }

//     public void OnDownloadFailed(string failureStatus, GenerativeAIException e)
//     {
//         onDownloadFailedAction?.Invoke(failureStatus, e);
//     }

//     public void OnDownloadProgress(long totalBytesDownloaded)
//     {
//         onDownloadProgressAction?.Invoke(totalBytesDownloaded);
//     }

//     public void OnDownloadCompleted()
//     {
//         onDownloadCompletedAction?.Invoke();
//     }
// }

// class Subscriber<T> : Java.Lang.Object, ISubscriber
//     where T : Java.Lang.Object
// {
//     private readonly Action<ISubscription> onSubscribe;
//     private readonly Action<T> onNext;
//     private readonly Action<Exception> onError;
//     private readonly Action onComplete;

//     public Subscriber(
//         Action<ISubscription> onSubscribe,
//         Action<T> onNext,
//         Action<Exception> onError,
//         Action onComplete)
//     {
//         this.onSubscribe = onSubscribe;
//         this.onNext = onNext;
//         this.onError = onError;
//         this.onComplete = onComplete;
//     }

//     public void OnSubscribe(ISubscription? p0) =>
//         onSubscribe(p0!);

//     public void OnNext(Java.Lang.Object? p0) =>
//         onNext((T)p0!);

//     public void OnError(Java.Lang.Throwable? p0) =>
//         onError(p0!);

//     public void OnComplete() =>
//         onComplete();
// }

// static class ListenableFutureExtensions
// {
//     public static void AddCallback<T>(
//         this IListenableFuture future,
//         Action<T?> onSuccess,
//         Action<Exception> onFailure,
//         IExecutor executor)
//         where T : Java.Lang.Object
//     {
//         var runnable = new Java.Lang.Runnable(() =>
//         {
//             try
//             {
//                 var result = future.Get();
//                 onSuccess((T?)result);
//             }
//             catch (Exception ex)
//             {
//                 onFailure(ex);
//             }
//         });

//         future.AddListener(runnable, executor);
//     }

//     public static Task<T?> AsTask<T>(this IListenableFuture future, IExecutor executor)
//         where T : Java.Lang.Object
//     {
//         var tcs = new TaskCompletionSource<T?>();

//         future.AddCallback<T>(
//             onSuccess: tcs.SetResult,
//             onFailure: tcs.SetException,
//             executor: executor);

//         return tcs.Task;
//     }
// }
