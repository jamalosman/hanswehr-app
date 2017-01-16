using System;
using System.Security.Cryptography;
using System.Text;
using SQLite;

namespace HansWehr
{
	public class Db
	{		
		public SQLiteConnection Connection { get; set; }

		public Db(string dbPath)
		{
			Connection = new SQLiteConnection(dbPath);

			Connection.CreateTable<WordDefinition>();
			Connection.CreateTable<WordOccuranceCount>();
		}
}
