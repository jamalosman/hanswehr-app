using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views.InputMethods;
using Android.Text;

namespace HansWehr.Droid
{
    [Activity(Label = "Hans Wehr", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
		LuceneDictionary HansWehr = LuceneDictionary.Instance;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
			HansWehr = LuceneDictionary.Instance;
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            var searchBox = FindViewById<EditText>(Resource.Id.searchBox);
            var resultList = FindViewById<ListView>(Resource.Id.resultList);

            searchBox.EditorAction += (sender, e) =>
            {
				if (e.Event.Action != KeyEventActions.Up) return;
                var words = HansWehr.Query(searchBox.Text).ToList();
                resultList.Adapter = new WordListAdapter(this, words);
            };
        }
    }
}

