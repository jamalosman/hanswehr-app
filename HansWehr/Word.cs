using System;
using System.Linq;

namespace HansWehr
{
	public class Word {

		public string Id { get; set; }
		public string ArabicWord { get; set; }
		public string Definition { get; set; }
		public string DefinitionSnippet
		{
			get
			{
				return Definition
					.Split(' ')
					.Take(5)
					.Aggregate((agg, word) => $"{agg} {word}");
			}
		}
	}
}
