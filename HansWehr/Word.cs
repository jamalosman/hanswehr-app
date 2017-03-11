using System;
namespace HansWehr
{
	public class Word
	{

		public int Id { get; set; }
		public string ArabicWord { get; set; }
		public string Definition { get; set; }
		public bool IsRoot { get; set; }

		public Word () { }
		public Word(RawWord rawWord)
		{
			Id = rawWord.Id;
			ArabicWord = rawWord.ArabicWord;
			Definition = rawWord.Definition;
			IsRoot = rawWord.IsRoot;
		}
	}
}