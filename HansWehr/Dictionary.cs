using System;
using System.Collections.Generic;
using IO = System.IO;
using System.Reflection;
using System.Linq;
using System.Xml.Linq;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using System.Diagnostics;
using Lucene.Net.QueryParsers;
using System.Security.Cryptography;
using System.Text;

namespace HansWehr
{
	public class Dictionary
	{
		static string AppFolder
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
		}

		string HansWehrPath = IO.Path.Combine(AppFolder, "hanswehr.xml");
		string IndexPath = IO.Path.Combine(AppFolder, "index");
		Directory IndexDirectory;

		private static Dictionary _instance;
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
			IndexDirectory = GetIndex() ?? BuildIndex();
		}

		Directory GetIndex()
		{
			var directory = FSDirectory.Open(IndexPath);
			directory.EnsureOpen();
			return directory.Directory.Exists ? directory : null;
		}

		Directory BuildIndex()
		{
			var dictionary = GetWords();
			var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
			var indexDirectory = new SimpleFSDirectory(new IO.DirectoryInfo(IndexPath));
			var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);


			foreach (var word in dictionary)
			{
				Document doc = new Document();
				doc.Add(new Field("Arabic", word.ArabicWord, Field.Store.YES, Field.Index.NOT_ANALYZED));
				doc.Add(new Field("Definition", word.Definition, Field.Store.YES, Field.Index.ANALYZED));
				doc.Add(new Field("DefinitionSnippet", word.DefinitionSnippet, Field.Store.YES, Field.Index.ANALYZED));
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
			Debug.WriteLine(string.Join(",", assembly.GetManifestResourceNames()));
			Debug.WriteLine(assembly.GetName().Name + ".hanswehr.xml");

			return XDocument.Load(stream);

			//var xmlString = File.ReadAllText(HansWehrPath);
			//return XDocument.Parse(xmlString);
		}

		XDocument GetDictionaryFromFile()
		{
			try
			{
				var xmlString = IO.File.ReadAllText(HansWehrPath);
				return XDocument.Parse(xmlString);
			}
			catch (IO.FileNotFoundException) // want to find out what exception will get thrown
			{
				return null;
			}

		}

		public IEnumerable<Word> GetWords()
		{
			var Words =
				GetDictionary()
				.Descendants()
				.Where(element => new[] { "rootword", "subword" }.Contains(element.Name.LocalName))
				.Select(wordElement => new Word
				{
					ArabicWord = wordElement.Element("arabic").Value,
					Definition = wordElement.Element("information").Value
				});
			return Words;
		}



		public IEnumerable<Word> Query(string queryString, int limit = 50)
		{
			var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
			var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Definition", analyzer);
			var query = parser.Parse(queryString);
			var data = new List<Word>();


			using (var searcher = new IndexSearcher(IndexDirectory))
			{
				var hits = searcher.Search(query, limit);

				foreach (var scoreDoc in hits.ScoreDocs)
				{
					var document = searcher.Doc(scoreDoc.Doc);
					data.Add(new Word()
					{
						ArabicWord = document.Get("Arabic"),
						Definition = document.Get("Definition")
					});
				}
			}
			return data
				.GroupBy(word => word.Definition)
				.Select(g => g.First());
		}
	}
}
