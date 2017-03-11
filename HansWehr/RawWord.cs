using System;
using SQLite;
namespace HansWehr
{
	public class Word
	{
		[PrimaryKey,Column("rowid")]
		public int Id { get; set; }
		public string ArabicWord { get; set; }
		public string Definition { get; set; }
		public bool IsRoot { get; set; }
		public int RoodWordId { get; set; }
		public byte[] RawMatchInfo { get; set; }
	}
}
