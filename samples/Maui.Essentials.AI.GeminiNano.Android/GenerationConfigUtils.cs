using Android.Content;
using Android.Preferences;

namespace Maui.Essentials.AI.GeminiNano.Android;

public static class GenerationConfigUtils
{
    public static float GetTemperature(Context context)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        return prefs?.GetFloat(context.GetString(Resource.String.pref_key_temperature), 0.2f) ?? 0.2f;
    }

    public static void SetTemperature(Context context, float temperature)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        var editor = prefs?.Edit();
        editor?.PutFloat(context.GetString(Resource.String.pref_key_temperature), temperature);
        editor?.Apply();
    }

    public static int GetTopK(Context context)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        return prefs?.GetInt(context.GetString(Resource.String.pref_key_top_k), 16) ?? 16;
    }

    public static void SetTopK(Context context, int topK)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        var editor = prefs?.Edit();
        editor?.PutInt(context.GetString(Resource.String.pref_key_top_k), topK);
        editor?.Apply();
    }

    public static int GetMaxOutputTokens(Context context)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        return prefs?.GetInt(context.GetString(Resource.String.pref_key_max_output_tokens), 256) ?? 256;
    }

    public static void SetMaxOutputTokens(Context context, int maxTokenCount)
    {
        var prefs = PreferenceManager.GetDefaultSharedPreferences(context);
        var editor = prefs?.Edit();
        editor?.PutInt(context.GetString(Resource.String.pref_key_max_output_tokens), maxTokenCount);
        editor?.Apply();
    }
}