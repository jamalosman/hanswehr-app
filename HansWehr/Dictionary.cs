using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SQLite;

namespace HansWehr
{
	public class Dictionary : IDisposable
	{
		private SQLiteConnection _database { get; set; }

		const string _baseQuery = "select ArabicWord, " +
								"Definition, " +
								"RootWordId, " +
								"IsRoot, " +
								"word.rowid as Id, " +
								"matchinfo(word,'pcnalx') as RawMatchInfo, " +
								"offsets(word) as Offsets " +
								"from word " +
								"inner join wordmetadata " +
								"on word.rowid = wordmetadata.rowid ";
		const string _englishSearchQuery = _baseQuery + "where definition match ? ";
		const string _arabicSearchQuery = _baseQuery + "where arabicword match ? ";

		const string _arabicPattern = @"[\u0620-\u0660]+";


		public Dictionary(IDatabaseLoader databaseLoader)
		{
			_database = new SQLiteConnection(databaseLoader.FilePath);
		}

		/// <summary>
		/// Search the database with the specified terms.
		/// </summary>
		/// <param name="terms">Search terms.</param>
		public IList<WordResult> Search(string terms)
		{

			var regex = new Regex(_arabicPattern);
			if (regex.IsMatch(terms)) {
				return _database
					.Query<RawWordResult>(_arabicSearchQuery, terms)
					.Select(raw => new WordResult(terms, raw))
					.ToList();
			} else {
				var words = _database
					.Query<RawWordResult>(_englishSearchQuery, terms)
					.Select(raw => new WordResult(terms, raw));
				return new OkapiBm25Ranker(words.ToList()).Rank();
			}
		}

		public Word GetWord(int wordId)
		{
			return _database.Table<Word>().FirstOrDefault(word => word.Id == wordId);
		}

		public List<Word> GetDerivedWords(int wordId)
		{
			return _database.Table<Word>().Where(word => word.RootWordId == wordId).ToList();
		}

		public void Dispose()
		{
			_database.Dispose();
			_database = null;

		}
	}
}
