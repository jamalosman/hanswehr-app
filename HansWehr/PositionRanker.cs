using System;
using System.Collections.Generic;
using System.Linq;

namespace HansWehr
{
	public class PositionRanker : IWordRanker
	{
		private int _maxOffset;
		private List<WordResult> _wordResults;
		private int _gate;

		public double Max
		{
			get
			{
				return _maxOffset / _gate;
			}
		}

		public PositionRanker(List<WordResult> wordResults, int gate = 1)
		{
			_wordResults = wordResults;
			_maxOffset = _wordResults.Max(result => result.Offsets.Min(offset => offset.ByteOffset));
			_gate = gate;
		}

		public List<WordResult> Rank()
		{
			_wordResults = _wordResults.ToList();
			foreach (var result in _wordResults)
			{
				result.Score = GetScore(result);
			}

			return _wordResults.OrderByDescending(result => result.Score).ToList();
		}

		public double GetScore(WordResult wordResult)
		{
			int score =  wordResult.Offsets.Min(offset => offset.ByteOffset) / _gate;
			int relativeMaxOffset = _maxOffset / _gate;
			return relativeMaxOffset - score;
		}
	}
}
