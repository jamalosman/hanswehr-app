using System;
using System.Collections.Generic;
using Foundation;
using UIKit;

namespace HansWehr.iOS
{
    public class WordListDataSource : UITableViewDataSource
    {
        string CellIdentifier = "WordCell";


        public List<Word> Words { get; set; }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) 
                                            ?? new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);
            var word = Words[indexPath.Row];
            cell.TextLabel.Text = word.ArabicWord;
            cell.DetailTextLabel.Text = word.Definition;

            return new UITableViewCell(UITableViewCellStyle.Subtitle, word.ArabicWord);
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return Words.Count;
        }
    }
}
