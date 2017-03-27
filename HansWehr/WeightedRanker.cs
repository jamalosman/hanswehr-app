using System;
using System.Collections.Generic;
using System.Linq;

namespace HansWehr
{
	public class WeightedRanker : IWordRanker 
	{
		private List<WordResult> _wordResults;
		private IWordRanker _leftRanker { get; set; }
		private IWordRanker _rightRanker { get; set; }
		private double _distribution;

		public double LeftWeight { get { return _distribution; } }
		public double RightWeight { get { return 1 - _distribution; } }

		public double Max
		{
			get
			{
				return _wordResults.Max(wordResult => GetScore(wordResult));
			}
		}

		public WeightedRanker(List<WordResult> wordResults, IWordRanker leftRanker, IWordRanker rightRanker, double distribution = 0.5)
		{
			_wordResults = wordResults;
			_leftRanker = leftRanker;
			_rightRanker = rightRanker;
			if (distribution > 1 || distribution < 0) 
				throw new ArgumentOutOfRangeException(nameof(distribution),"distribution value must be between 0 and 1");

			wordResults = wordResults.ToList();

			_distribution = distribution;
		}

		public List<WordResult> Rank()
		{
			foreach (var wordResult in _wordResults)
			{
				wordResult.Score = GetScore(wordResult);
			}
			return _wordResults.OrderByDescending(result => result.Score).ToList();
		}

		public double GetScore(WordResult wordResult)
		{
			double leftScore = (_leftRanker.GetScore(wordResult) / _leftRanker.Max) * 100;
			double rightScore = (_rightRanker.GetScore(wordResult) / _rightRanker.Max) * 100;

			return (leftScore * LeftWeight) + (rightScore * RightWeight);
		}
	}
}
