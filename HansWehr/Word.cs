using System;
using SQLite;

namespace HansWehr
{
	public class Word
	{
		[Column("rowid")]
		public int Id { get; set; }
		public string ArabicWord { get; set; }
		public string Definition { get; set; }
		public bool IsRoot { get; set; }
		public int RootWordId { get; set; }
		public Word RootWord { 
			get 
			{
				using (var dictionary = new Dictionary(""))
				{
					return dictionary.GetWord(RootWordId);
				}
			}
		}

		public Word () { }
		public Word(RawWordResult rawWord)
		{
			Id = rawWord.Id;
			ArabicWord = rawWord.ArabicWord;
			Definition = rawWord.Definition;
			IsRoot = rawWord.IsRoot;
		}
	}
}