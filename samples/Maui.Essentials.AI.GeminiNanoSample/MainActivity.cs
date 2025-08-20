using Android.App;
using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Concurrent.Futures;
using AndroidX.Core.Content;
using AndroidX.RecyclerView.Widget;
using Google.AI.Edge.AICore;
using Google.AI.Edge.AICore.Java;
using Google.Common.Util.Concurrent;
using Java.Util.Concurrent;
using ReactiveStreams;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Essentials.AI.GeminiNanoSample;

/// <summary>
/// Demonstrates the AICore SDK usage from C#.
/// </summary>
[Activity(ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
public class MainActivity : AppCompatActivity, GenerationConfigDialog.IOnConfigUpdateListener
{
    private const string Tag = "MainActivity";

    private EditText requestEditText = null!;
    private Button sendButton = null!;
    private CompoundButton streamingSwitch = null!;
    private Button configButton = null!;
    private RecyclerView contentRecyclerView = null!;

    private GenerativeModel? model;
    private GenerativeModelFutures? modelFutures;

    private bool useStreaming;
    private bool hasFirstStreamingResult;

    private readonly ContentAdapter contentAdapter = new();

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        SetContentView(Resource.Layout.activity_main);

        requestEditText = FindViewById<EditText>(Resource.Id.request_edit_text)!;
        sendButton = FindViewById<Button>(Resource.Id.send_button)!;
        sendButton.Click += async (sender, e) =>
        {
            var request = requestEditText.Text?.ToString();
            if (TextUtils.IsEmpty(request))
            {
                Toast.MakeText(this, Resource.String.prompt_is_empty, ToastLength.Short)!.Show();
                return;
            }

            contentAdapter.AddContent(ContentAdapter.ViewTypeRequest, request!);

            StartGeneratingUi();

            await GenerateContent(request!);
        };

        streamingSwitch = FindViewById<CompoundButton>(Resource.Id.streaming_switch)!;
        streamingSwitch.CheckedChange += (sender, e) => useStreaming = e.IsChecked;
        useStreaming = streamingSwitch.Checked;

        configButton = FindViewById<Button>(Resource.Id.config_button)!;
        configButton.Click += (sender, e) =>
        {
            var dialog = new GenerationConfigDialog();
            dialog.Show(SupportFragmentManager, null);
        };

        contentRecyclerView = FindViewById<RecyclerView>(Resource.Id.content_recycler_view)!;
        contentRecyclerView.SetLayoutManager(new LinearLayoutManager(this));
        contentRecyclerView.SetAdapter(contentAdapter);

        InitGenerativeModel();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        modelFutures?.GenerativeModel.Close();
    }

    private void InitGenerativeModel()
    {
        var context = ApplicationContext!;

        var configBuilder = new GenerationConfig.Builder()
        {
            Context = context,
            Temperature = new(GenerationConfigUtils.GetTemperature(context)),
            TopK = new(GenerationConfigUtils.GetTopK(context)),
            MaxOutputTokens = new(GenerationConfigUtils.GetMaxOutputTokens(context))
        };

        model = new GenerativeModel(configBuilder.Build());

        modelFutures = GenerativeModelFutures.From(model);
    }

    private async Task GenerateContent(string request)
    {
        var content = new Content.Builder().AddText(request).Build();

        if (useStreaming)
        {
            hasFirstStreamingResult = false;

            var resultBuilder = new StringBuilder();

            modelFutures!.GenerateContentStream(content).Subscribe(new Subscriber<GenerateContentResponse>(
                onSubscribe: s =>
                {
                    s.Request(long.MaxValue);
                },
                onNext: response =>
                {
                    resultBuilder.Append(response.Text);
                    RunOnUiThread(() =>
                    {
                        if (hasFirstStreamingResult)
                            contentAdapter.UpdateStreamingResponse(resultBuilder.ToString());
                        else
                            contentAdapter.AddContent(ContentAdapter.ViewTypeResponse, resultBuilder.ToString());

                        hasFirstStreamingResult = true;
                    });
                },
                onError: ex =>
                {
                    Console.WriteLine("Failed to subscribe: " + ex);

                    RunOnUiThread(() =>
                    {
                        contentAdapter.AddContent(ContentAdapter.ViewTypeResponseError, ex.Message);
                        EndGeneratingUi();
                    });
                },
                onComplete: () =>
                {
                    RunOnUiThread(() => EndGeneratingUi());
                }));
        }
        else
        {
            try
            {
                var result = await modelFutures!.GenerateContent(content).AsTask<GenerateContentResponse>(ContextCompat.GetMainExecutor(this)!);

                contentAdapter.AddContent(ContentAdapter.ViewTypeResponse, result?.Text ?? "");

                EndGeneratingUi();
            }
            catch (Exception e)
            {
                contentAdapter.AddContent(ContentAdapter.ViewTypeResponseError, e.Message ?? "Unknown error");

                EndGeneratingUi();
            }
        }
    }

    private void StartGeneratingUi()
    {
        sendButton.Enabled = false;
        sendButton.SetText(Resource.String.generating);
        requestEditText.SetText(Resource.String.empty);
        streamingSwitch.Enabled = false;
        configButton.Enabled = false;
    }

    private void EndGeneratingUi()
    {
        sendButton.Enabled = true;
        sendButton.SetText(Resource.String.button_send);
        streamingSwitch.Enabled = true;
        configButton.Enabled = true;
        contentRecyclerView.SmoothScrollToPosition(contentAdapter.ItemCount - 1);
    }

    public void OnConfigUpdated()
    {
        modelFutures?.GenerativeModel.Close();

        InitGenerativeModel();
    }
}
