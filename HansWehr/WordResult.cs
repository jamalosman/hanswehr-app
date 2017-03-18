using System;
using System.Collections.Generic;
using System.Linq;

namespace HansWehr
{
	public class WordResult
	{

		private Word _word;
		public List<string> SearchTerms { get; set; }
		public string ArabicWord { get { return _word.ArabicWord; } }
		public string Definition { get { return _word.Definition; } }
		public int Id { get { return _word.Id; } }
		public bool IsRoot { get { return _word.IsRoot; } }
		public int RootWordId { get { return _word.RootWordId; } }
		public double Score { get; set; }

		public MatchInfo MatchInfo { get; set; }
		public List<OffsetInfo> Offsets { get; set; }

		public WordResult(string searchTerms, RawWordResult rawWord) 
		{
			SearchTerms = searchTerms.Split(' ').ToList(); 
			_word = new Word(rawWord);
			MatchInfo = new MatchInfo(SearchTerms, rawWord.RawMatchInfo);
			Offsets = OffsetInfo.Parse(rawWord.Offsets);
		}
	}
}
