using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Util = Lucene.Net.Util;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using System.Diagnostics;
using Lucene.Net.QueryParsers;
using SQLite;

namespace HansWehr
{
	/// <summary>
	/// A searchable hans wehr dictionary
	/// </summary>
	public class SQLiteDictionary : IWordDictionary
	{
		static string AppFolder
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
		}

		IWordExtractor _WordExtractor = new XmlWordExtractor();
		string _DatabasePath = IO.Path.Combine(AppFolder, "hanswehr.db");
		readonly DictionaryConnection _Database;

		private static SQLiteDictionary _instance;

		/// <summary>
		/// Gets the single instance of the dictionary.
		/// </summary>
		/// <value>The single instance of the dictionary.</value>
		public static SQLiteDictionary Instance
		{
			get
			{
				if (_instance == null) return _instance = new SQLiteDictionary();
				else return _instance;
			}
		}

		SQLiteDictionary()
		{
			_Database = new DictionaryConnection(_DatabasePath);

			if (!_Database.Table<Word>().Any())
			{
				IEnumerable<Word> words = _WordExtractor.GetWords();
				_Database.Populate(words);
			}
		}


		public IEnumerable<Word> Query(string query)
		{
			return _Database.Query<Word>($"SELECT * FROM word " +
			                             $"WHERE definition MATCH '{query}' " +
			                             $"ORDER BY offsets(word) DESC");
			
		}



	}
}
