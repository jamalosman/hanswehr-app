using System;
using System.Collections.Generic;
using Android.Content;
using Android.Database;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace HansWehr.Droid
{
	public class ExpandableWordListView : ExpandableListView
	{
		public ExpandableWordListView (Context context) : base(context)
		{
		}
	}


	public class ExpandableWordListAdapter : BaseExpandableListAdapter
	{

		private List<Word> _words;
		private Word _word;
		private Context _context;
		static string LeftToRightMarker = "\u200e";

		public ExpandableWordListAdapter(Context context, Word word, List<Word> wordResults)
		{
			_context = context;
			_words = wordResults;
			_word = word;
		}

		public override int GroupCount
		{
			get
			{
				return 1;
			}
		}

		public override bool HasStableIds
		{
			get
			{
				return false;
			}
		}

		public override Java.Lang.Object GetChild(int groupPosition, int childPosition)
		{
			return _words[childPosition].ArabicWord;
		}

		public override long GetChildId(int groupPosition, int childPosition)
		{
			return childPosition;
		}

		public override int GetChildrenCount(int groupPosition)
		{
			return _words.Count;
		}

		public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
		{
			var word = _words[childPosition];

			View view = convertView ?? View.Inflate(_context, Android.Resource.Layout.SimpleListItem2, null);
			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = word.ArabicWord;

			var detail = view.FindViewById<TextView>(Android.Resource.Id.Text2);
			detail.Text = LeftToRightMarker + word.Definition;
			detail.Ellipsize = TextUtils.TruncateAt.End;
			detail.TextDirection = TextDirection.Ltr;
			detail.SetMaxLines(2);

			return view;
		}



		public override Java.Lang.Object GetGroup(int groupPosition)
		{
			return "Words derived from " + _word.ArabicWord;
		}

		public override long GetGroupId(int groupPosition)
		{
			return groupPosition;
		}

		public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
		{
			
			View view = convertView ?? View.Inflate(_context, Android.Resource.Layout.SimpleListItem2, null);

			var detail = view.FindViewById<TextView>(Android.Resource.Id.Text2);
			detail.Text = LeftToRightMarker + _word.Definition;

			return view;


		}

		public override bool IsChildSelectable(int groupPosition, int childPosition)
		{
			return false;
		}
	}
}
