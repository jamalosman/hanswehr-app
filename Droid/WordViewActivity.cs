
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HansWehr.Droid
{
	[Activity(Label = "WordViewActivity")]
	public class WordViewActivity : Activity
	{
		public static readonly string ARABIC_WORD = "ARABIC_WORD";
		public static readonly string DEFINITION = "DEFINITION";

		TextView _titleView;
		TextView TitleView
		{
			get
			{
				if (_titleView == null) _titleView = FindViewById<TextView>(Resource.Id.TitleView);
				return _titleView;
			}
		}

		TextView _definitionView;
		TextView DefinitionView { 
			get 
			{
				if (_definitionView == null)  _definitionView = FindViewById<TextView>(Resource.Id.DefinitionView);
				return _definitionView;
			}
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.WordView);

			if (Intent != null)
			{
				var arabicWord = Intent.GetStringExtra(ARABIC_WORD);
				var definition = Intent.GetStringExtra(DEFINITION);

				TitleView.Text = arabicWord;
				DefinitionView.Text = definition;
			}
		}
	}
}
