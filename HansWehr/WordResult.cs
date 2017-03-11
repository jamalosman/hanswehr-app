using System;
namespace HansWehr
{
	public class WordResult
	{

		public RawWord Word { get; set; }

		//private MatchInfo _matchInfo;
		public MatchInfo MatchInfo { get; set; }

		public WordResult(RawWord rawWord) 
		{
			Word = rawWord;
			MatchInfo = new MatchInfo(rawWord.RawMatchInfo);
		}

		public static implicit operator WordResult(RawWord rawWord)
		{
			return new WordResult(rawWord);
		}


	}
}
