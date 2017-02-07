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

namespace HansWehr
{
	/// <summary>
	/// A searchable hans wehr dictionary
	/// </summary>
	public class LuceneDictionary : IWordDictionary
	{
		static string AppFolder
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
		}

		string _HansWehrPath = IO.Path.Combine(AppFolder, "hanswehr.xml");
		string _IndexPath = IO.Path.Combine(AppFolder, "index");
		Directory _IndexDirectory;

		private static LuceneDictionary _instance;

		/// <summary>
		/// Gets the single instance of the dictionary.
		/// </summary>
		/// <value>The single instance of the dictionary.</value>
		public static LuceneDictionary Instance
		{
			get
			{
				if (_instance == null) return _instance = new LuceneDictionary();
				else return _instance;
			}
		}

		LuceneDictionary()
		{
				
			XDocument dictionary = GetDictionary();
			IEnumerable<Word> words = GetWords(dictionary);
			_IndexDirectory = GetIndex() ?? BuildIndex(words);

		}


		Directory GetIndex()
		{
			var directory = FSDirectory.Open(_IndexPath);
			directory.EnsureOpen();
			return directory.Directory.Exists ? directory : null;
		}

		Directory BuildIndex(IEnumerable<Word> words)
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

			var assembly = typeof(LuceneDictionary).GetTypeInfo().Assembly;
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
		IEnumerable<Word> GetWords(XDocument dictionary)
		{
			return
				dictionary
				.Descendants()
				.Where(element => new[] { "rootword", "subword" }.Contains(element.Name.LocalName))
				.Select(wordElement => new Word(wordElement.Element("arabic").Value,wordElement.Element("information").Value));
		}


		/// <summary>
		/// Query the dictionary for a word.
		/// </summary>
		/// <param name="queryString">The search terms</param>
		/// <param name="limit">The maximum number of results</param>
		public IEnumerable<Word> Query(string queryString, int limit)
		{
			var analyzer = new StandardAnalyzer(Util.Version.LUCENE_30);
			var parser = new QueryParser(Util.Version.LUCENE_30, "Definition", analyzer);
			var query = parser.Parse(queryString);
			var data = new List<Word>();


			using (var searcher = new IndexSearcher(_IndexDirectory))
			{
				var hits = searcher.Search(query, limit);

				foreach (var scoreDoc in hits.ScoreDocs)
				{
					var document = searcher.Doc(scoreDoc.Doc);
					data.Add(new Word()
					{
						ArabicWord = document.Get("Arabic"),
						Definition = document.Get("Definition"),
					});
				}
			}
			return data
				.GroupBy(word => word.ArabicWord)
				.Select(g => g.First());
		}

		public IEnumerable<Word> Query(string queryString)
		{
			return Query(queryString, 50);
		}
	}
}
