using System;
namespace HansWehr
{
	public class WordResult
	{

		private Word _word;
		public string ArabicWord { get { return _word.ArabicWord; } }
		public string Definition { get { return _word.Definition; } }
		public int Id { get { return _word.Id; } }
		public bool IsRoot { get { return _word.IsRoot; } }
		public Word RootWord { get { return _word.RootWord; } }

		public MatchInfo MatchInfo { get; set; }

		public WordResult(RawWordResult rawWord) 
		{
			_word = new Word(rawWord);
			MatchInfo = new MatchInfo(rawWord.RawMatchInfo);
		}

		public static implicit operator WordResult(RawWordResult rawWord)
		{
			return new WordResult(rawWord);
		}
	}
}
