using System;
using System.Linq;
using SQLite;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;

namespace HansWehr
{
	public class WordDefinition
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; }

		[Indexed]
		public string ArabicWord { get; set; }

		[Indexed]
		public string Definition { get; set; }

		[Indexed]
		public string DefinitionSnippet { get; set; }

		[OneToMany(CascadeOperations = CascadeOperation.All)]
		public List<WordOccuranceCount> RecurringWords { get; set; }

		public WordDefinition()
		{
			if (string.IsNullOrWhiteSpace(Definition)) return;

			var words = Definition
				.Split(' ');

			DefinitionSnippet = words
				.Take(10)
				.Aggregate((agg, word) => $"{agg} {word}");

			RecurringWords = words
				.Select(word => new WordOccuranceCount
				{
					Word = word.ToLower(),
					Count = words.Count(otherWord => word == otherWord),
					WordDefinitionId = this.Id,
				})
				.Where(occ => occ.Count > 1)
				.ToList();
		}
	}

	public class WordOccuranceCount
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[Indexed]
		public string Word { get; set; }

		[Indexed]
		public int Count { get; set; }

		[ManyToOne]
		public WordDefinition Source { get; set; }

		[ForeignKey(typeof(WordDefinition))]
		public int WordDefinitionId { get; set; }
	}
}
