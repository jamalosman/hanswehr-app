using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace HansWehr.iOS
{
    public partial class SearchViewController : UIViewController
    {
		static IWordDictionary HansWehr = SQLiteDictionary.Instance;

        public SearchViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            SearchBar.SearchButtonClicked += (sender, e) =>
            {
				var words = HansWehr.Query(SearchBar.Text).ToList();
                ResultList.Source = new WordListDataSource { Words = words };
				ResultList.ReloadData();
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
