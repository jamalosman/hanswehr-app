
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
using Android.Util;
using Android.Text.Util;
using LayoutParams = Android.Views.ViewGroup.LayoutParams;
using IoC = TinyIoC.TinyIoCContainer;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;

namespace HansWehr.Droid
{
	[Activity(Label = "WordViewActivity")]
	public class WordViewActivity : Activity
	{
		public static readonly string ARABIC_WORD = "ARABIC_WORD";
		public static readonly string DEFINITION = "DEFINITION";
		public static readonly string WORD_ID = "WORD_ID";
		public static readonly string ROOT_WORD_ID = "ROOT_WORD_ID";


		TextView _titleView;
		TextView _definitionView;
		Button _rootWordLinkView;


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.WordView);
			_titleView = FindViewById<TextView>(Resource.Id.WordTitleView);
			_definitionView = FindViewById<TextView>(Resource.Id.WordDefinitionView);
			_rootWordLinkView = FindViewById<Button>(Resource.Id.WordRootLinkView);
		

			if (Intent != null)
			{
				var wordId = Intent.GetIntExtra(WORD_ID, -1);
				var rootWordId = Intent.GetIntExtra(ROOT_WORD_ID, -1);



				using (var dictionary = IoC.Current.Resolve<Dictionary>())
				{
					var word = dictionary.GetWord(wordId);

					_titleView.Text = word.ArabicWord;
					_definitionView.Text = word.Definition;

					if (!word.IsRoot && rootWordId > 0)
					{
						var rootWord = dictionary.GetWord(rootWordId);
						_rootWordLinkView.Text = $"Root: {rootWord.ArabicWord}";
						_rootWordLinkView.Click += (sender, e) =>
						{
							var intent = new Intent(this, typeof(WordViewActivity));
							intent.PutExtra(WordViewActivity.WORD_ID, rootWordId);
							intent.PutExtra(WordViewActivity.ROOT_WORD_ID, rootWord.RootWordId);
							StartActivity(intent);
						};
					}
				}
			}
		}
	}
}
