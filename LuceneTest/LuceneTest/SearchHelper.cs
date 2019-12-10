using System;
using System.Collections.Generic;
using System.IO;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Directory = Lucene.Net.Store.Directory;

namespace LuceneTest
{
	public static class SearchHelper
	{
		//private static IndexSearcher s_searcher;
		//private static IndexReader s_luceneReader;

		private static readonly QueryParser s_queryParser = new QueryParser(LuceneVersion.LUCENE_48, CONTENT_FIELD,
				new StandardAnalyzer(LuceneVersion.LUCENE_48))
			{ AllowLeadingWildcard = true };

		private const string CONTENT_FIELD = "contents";
		private const string DOC_ID_FIELD = "docId";
		private const string PAGE_FIELD = "page";
		private const string PARENT_DESK_FIELD = "parentDesk";

		private static string s_lucenePath;

		public static void Init(string lucenePath)
		{
			s_lucenePath = lucenePath;

			try
			{
				SimpleFSDirectory luceneDir = new SimpleFSDirectory(lucenePath);
				if (!luceneDir.Directory.Exists)
					CreateLuceneIndex(null, Guid.Empty, Guid.Empty, 0);
				//IndexReader luceneReader = DirectoryReader.Open(luceneDir);
				//s_searcher = new IndexSearcher(luceneReader);
				//s_luceneReader = DirectoryReader.Open(FSDirectory.Open(s_lucenePath));
				//s_searcher = new IndexSearcher(s_luceneReader);
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot init Lucene due to " + e.Message);
			}
		}

		public static Dictionary<Guid, int> FullTextSearch(string searchTerm)
		{
			Dictionary<Guid, int> res = new Dictionary<Guid, int>();
			Query luceneQuery;
			if (searchTerm.Contains(" "))
				luceneQuery = s_queryParser.Parse("\"" + searchTerm + "\"");
			else
				luceneQuery = s_queryParser.Parse("*" + searchTerm + "*");

			using (IndexReader luceneReader = DirectoryReader.Open(FSDirectory.Open(s_lucenePath)))
			{
				IndexSearcher searcher = new IndexSearcher(luceneReader);
				TopDocs luceneRes = searcher.Search(luceneQuery, int.MaxValue);
				ScoreDoc[] foundDocs = luceneRes?.ScoreDocs;
				if (foundDocs != null)
				{
					foreach (ScoreDoc foundDoc in foundDocs)
					{
						Document doc = searcher.Doc(foundDoc.Doc);
						string parentDesktop = doc.Get(PARENT_DESK_FIELD);
						if (!string.IsNullOrEmpty(parentDesktop))
							AddToDict(ref res, Guid.Parse(parentDesktop));
					}
				}
			}

			return res;
		}

		private static void AddToDict(ref Dictionary<Guid, int> res, Guid id)
		{
			if (res.ContainsKey(id))
				res[id] = res[id] + 1;
			else
				res.Add(id, 1);
		}

		public static void CreateLuceneIndex(string content, Guid parentDesktop, Guid docId, int page)
		{
			Directory luceneDir = new SimpleFSDirectory(s_lucenePath);
			StandardAnalyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
			IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer) { OpenMode = OpenMode.CREATE_OR_APPEND };
			using (IndexWriter writer = new IndexWriter(luceneDir, iwc))
			{
				Document doc = new Document();

				if (content != null)
				{
					Lucene.Net.Documents.Field fldContent =
						new Field(CONTENT_FIELD, content, Field.Store.NO, Field.Index.ANALYZED,
							Field.TermVector.WITH_POSITIONS_OFFSETS);
					doc.Add(fldContent);
					//doc.Add(new TextField(CONTENT_FIELD, content, Field.Store.YES));
					doc.Add(new StringField(DOC_ID_FIELD, docId.ToString(), Field.Store.YES));
					doc.Add(new Int32Field(PAGE_FIELD, page, Field.Store.YES));
					doc.Add(new StringField(PARENT_DESK_FIELD, parentDesktop.ToString(), Field.Store.YES));
				}

				writer.AddDocument(doc);
			}
		}
	}
}
