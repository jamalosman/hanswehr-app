using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HansWehr
{
	public interface IWordExtractor
	{
		IEnumerable<Word> GetWords();
	}
}
