using Android.App;
using Android.Widget;
using Android.OS;
using System.Linq;
using HansWehr;

namespace HansWehr.Droid
{
	[Activity(Label = "HansWehr", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{
		private SearchView WordSearchView { get; set; }
		private ListView ResultListView { get; set; }

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
					ResultListView.Adapter = new WordResultAdapter(this, dictionary.Search(e.Query).ToList());
				}
			};
		}

	}
}

