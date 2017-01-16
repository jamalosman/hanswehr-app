using System;
using System.Collections.Generic;
using Android.App;
using Android.Text;
using Android.Views;
using DroidRes = Android.Resource;
using Android.Widget;

namespace HansWehr.Droid
{
	public class WordListAdapter : BaseAdapter<WordDefinition>
	{
		public Activity Context { get; set; }
		public IList<WordDefinition> Words { get; set; }

		public WordListAdapter(Activity context, IList<WordDefinition> words) : base()
		{
			Context = context;
			Words = words;
		}
		public override WordDefinition this[int position]
		{
			get { return Words[position]; }
		}

		public override int Count
		{
			get { return Words.Count; }
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView ?? Context.LayoutInflater.Inflate(DroidRes.Layout.SimpleListItem2, null);


			var word = Words[position];
			view.FindViewById<TextView>(DroidRes.Id.Text1).Text = word.ArabicWord;

			var textView = view.FindViewById<TextView>(DroidRes.Id.Text2);

			textView.Text = word.Definition;
			textView.Ellipsize = TextUtils.TruncateAt.End;
			textView.SetSingleLine();

			return view;
		}
	}
}
