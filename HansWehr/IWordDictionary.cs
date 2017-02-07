using System;
using System.Collections.Generic;

namespace HansWehr
{
	public interface IWordDictionary
	{
		IEnumerable<Word> Query(string queryString);
	}
}
