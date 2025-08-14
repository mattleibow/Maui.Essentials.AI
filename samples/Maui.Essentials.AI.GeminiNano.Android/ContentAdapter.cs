using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Java.Lang;

namespace Maui.Essentials.AI.GeminiNano.Android;

/// <summary>
/// A ListView adapter for displaying the request and response views.
/// </summary>
public class ContentAdapter : BaseAdapter
{
    public const int ViewTypeRequest = 0;
    public const int ViewTypeResponse = 1;
    public const int ViewTypeResponseError = 2;

    private readonly List<(int ViewType, string Content)> _contentList = new();
    private readonly Context _context;

    public ContentAdapter(Context context)
    {
        _context = context;
    }

    public void AddContent(int viewType, string? content)
    {
        _contentList.Add((viewType, content ?? string.Empty));
        NotifyDataSetChanged();
    }

    public void UpdateStreamingResponse(string response)
    {
        if (_contentList.Count > 0)
        {
            var lastIndex = _contentList.Count - 1;
            _contentList[lastIndex] = (ViewTypeResponse, response);
            NotifyDataSetChanged();
        }
    }

    public override int Count => _contentList.Count;

    public override Object? GetItem(int position)
    {
        return _contentList[position].Content;
    }

    public override long GetItemId(int position)
    {
        return position;
    }

    public override View? GetView(int position, View? convertView, ViewGroup? parent)
    {
        var (viewType, content) = _contentList[position];
        
        View? view = convertView;
        
        if (view == null || (int?)view.Tag != viewType)
        {
            var layoutInflater = LayoutInflater.From(_context);
            var layoutId = viewType switch
            {
                ViewTypeRequest => Resource.Layout.row_item_request,
                ViewTypeResponse => Resource.Layout.row_item_response,
                ViewTypeResponseError => Resource.Layout.row_item_response,
                _ => Resource.Layout.row_item_response
            };
            
            view = layoutInflater?.Inflate(layoutId, parent, false);
            if (view != null)
            {
                view.Tag = viewType;
            }
        }
        
        if (view != null)
        {
            var contentTextView = view.FindViewById<TextView>(Resource.Id.content_text_view);
            if (contentTextView != null)
            {
                contentTextView.Text = content;
                
                if (viewType == ViewTypeResponseError)
                {
                    contentTextView.SetTextColor(global::Android.Graphics.Color.Red);
                }
                else
                {
                    contentTextView.SetTextColor(global::Android.Graphics.Color.Black);
                }
            }
        }
        
        return view;
    }

    public override int ViewTypeCount => 3;

    public override int GetItemViewType(int position)
    {
        return _contentList[position].ViewType;
    }
}