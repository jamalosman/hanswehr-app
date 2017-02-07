using System;
using System.Linq;
using SQLite;
using System.Collections.Generic;

namespace HansWehr
{

	/// <summary>
	/// A word in the hans wehr dictionary.
	/// </summary>
	public class Word
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string ArabicWord { get; set; }

		public string Definition { get; set; }

		public string DefinitionSnippet { get; set; }

		public Word()
		{
			
		}

		public Word(string arabicWord, string definition)
		{
			ArabicWord = arabicWord;
			Definition = definition;

			var words = Definition
				.Split(' ');

			DefinitionSnippet = words
				.Take(10)
				.Aggregate((agg, word) => $"{agg} {word}");
		}
	}

}
