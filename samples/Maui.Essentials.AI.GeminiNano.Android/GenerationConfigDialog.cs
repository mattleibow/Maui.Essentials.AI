using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;

namespace Maui.Essentials.AI.GeminiNano.Android;

public class GenerationConfigDialog : DialogFragment
{
    public interface IOnConfigUpdateListener
    {
        void OnConfigUpdated();
    }

    public override Dialog? OnCreateDialog(Bundle? savedInstanceState)
    {
        var activity = Activity;
        var builder = new AlertDialog.Builder(activity);

        var inflater = Activity?.LayoutInflater;
        var view = inflater.Inflate(Resource.Layout.dialog_generation_config, null);

        var temperatureEditText = view?.FindViewById<EditText>(Resource.Id.temperature_edit_text);
        var topKEditText = view?.FindViewById<EditText>(Resource.Id.top_k_edit_text);
        var maxOutputTokensEditText = view?.FindViewById<EditText>(Resource.Id.max_output_tokens_edit_text);

        if (temperatureEditText != null)
        {
            temperatureEditText.Text = GenerationConfigUtils.GetTemperature(activity).ToString();
        }

        if (topKEditText != null)
        {
            topKEditText.Text = GenerationConfigUtils.GetTopK(activity).ToString();
        }

        if (maxOutputTokensEditText != null)
        {
            maxOutputTokensEditText.Text = GenerationConfigUtils.GetMaxOutputTokens(activity).ToString();
        }

        builder.SetView(view)
            .SetPositiveButton(Resource.String.button_save, (sender, args) =>
            {
                try
                {
                    if (temperatureEditText?.Text != null && float.TryParse(temperatureEditText.Text, out float temperature))
                    {
                        GenerationConfigUtils.SetTemperature(activity, temperature);
                    }

                    if (topKEditText?.Text != null && int.TryParse(topKEditText.Text, out int topK))
                    {
                        GenerationConfigUtils.SetTopK(activity, topK);
                    }

                    if (maxOutputTokensEditText?.Text != null && int.TryParse(maxOutputTokensEditText.Text, out int maxTokens))
                    {
                        GenerationConfigUtils.SetMaxOutputTokens(activity, maxTokens);
                    }

                    if (activity is IOnConfigUpdateListener listener)
                    {
                        listener.OnConfigUpdated();
                    }
                    else if (activity is MainActivity mainActivity)
                    {
                        mainActivity.OnConfigUpdated();
                    }
                }
                catch (System.Exception ex)
                {
                    Toast.MakeText(activity, $"Error saving configuration: {ex.Message}", ToastLength.Short)?.Show();
                }
            })
            .SetNegativeButton(Resource.String.button_cancel, (sender, args) =>
            {
                // Do nothing, just dismiss
            });

        return builder.Create();
    }
}