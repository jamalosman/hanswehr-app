using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Reflection;
using System.Linq;

namespace HansWehr
{
	/// <summary>
	/// For initally populating dicitionary providers
	/// </summary>
	public class XmlWordExtractor : IWordExtractor
	{
		static string AppFolder
		{
			get { return Environment.GetFolderPath(Environment.SpecialFolder.Personal); }
		}
		string _HansWehrPath = Path.Combine(AppFolder, "hanswehr.xml");
		

		XDocument GetDictionary()
		{
			return GetDictionaryFromFile() ?? GetDictionaryFromResource();
		}

		XDocument GetDictionaryFromResource()
		{

			var assembly = typeof(XmlWordExtractor).GetTypeInfo().Assembly;
			var stream = assembly.GetManifestResourceStream(assembly.GetName().Name + ".hanswehr.xml");

			return XDocument.Load(stream);
		}

		XDocument GetDictionaryFromFile()
		{
			try
			{
				var xmlString = File.ReadAllText(_HansWehrPath);
				return XDocument.Parse(xmlString);
			}
			catch (FileNotFoundException)
			{
				return null;
			}

		}

		/// <summary>
		/// Get all the words in the dictionary.
		/// </summary>
		/// <returns>The words in the dictionary as a list if WordDefinitions</returns>
		/// <param name="dictionary">an XML document containing the words</param>
		public IEnumerable<Word> GetWords()
		{
			return
				GetDictionary()
				.Descendants()
				.Where(element => new[] { "rootword", "subword" }.Contains(element.Name.LocalName))
				.Select(wordElement => new Word(wordElement.Element("arabic").Value, wordElement.Element("information").Value));
		}
	}
}
