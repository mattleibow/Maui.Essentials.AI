namespace Maui.Essentials.AI.GeminiNanoSample;

public class GenerationConfigDialog : AndroidX.Fragment.App.DialogFragment
{
    public interface IOnConfigUpdateListener
    {
        void OnConfigUpdated();
    }

    public override Dialog OnCreateDialog(Bundle? savedInstanceState)
    {
        var view = Activity!.LayoutInflater.Inflate(Resource.Layout.dialog_generation_config, null)!;

        var temperatureEditText = view.FindViewById<EditText>(Resource.Id.temperature_edit_text)!;
        var topKEditText = view.FindViewById<EditText>(Resource.Id.top_k_edit_text)!;
        var maxOutputTokensEditText = view.FindViewById<EditText>(Resource.Id.max_output_tokens_edit_text)!;

        temperatureEditText.Text = GenerationConfigUtils.GetTemperature(Activity).ToString();
        topKEditText.Text = GenerationConfigUtils.GetTopK(Activity).ToString();
        maxOutputTokensEditText.Text = GenerationConfigUtils.GetMaxOutputTokens(Activity).ToString();

        return new AlertDialog.Builder(Activity)
            .SetView(view)!
            .SetPositiveButton(Resource.String.button_save, (sender, args) =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(temperatureEditText.Text) && float.TryParse(temperatureEditText.Text, out float temperature))
                        GenerationConfigUtils.SetTemperature(Activity, temperature);

                    if (!string.IsNullOrEmpty(topKEditText.Text) && int.TryParse(topKEditText.Text, out int topK))
                        GenerationConfigUtils.SetTopK(Activity, topK);

                    if (!string.IsNullOrEmpty(maxOutputTokensEditText.Text) && int.TryParse(maxOutputTokensEditText.Text, out int maxTokens))
                        GenerationConfigUtils.SetMaxOutputTokens(Activity, maxTokens);

                    if (Activity is IOnConfigUpdateListener listener)
                        listener.OnConfigUpdated();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(Activity, $"Error saving configuration: {ex.Message}", ToastLength.Short)?.Show();
                }
            })!
            .SetNegativeButton(Resource.String.button_cancel, (sender, args) =>
            {
                // Do nothing, just dismiss
            })!
            .Create()!;
    }
}
