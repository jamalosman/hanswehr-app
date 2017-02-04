using System;
using System.Collections.Generic;

namespace HansWehr
{
	public interface IDictionaryProvider
	{
		IEnumerable<WordDefinition> Query(string QueryString);
	}
}
