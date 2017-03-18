using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using HansWehr;
using Android.Content;
using Android.Views;
using IoC = TinyIoC.TinyIoCContainer;

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
			WordSearchView = FindViewById<SearchView>(Resource.Id.WordSearchView);
			ResultListView = FindViewById<ListView>(Resource.Id.ResultListView);



			WordSearchView.QueryTextSubmit += (sender, e) =>
			{
				using (var dictionary = IoC.Current.Resolve<Dictionary>())
				{
					var words = dictionary.Search(e.Query).ToList();
					//words = new WeightedRanker(new OkapiBm25Ranker(),new PositionRanker(), 0.1).Rank(words);
					//words = new PositionRanker().Rank(words);
					//var okapiBm25Ranker = new OkapiBm25Ranker(words);

					ResultListView.Adapter = Adapter = new WordResultAdapter(this, words);
				}
			};
			ResultListView.ItemClick += (sender, e) => DisplayWordView(Adapter.Results[e.Position]);
		}

		private void DisplayWordView(WordResult word)
		{
			var intent = new Intent(this, typeof(WordViewActivity));
			intent.PutExtra(WordViewActivity.WORD_ID, word.Id);
			intent.PutExtra(WordViewActivity.ROOT_WORD_ID, word.RootWordId);
			StartActivity(intent);
		}

	}
}

