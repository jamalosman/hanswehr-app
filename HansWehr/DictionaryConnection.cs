using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SQLite;

namespace HansWehr
{
	public class DictionaryConnection : SQLiteConnection
	{

		public DictionaryConnection(string databasePath, IEnumerable<Word> words = null) : base(databasePath)
		{
			DropTable<Word>();
			CreateTable<Word>(CreateFlags.FullTextSearch4);

			Populate(words);
		}

		public void Populate(IEnumerable<Word> words)
		{
			if (!Table<Word>().Any() && words != null)
				this.InsertAll(words, true);
		}
	}
}
