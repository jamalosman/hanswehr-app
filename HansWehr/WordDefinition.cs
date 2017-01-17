using System;
using System.Linq;
using System.Collections.Generic;
using SQLiteNetExtensions.Attributes;
using SQLite.Net.Attributes;

namespace HansWehr
{
	public class WordDefinition
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; }

		public string ArabicWord { get; set; }

		public string Definition { get; set; }

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
				.Select(word => new
				{
					Word = word.ToLower(),
					Count = words.Count(otherWord => word == otherWord),
				})
				.Where(wrd => wrd.Count > 1)
				.GroupBy(wrd => wrd.Count)
				.Select(group => new WordOccuranceCount
				{
					Count = group.First().Count,
					Words = group.Select(grp => grp.Word).Aggregate((joined, wrd) => $"{joined} {wrd}"),
					WordDefinitionId = this.Id,
				})
				.ToList();
		}
	}

	public class WordOccuranceCount
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		/// <summary>
		/// All the words in this definition that occur 'Count' number of times
		/// </summary>
		/// <value>The words.</value>
		public string Words { get; set; }

		[Ignore]
		public List<string> WordsSeparated
		{
			get
			{
				return Words.Split(' ').ToList();
			}
		}

		public int Count { get; set; }

		[ManyToOne]
		public WordDefinition Source { get; set; }

		[ForeignKey(typeof(WordDefinition))]
		public int WordDefinitionId { get; set; }
	}
}
