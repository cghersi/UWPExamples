using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuceneTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string idxDir = Directory.GetCurrentDirectory() + "\\Index";
			string sourceDir = Directory.GetCurrentDirectory() + "\\testDocs";
			//IndexFiles.Index(new [] { idxDir, sourceDir });
			//SearchFiles.Search(new[] { idxDir, "-q", "contents:test" });

			TestSearch();
			Console.WriteLine("END");
		}

		private static void TestSearch()
		{
			//setup environment:
			SearchHelper.Init(Directory.GetCurrentDirectory() + "\\Index");
			Guid desk1 = Guid.NewGuid();
			Guid desk2 = Guid.NewGuid();
			SearchHelper.CreateLuceneIndex("this is a simple content for test of Lucene", desk1, Guid.NewGuid(), 3);
			SearchHelper.CreateLuceneIndex("this is a complex content for test of Lucene", desk1, Guid.NewGuid(), 4);
			SearchHelper.CreateLuceneIndex("this is a very complex content for test of Lucene", desk2, Guid.NewGuid(), 5);

			//Thread.Sleep(4000);

			string input = "luc";

			//invoke action:
			Dictionary<Guid, int> res = SearchHelper.FullTextSearch(input);
			foreach (KeyValuePair<Guid, int> pair in res)
			{
				Console.WriteLine("desktop " + pair.Key + " has " + pair.Value + " documents");
			}
		}
	}
}
