using Android.Content.Res;
using Android.Graphics;
using Android.Views;
using AndroidX.RecyclerView.Widget;

namespace Maui.Essentials.AI.GeminiNanoSample;

/// <summary>
/// A RecyclerView.Adapter for displaying the request and response views.
/// </summary>
public class ContentAdapter : RecyclerView.Adapter
{
    public const int ViewTypeRequest = 0;
    public const int ViewTypeResponse = 1;
    public const int ViewTypeResponseError = 2;

    private readonly List<(int ViewType, string Content)> _contentList = [];

    public override int ItemCount => _contentList.Count;

    public void AddContent(int viewType, string? content)
    {
        _contentList.Add((viewType, content ?? string.Empty));
        NotifyDataSetChanged();
    }

    public void UpdateStreamingResponse(string response)
    {
        if (_contentList.Count <= 0)
            return;

        _contentList[^1] = (ViewTypeResponse, response);
        NotifyDataSetChanged();
    }

    public override int GetItemViewType(int position) =>
        _contentList[position].ViewType;

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var layoutId = viewType switch
        {
            ViewTypeRequest => Resource.Layout.row_item_request,
            ViewTypeResponse => Resource.Layout.row_item_response,
            ViewTypeResponseError => Resource.Layout.row_item_response,
            _ => throw new System.ArgumentException($"Invalid view type {viewType}")
        };

        var layoutInflater = LayoutInflater.From(parent.Context);
        return new ViewHolder(layoutInflater!.Inflate(layoutId, parent, false)!);
    }

    public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
    {
        if (holder is ViewHolder vh)
            vh.Bind(_contentList[position]);
    }

    class ViewHolder : RecyclerView.ViewHolder
    {
        private readonly TextView _contentTextView;
        private readonly ColorStateList _defaultTextColors;

        public ViewHolder(View view) : base(view)
        {
            _contentTextView = view.FindViewById<TextView>(Resource.Id.content_text_view)!;
            _defaultTextColors = _contentTextView.TextColors!;
        }

        public void Bind((int ViewType, string Content) content)
        {
            _contentTextView.Text = content.Content;
            if (content.ViewType == ViewTypeResponseError)
                _contentTextView.SetTextColor(Color.Red);
            else
                _contentTextView.SetTextColor(_defaultTextColors);
        }
    }
}
