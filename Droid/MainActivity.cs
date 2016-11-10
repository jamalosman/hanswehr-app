using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Views.InputMethods;

namespace HansWehr.Droid
{
    [Activity(Label = "Hans Wehr", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : Activity
    {
        int count = 1;
        Dictionary HansWehr;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            HansWehr = Dictionary.Instance
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

    public class WordListAdapter : BaseAdapter<Word>
    {
        public Activity Context { get; set; }
        public IList<Word> Words { get; set; }

        public WordListAdapter(Activity context, IList<Word> words) : base()
        {
            Context = context;
            Words = words;
        }
        public override Word this[int position]
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
            var view = convertView ?? Context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
            var word = Words[position];
            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = word.ArabicWord;
            view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = word.Definition;
            return view;
        }
    }
}

