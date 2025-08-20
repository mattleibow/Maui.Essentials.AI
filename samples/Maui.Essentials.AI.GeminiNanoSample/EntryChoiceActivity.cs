using Android.Content;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using Google.AI.Edge.AICore;

namespace Maui.Essentials.AI.GeminiNanoSample;

[Activity(MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
public class EntryChoiceActivity : AppCompatActivity
{
    private const long Megabyte = 1024 * 1024;

    private bool modelDownloaded = false;
    private GenerativeModel? model;

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_entry_choice);

        var csharpEntryPoint = FindViewById<TextView>(Resource.Id.csharp_entry_point)!;
        csharpEntryPoint.Click += (sender, e) =>
        {
            if (modelDownloaded)
                StartActivity(new Intent(this, typeof(MainActivity)));
            else
                Toast.MakeText(this, Resource.String.model_unavailable, ToastLength.Short)!.Show();
        };

        EnsureModelDownloaded();
    }

    private void EnsureModelDownloaded()
    {
        var downloadProgressTextView = FindViewById<TextView>(Resource.Id.download_progress_text_view)!;

        long totalBytesToDownload = 0;

        var downloadCallback = new DownloadCallbackImpl(
            onDownloadStarted: (bytesToDownload) =>
            {
                totalBytesToDownload = bytesToDownload;
            },
            onDownloadFailed: (failureStatus, exception) =>
            {
                Console.WriteLine($"Failed to download model: {exception}");
            },
            onDownloadProgress: (totalBytesDownloaded) =>
            {
                if (totalBytesToDownload > 0)
                {
                    downloadProgressTextView.Visibility = ViewStates.Visible;
                    var downloadedMB = totalBytesDownloaded / Megabyte;
                    var totalMB = totalBytesToDownload / Megabyte;
                    var progress = 100.0 * totalBytesDownloaded / totalBytesToDownload;
                    downloadProgressTextView.Text = $"Downloading model:  {downloadedMB} / {totalMB} MB ({progress:F2}%)";
                }
            },
            onDownloadCompleted: () =>
            {
                modelDownloaded = true;
            });

        var downloadConfig = new DownloadConfig(downloadCallback);
        var generationConfig = new GenerationConfig.Builder() { Context = ApplicationContext }.Build();

        model = new GenerativeModel(generationConfig, downloadConfig);

        Task.Run(async () =>
        {
            try
            {   
                await model.PrepareInferenceEngineAsync(this);
            }
            catch (GenerativeAIException e)
            {
                Console.WriteLine($"Failed to check model availability: {e}");

                RunOnUiThread(() =>
                {
                    Toast.MakeText(ApplicationContext, e.Message, ToastLength.Short)!.Show();
                });
            }
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        model?.Close();
    }
}
