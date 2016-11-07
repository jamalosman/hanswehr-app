using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;

namespace HansWehr.iOS
{
    public partial class ViewController : UIViewController
    {
		static Dictionary HansWehr = new Dictionary();

        public ViewController(IntPtr handle) : base(handle)
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
				Add(ResultList);
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
