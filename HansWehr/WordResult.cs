using System;
using System.Collections.Generic;
using System.Linq;

namespace HansWehr
{
	public class WordResult
	{

		private Word _word;
		public List<string> QueryPhrases { get; set; }
		public string ArabicWord { get { return _word.ArabicWord; } }
		public string Definition { get { return _word.Definition; } }
		public int Id { get { return _word.Id; } }
		public bool IsRoot { get { return _word.IsRoot; } }
		public Word RootWord { get { return _word.RootWord; } }
		public double Score { get; set; }

		public MatchInfo MatchInfo { get; set; }
		public List<OffsetInfo> Offsets { get; set; }

		public WordResult(string query, RawWordResult rawWord) 
		{
			QueryPhrases = query.Split(' ').ToList(); 
			_word = new Word(rawWord);
			MatchInfo = new MatchInfo(QueryPhrases, rawWord.RawMatchInfo);
			Offsets = OffsetInfo.Parse(rawWord.Offsets);
		}
	}
}
