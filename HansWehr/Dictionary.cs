using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace HansWehr
{
	public class Dictionary : IDisposable
	{
		private SQLiteConnection _database { get; set; }

		static string _searchQuery = "select *, word.rowid, matchinfo(word,'pcnalx') as RawMatchInfo, offsets(word) as Offsets " +
								"from word " +
								"inner join wordmetadata " +
								"on word.rowid = wordmetadata.rowid " +
								"where definition match ? ";




		public Dictionary(IDatabaseLoader databaseLoader) {
			_database = new SQLiteConnection(databaseLoader.FilePath);
		}

		/// <summary>
		/// Search the database with the specified terms.
		/// </summary>
		/// <param name="terms">Search terms.</param>
		public IEnumerable<WordResult> Search(string terms)
		{
			var words = _database.Query<RawWordResult>(_searchQuery, terms)
								 .Select(raw => new WordResult(_searchQuery, raw));
			return words;
		}

		public Word GetWord(int wordId)
		{
			return _database.Table<Word>().FirstOrDefault(word => word.Id == wordId);
		}

		public void Dispose()
		{
			_database.Dispose();
			_database = null;

		}
	}
}
