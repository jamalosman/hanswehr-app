using System;
using System.Collections.Generic;
using System.Linq;

namespace HansWehr
{
	public class OkapiBm25Ranker : IWordRanker
	{
		private List<WordResult> _wordResults;

		public double Max
		{
			get
			{
				return _wordResults.Max(wordResult => GetScore(wordResult));
			}
		}

		public OkapiBm25Ranker(List<WordResult> wordResults) { _wordResults = wordResults; }

		public List<WordResult> Rank()
		{
			int phraseCount = _wordResults[0].MatchInfo.PhraseCount;

			foreach (var wordResult in _wordResults)
			{
				wordResult.Score = PhraseScore(wordResult.MatchInfo, 0);
			}
			return _wordResults.OrderByDescending(wr => wr.Score).ToList();
		}

		public double PhraseScore(MatchInfo mi, int phraseIndex)
		{
			double k1 = 1.2f, b = 0.75f;
			PhraseColumnData col = mi.PhraseDatas[phraseIndex].ColumnDatas[1];

			double right = ((col.CurrentRowTermFrequency * (k1 + 1)) /
			                (col.CurrentRowTermFrequency + (k1 * (1 - b + (b * (mi.TokenCounts[1] / mi.AverageTokenCounts[1]))))));
			                double idf = Math.Log10((mi.RowCount - col.MatchCount + 0.5) /
			                        (col.MatchCount + 0.5));
			return idf * right;
		}

		public double GetScore(WordResult wordResult)
		{
			return PhraseScore(wordResult.MatchInfo, 0);
		}
	}
}
