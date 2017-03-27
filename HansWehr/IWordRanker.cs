using System;
using System.Collections.Generic;

namespace HansWehr
{
	public interface IWordRanker
	{
		List<WordResult> Rank();

		double GetScore(WordResult wordResult);

		double Max { get; }
	}
}
