using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace HansWehr
{
	public class Dictionary : SQLiteConnection
	{
		string _searchQuery = "select *, matchinfo(word,'pcnalx') as RawMatchInfo from word " +
								"inner join wordmetadata " +
								"on word.rowid = wordmetadata.rowid " +
								"where definition match ? ";



		public Dictionary(string databasePath) : base(databasePath) { }

		/// <summary>
		/// Search the database with the specified terms.
		/// </summary>
		/// <param name="terms">Search terms.</param>
		public IEnumerable<WordResult> Search(string terms)
		{
			var words = Query<RawWord>(_searchQuery, terms)
				.Select(raw => new WordResult(raw))
				.OrderByDescending(result => result.MatchInfo.TokenCounts[1]);
			return words;
		}
	}
}
