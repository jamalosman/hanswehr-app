using System;
using System.Linq;
using UIKit;

namespace HansWehr.iOS
{
    public partial class ViewController : UIViewController
    {
        int count = 1;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Perform any additional setup after loading the view, typically from a nib.
            SearchBar.SearchButtonClicked += (sender, e) =>
            {
                var words = Dictionary.Query(SearchBar.Text).ToList();
                ResultList.DataSource = new WordListDataSource { Words = words };
            };
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.		
        }
    }
}
