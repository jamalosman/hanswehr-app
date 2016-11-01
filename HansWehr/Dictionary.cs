using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Lucene.Net.Analysis.Standard;
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

        private static XDocument GetDictionary()
        {
            var assembly = typeof(Dictionary).GetTypeInfo().Assembly;

            string fileName = "hanswehr.xml"; // remember case-sensitive
            string path = Path.Combine(NSBundle.MainBundle.BundlePath, fileName);

            var xmlString = File.ReadAllText(path);
            return XDocument.Parse(xmlString);
        }

        public static IEnumerable<Word> GetWords()
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

        private static RAMDirectory BuildIndex()
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var indexDirectory = new RAMDirectory();
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
            return indexDirectory;
        }

        public static IEnumerable<Word> Query(string queryString, int limit = 50)
        {
            var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            var parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_30, "Definition", analyzer);
            var query = parser.Parse(queryString);
            var indexDirectory = BuildIndex();
            var data = new List<Word>();

            using (var searcher = new IndexSearcher(indexDirectory))
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
