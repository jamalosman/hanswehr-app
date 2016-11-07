using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Lucene.Net.Analysis.Standard;
using Store = Lucene.Net.Store;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using System.Diagnostics;
using Lucene.Net.QueryParsers;
using Foundation;

namespace HansWehr
{
    public class Dictionary
    {
		string HansWehrPath = Path.Combine(NSBundle.MainBundle.BundlePath, "hanswehr.xml");
		string IndexPath = Path.Combine(NSBundle.MainBundle.BundlePath, "index");
		Store.Directory IndexDirectory;

		public Dictionary()
		{
			BuildIndex();
		}

		private XDocument GetDictionary()
        {
            var assembly = typeof(Dictionary).GetTypeInfo().Assembly;
            string fileName = ""; // remember case-sensitive

            var xmlString = File.ReadAllText(HansWehrPath);
            return XDocument.Parse(xmlString);
        }

        public IEnumerable<Word> GetWords()
        {
            return
                GetDictionary()
                .Descendants()
                .Where(element => new[] { "rootword", "subword" }.Contains(element.Name.LocalName))
                .Select(wordElement => new Word
                {
                    ArabicWord = wordElement.Element("arabic").Value,
                    Definition = wordElement.Element("information").Value
                });
        }

		private void BuildIndex()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
			var indexDirectory = new SimpleFSDirectory(new DirectoryInfo(IndexPath));
            var writer = new IndexWriter(indexDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
            var dictionary = GetWords();

            foreach (var word in dictionary)
            {
                Document doc = new Document();
                doc.Add(new Field("Arabic", word.ArabicWord, Field.Store.YES, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Definition", word.Definition, Field.Store.YES, Field.Index.ANALYZED));
                writer.AddDocument(doc);
            }

            writer.Optimize();
            writer.Commit();
            writer.Dispose();
            IndexDirectory = indexDirectory;
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
                Debug.WriteLine(hits.TotalHits + " result(s) found for query: " + query.ToString());
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
            return data;
        }
    }

    public class Word
    {
        public string ArabicWord { get; set; }
        public string Definition { get; set; }
    }
}
