using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SQLite.Net;
using SQLite.Net.Interop;
using SQLiteNetExtensions.Extensions;

namespace HansWehr
{
	public class Database : SQLiteConnection
	{
		public static ISQLitePlatform OSPlatform
		{
			get
			{
#if __ANDROID__
				return new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();
#elif __IOS__
				return new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();
#endif
			}
		}

		public Database(string databasePath, IEnumerable<WordDefinition> words = null) : base(OSPlatform, databasePath)
		{
			CreateTable<WordDefinition>(CreateFlags.ImplicitPK | CreateFlags.AutoIncPK);
			CreateTable<WordOccuranceCount>();

			Populate(words);
		}

		public void Populate(IEnumerable<WordDefinition> words)
		{
			if (!Table<WordDefinition>().Any() && words != null)
				this.InsertAllWithChildren(words, true);
		}
	}
}
