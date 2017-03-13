using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using HansWehr;
using Android.Content;

namespace HansWehr.Droid
{
	[Activity(Label = "HansWehr", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private SearchView WordSearchView { get; set; }
		private ListView ResultListView { get; set; }
		private WordResultAdapter Adapter { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			WordSearchView = FindViewById<SearchView>(Resource.Id.WordSearchViewId);
			ResultListView = FindViewById<ListView>(Resource.Id.ResultListViewId);

			var loader = new DatabaseLoader(this);

			WordSearchView.QueryTextSubmit += (sender, e) =>
			{
				using (var dictionary = new Dictionary(loader.FilePath))
				{
					var words = dictionary.Search(e.Query).ToList();
					ResultListView.Adapter = Adapter = new WordResultAdapter(this, words);
				}
			};
			ResultListView.ItemClick += (sender, e) => DisplayWordView(Adapter.Results[e.Position]);
		}

		private void DisplayWordView(WordResult word)
		{
			var intent = new Intent(this, typeof(WordViewActivity));
			intent.PutExtra(WordViewActivity.ARABIC_WORD, word.ArabicWord);
			intent.PutExtra(WordViewActivity.DEFINITION, word.Definition);
			StartActivity(intent);
		}

	}
}

