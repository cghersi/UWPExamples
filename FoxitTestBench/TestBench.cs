using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using foxit.common.fxcrt;
using foxit.pdf;
using foxit.pdf.annots;

namespace FoxitTestBench
{
	public class TestBench
	{
		private static TextPage[] s_foxitTextPages;
		private PDFDoc m_doc;
		private static readonly RectF EMPTY_RECT_FOR_HIGHLIGHTS = new RectF(0, 0, 1, 1);

		protected void LoadDocSync(string inputPath)
		{
			m_doc = inputPath.OpenFoxitDoc();
		}

		protected int PageCount()
		{
			return m_doc.GetPageCount();
		}

		protected SoftwareBitmap RenderPageSync(int page, LTSize size)
		{
			return m_doc.RenderPage(page, size);
		}

		protected void AddHighlight(int page, List<LTRect> rectsStable)
		{
			LTProfiler prof = new LTProfiler("AddHighlight", LTProfilerLevel.Low);

			// Create the highlight for Foxit:
			PDFPage pdfPage = m_doc.GetParsedPage(page);
			if (pdfPage == null)
				return;

			try
			{
				using (Annot annot = pdfPage.AddAnnot(AnnotType.e_Highlight, EMPTY_RECT_FOR_HIGHLIGHTS))
				{
					prof.ReadTime("CreateFoxitAnnotation");
					Highlight pha = new Highlight(annot);
					prof.ReadTime("CreateFoxitHighlight");

					LTRect pageRectPDFSpace = GetRect(page, true);
					prof.ReadTime("pageRectPDFSpace");

					using (QuadPointsArray qa = new QuadPointsArray())
					{
						foreach (LTRect rctSt in rectsStable)
						{
							LTRect rctStable = Encoders.BuildLTRect(Math.Max(0, Math.Min(0.99999, rctSt.X)),
								Math.Max(page, Math.Min(page + 0.99999, rctSt.Y)),
								Math.Max(-1, Math.Min(1, rctSt.W)),
								Math.Max(-1, Math.Min(1, rctSt.H)));

							LTPoint tlPageFract = Encoders.BuildLTPoint(rctStable.X - Math.Floor(rctStable.X), rctStable.Y - Math.Floor(rctStable.Y));
							LTPoint brPageFract = Encoders.BuildLTPoint(rctStable.GetMaxX() - Math.Floor(rctStable.GetMaxX()), rctStable.GetMaxY() - Math.Floor(rctStable.GetMaxY()));

							LTPoint ulPDFSpace = tlPageFract.ConvertPoint(Encoders.RECT_ONE, pageRectPDFSpace, true, 0);
							LTPoint brPDFSpace = brPageFract.ConvertPoint(Encoders.RECT_ONE, pageRectPDFSpace, true, 0);
							LTRect rctPDFSpace = ulPDFSpace.GetRect(brPDFSpace);

							//pha.Rects.Add(rctPDFSpace.ToRect());
							qa.Add(rctPDFSpace.ToQuadPoint());
						}
						pha.SetQuadPoints(qa);
						prof.ReadTime("pha.Rects");
					}

					pha.SetBorderColor(0xff00ff);
					//pha.Color = PdfExportPdfHighlightColorForColor(ha.Color);
					//pha.PageIndex = page;  // Add start page because we're actually going through many documents here, and this keeps us on the right document.
					prof.ReadTime("pha.Color");

					pha.SetContent("Test string");
					prof.ReadTime("pha.Content");

					pha.SetUniqueID(Guid.NewGuid().ToString());
					pha.ResetAppearanceStream();
					prof.ReadTime("pha.ResetAppearanceStream");
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot create highlight due to {0}", e.Message);
				return;
			}
			prof.CollectResults("Delegate");
		}

		private LTRect GetRect(int page, bool considerRotation, bool onlyCropBox = false)
		{
			LTPDFPageInfo pageInfo = m_doc.GetPageInfo(page, PageCount());
			LTRect rectRes = onlyCropBox ? pageInfo.CropRect : pageInfo.Rect;
			return considerRotation ? rectRes.ApplyRotation(pageInfo.Rotation) : rectRes;
		}

		private TextPage GetTextPage(int page)
		{
			TextPage res = s_foxitTextPages[page];
			if (res == null)
			{
				res = m_doc.GetTextPage(page);
				s_foxitTextPages[page] = res;
			}

			return res;
		}

		public string LoadDocAndPrintPages(string pdfPath)
		{
			LTProfiler prof = new LTProfiler("Foxit:" + pdfPath.Substring(pdfPath.Length - 10), LTProfilerLevel.High);

			LoadDocSync(pdfPath);
			prof.ReadTime("LoadDoc");

			int pageCount = PageCount();
			prof.ReadTime("PageCount");

			int hash = 0;
			double originX = 0.1;
			double size1 = 0.5;
			double size2 = 0.6;
			double size3 = 0.3;
			double originY1 = 0.1;
			double originY2 = 0.4;
			double originY3 = 0.7;
			double h = 0.015;
			SoftwareBitmap imagePage = null;
			if (s_foxitTextPages == null)
				s_foxitTextPages = new TextPage[pageCount];
			List<Task> parallelTasks = new List<Task>();
			for (int i = 0; i < pageCount; i++)
			{
				// add highlights:
				AddHighlight(i, new List<LTRect>
				{
					Encoders.BuildLTRect(originX, i+ originY1, size1, h),
					Encoders.BuildLTRect(originX, i+originY1 + h + 0.001, size1, h),
					Encoders.BuildLTRect(originX, i+originY1 + 2 * h + 0.002, size1, h)
				});
				//AddHighlight(i, new List<LTRect>
				//{
				//	Encoders.BuildLTRect(originX, i+originY2, size2, h),
				//	Encoders.BuildLTRect(originX, i+originY2 + h + 0.001, size2, h),
				//	Encoders.BuildLTRect(originX, i+originY2 + 2 * h + 0.002, size2, h)
				//});
				//AddHighlight(i, new List<LTRect>
				//{
				//	Encoders.BuildLTRect(originX, i+originY3, size3, h),
				//	Encoders.BuildLTRect(originX, i+originY3 + h + 0.001, size3, h),
				//	Encoders.BuildLTRect(originX, i+originY3 + 2 * h + 0.002, size3, h)
				//});

				// Retrieve some glyph indexes (single thread):
				for (int x = 10; x < 500; x += 50)
				{
					for (int y = 10; y < 500; y += 30)
					{
						hash += GetTextPage(i).GetIndexAtPos(x, y, 25);
					}
				}

				// Retrieve some glyph indexes (parallel threads):
				for (int x = 10; x < 500; x += 50)
				{
					for (int y = 10; y < 500; y += 30)
					{
						Task t = Task.Run(() => hash += GetTextPage(i).GetIndexAtPos(x, y, 25));
						parallelTasks.Add(t);
					}
				}

				imagePage = RenderPageSync(i, m_doc.GetPageInfo(i, pageCount).Rect.Size());
				if (imagePage != null)
					hash += imagePage.PixelHeight;
			}

			Task.WaitAll(parallelTasks.ToArray());
			prof.CollectResults("RenderPage");

			// Save the full page image on the file system:
			SaveOnFileSystem(imagePage);

			//Results.Text += "\nFile " + file.Path + " saved: \n" + prof.PrintResults();
			return string.Format("\nFile loaded: pages={0}; hash={1}\n{2}", pageCount, hash,
				prof.PrintResults());
		}

		private static async void SaveOnFileSystem(SoftwareBitmap imagePage)
		{
			byte[] imageBytes = await imagePage.ToByteArray(false);
			StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(Guid.NewGuid() + ".jpg", CreationCollisionOption.OpenIfExists);
			await FileIO.WriteBytesAsync(file, imageBytes);

			Debug.WriteLine("Create image for full page at {0}", file.Path);
			//return file.Path;
		}
	}
}
