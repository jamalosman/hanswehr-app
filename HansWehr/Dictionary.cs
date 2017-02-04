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
using SQLite.Net;

namespace HansWehr
{
	/// <summary>
	/// A searchable hans wehr dictionary
	/// </summary>
	public class Dictionary
	{
		static string AppFolder
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
		}

		string _HansWehrPath = IO.Path.Combine(AppFolder, "hanswehr.xml");
		string _DatabasePath = IO.Path.Combine(AppFolder, "hanswehr.db");
		string _IndexPath = IO.Path.Combine(AppFolder, "index");
		Database _Database;
		Directory _IndexDirectory;
		string[] _IndexFields = {"Definition", "DefinitionSnippet",
			/*"RecurringWord2", "RecurringWord3", "RecurringWord4", "RecurringWord5"*/ };


		private static Dictionary _instance;

		/// <summary>
		/// Gets the single instance of the dictionary.
		/// </summary>
		/// <value>The single instance of the dictionary.</value>
		public static Dictionary Instance
		{
			get
			{
				if (_instance == null) return _instance = new Dictionary();
				else return _instance;
			}
		}

		Dictionary()
		{

			_Database = new Database(_DatabasePath);

			if (!_Database.Table<WordDefinition>().Any())
			{
				XDocument dictionary = GetDictionary();
				IEnumerable<WordDefinition> words = GetWords(dictionary);
				_Database.Populate(words);
				_IndexDirectory = GetIndex() ?? BuildIndex(words);

			}

		}


		Directory GetIndex()
		{
			var directory = FSDirectory.Open(_IndexPath);
			directory.EnsureOpen();
			return directory.Directory.Exists ? directory : null;
		}

		Directory BuildIndex(IEnumerable<WordDefinition> words)
		{
			var analyzer = new StandardAnalyzer(Util.Version.LUCENE_30);
			var indexDirectory = new SimpleFSDirectory(new IO.DirectoryInfo(_IndexPath));
			var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);


			foreach (var word in words)
			{
				Document doc = new Document();

				if (word == null &&
					string.IsNullOrWhiteSpace(word.ArabicWord) &&
					string.IsNullOrWhiteSpace(word.Definition))
					continue;
				doc.Add(new Field("Arabic", word.ArabicWord, Field.Store.YES, Field.Index.NOT_ANALYZED));
				doc.Add(new Field("Definition", word.Definition, Field.Store.YES, Field.Index.ANALYZED));
				// if the word appears in the snippet, which is the first few words, then it is more likely to be what they're looking for
				doc.Add(new Field("DefinitionSnippet", word.DefinitionSnippet ?? "", Field.Store.YES, Field.Index.ANALYZED) { Boost = 15 });

				//if (word.RecurringWords != null)
				//	foreach (WordOccuranceCount recurringWord in word.RecurringWords)
				//	{
				//		// words that occur more than once are added as separate fields and boosted based on how many times they occured
				//		doc.Add(new Field($"RecurringWord{recurringWord.Count}", recurringWord.Words, Field.Store.YES, Field.Index.ANALYZED)
				//		{ Boost = recurringWord.Count * 10 });
				//	}

				writer.AddDocument(doc);
			}

			return indexDirectory;
		}

		XDocument GetDictionary()
		{
			return GetDictionaryFromFile() ?? GetDictionaryFromResource();
		}

		XDocument GetDictionaryFromResource()
		{

			var assembly = typeof(Dictionary).GetTypeInfo().Assembly;
			var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".hanswehr.xml");

			return XDocument.Load(stream);
		}

		XDocument GetDictionaryFromFile()
		{
			try
			{
				var xmlString = IO.File.ReadAllText(_HansWehrPath);
				return XDocument.Parse(xmlString);
			}
			catch (IO.FileNotFoundException)
			{
				return null;
			}

		}

		/// <summary>
		/// Get all the words in the dictionary.
		/// </summary>
		/// <returns>The words in the dictionary as a list if WordDefinitions</returns>
		/// <param name="dictionary">an XML document containing the words</param>
		IEnumerable<WordDefinition> GetWords(XDocument dictionary)
		{
			return
				dictionary
				.Descendants()
				.Where(element => new[] { "rootword", "subword" }.Contains(element.Name.LocalName))
				.Select(wordElement => new WordDefinition(wordElement.Element("arabic").Value,wordElement.Element("information").Value));
		}


		/// <summary>
		/// Query the dictionary for a word.
		/// </summary>
		/// <param name="queryString">The search terms</param>
		/// <param name="limit">The maximum number of results</param>
		public IEnumerable<WordDefinition> Query(string queryString, int limit = 50)
		{
			var analyzer = new StandardAnalyzer(Util.Version.LUCENE_30);
			var parser = new MultiFieldQueryParser(Util.Version.LUCENE_30, _IndexFields, analyzer);
			var query = parser.Parse(queryString);
			var data = new List<WordDefinition>();


			using (var searcher = new IndexSearcher(_IndexDirectory))
			{
				var hits = searcher.Search(query, limit);

				foreach (var scoreDoc in hits.ScoreDocs)
				{
					var document = searcher.Doc(scoreDoc.Doc);
					data.Add(new WordDefinition()
					{
						ArabicWord = document.Get("Arabic"),
						Definition = document.Get("Definition"),
						DefinitionSnippet = document.Get("DefinitionSnippet"),
					});
				}
			}
			return data
				.GroupBy(word => word.ArabicWord)
				.Select(g => g.First());
		}
	}
}
