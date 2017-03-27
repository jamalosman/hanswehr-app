using System;
using SQLite;

namespace HansWehr
{
	[Table("WordView")]
	public class Word
	{
		[Column("rowid")]
		public int Id { get; set; }
		public string ArabicWord { get; set; }
		public string Definition { get; set; }
		public bool IsRoot { get; set; }
		public int RootWordId { get; set; }

		public Word () { }
		public Word(RawWordResult rawWord)
		{
			Id = rawWord.Id;
			RootWordId = rawWord.RootWordId;
			ArabicWord = rawWord.ArabicWord;
			Definition = rawWord.Definition;
			IsRoot = rawWord.IsRoot;
		}
	}
}