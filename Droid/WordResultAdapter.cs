using System;
using System.Collections.Generic;
using Android.Content;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Net;

namespace HansWehr.Droid
{
	public class WordResultAdapter : BaseAdapter<WordResult>
	{
		static string LeftToRightMarker = "\u200e";

		public WordResultAdapter(Context context, List<WordResult> results)
		{
			Results = results;
			Context = context;
		}

		public List<WordResult> Results { get; set; }
		public Context Context { get; set; }

		public override int Count
		{
			get
			{
				return Results?.Count ?? 0;
			}
		}

		public override WordResult this[int position]
		{
			get
			{
				return Results[position];
			}
		}

		public override Java.Lang.Object GetItem(int position)
		{
			return Results[position].ArabicWord;
		}

		public override long GetItemId(int position)
		{
			return Results[position].Id;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var wordResult = this[position];

			View view = convertView ?? View.Inflate(Context, Android.Resource.Layout.SimpleListItem2, null);
			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = wordResult.ArabicWord;

			var detail = view.FindViewById<TextView>(Android.Resource.Id.Text2);
			detail.Text = LeftToRightMarker + wordResult.Definition;
			detail.Ellipsize = TextUtils.TruncateAt.End;
			detail.TextDirection = TextDirection.Ltr;
			detail.SetMaxLines(3);

			return view;
		}
	}
}
