//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage;
using Windows.Graphics.Imaging;
using foxit.common;
using foxit.common.fxcrt;
using foxit.pdf;
using foxit.pdf.actions;
using foxit.pdf.annots;
using DateTime = System.DateTime;
using System.Diagnostics;
// ReSharper disable BitwiseOperatorOnEnumWithoutFlags


namespace FoxitTestBench
{
	/// <summary>
	/// Contains all the utilities to consume Foxit PDF SDK.
	/// </summary>
	public static class FoxitUtils
	{
		private const int PREVIEW_SIZE = 640;

		/// <summary>
		/// Opens the PDF document specified with the given StorageFile.
		/// </summary>
		/// <param name="file"></param>
		/// <param name="psw"></param>
		/// <returns></returns>
		public static PDFDoc OpenFoxitDoc(this StorageFile file, string psw = null)
		{
			if (file == null)
				return null;

			return OpenFoxitDoc(file.Path, psw);
		}

		/// <summary>
		/// Opens the PDF document specified at the given path.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="psw"></param>
		/// <returns></returns>
		public static PDFDoc OpenFoxitDoc(this string path, string psw = null)
		{
			if (string.IsNullOrEmpty(path))
				return null;
			try
			{
				PDFDoc res = new PDFDoc(path);
				ErrorCode err = res.LoadW(psw);
				switch (err)
				{
					case ErrorCode.e_ErrSuccess: return res;
					default:
						return null;
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot open PDF {0} due to {1}", path, e.Message);
				return null;
			}
		}

		public static string GetTitle(this PDFDoc doc)
		{
			if (doc == null)
				return string.Empty;
			using (Metadata metadata = new Metadata(doc))
			{
				WStringArray value = metadata.GetValues("Title");
				if (value != null)
				{
					string val = string.Empty;
					for (uint j = 0; j < value.GetSize(); j++)
					{
						val += string.Format("{0}", value.GetAt(j));
					}

					return val;
				}
			}
			return string.Empty;
		}

		public static PDFPage GetParsedPage(this PDFDoc doc, int page)
		{
			PDFPage p = doc?.GetPage(page);
			if (p == null)
				return null;
			if (!p.IsParsed())
				p.StartParse((int)ParseFlags.e_ParsePageNormal, null, false);
			return p;
		}

		/// <summary>
		/// Retrieves the final rectangle for a given page, in PDF coordinates.
		/// First it tries with the CropBox, if nothing is found, it falls back to MediaBox.
		/// If specified by considerRotations = true, the final rectangle is after any possible rotation
		/// applied to the page.
		/// It supports only rotations of 90, 180 and 270 degrees.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="page"></param>
		/// <param name="totPages"></param>
		/// <returns></returns>
		public static LTPDFPageInfo GetPageInfo(this PDFDoc doc, int page, int totPages)
		{
			if ((doc == null) || (page < 0) || (page >= totPages))
				return LTPDFPageInfo.EMPTY;

			PDFPage p = doc.GetParsedPage(page); // doc.GetPage(page);
			return new LTPDFPageInfo
			{
				CropRect = ToRect(p.GetBox(BoxType.e_CropBox)),
				Rect = Encoders.BuildLTRect(0, 0, p.GetWidth(), p.GetHeight()),
				Rotation = ToRot(p.GetRotation())
			};
		}

		/// <summary>
		/// Renders the given page of the given document, using the given resolution.
		/// This method can be used if the action is invoked from a background thread (non-UI thread).
		/// </summary>
		/// <param name="doc">Document which the page belongs to</param>
		/// <param name="pag">page to render (zero-based index)</param>
		/// <param name="targetSize">final size of the resulting image</param>
		/// <returns>null if anything goes wrong, otherwise the SoftwareBitmap representing an image of the page.</returns>
		public static SoftwareBitmap RenderPage(this PDFDoc doc, int pag, LTSize targetSize)
		{
			if ((doc == null) || (pag < 0) || targetSize.Equals(Encoders.SIZE_ZERO))
				return null;

			try
			{
				PDFPage page = doc.GetParsedPage(pag);
				if (page == null)
					return null;

				int targetWd = (int)targetSize.Width;
				int targetHt = (int)targetSize.Height;

				Bitmap bitmap = new Bitmap(targetWd, targetHt, DIBFormat.e_DIBRgb32, null, 0);

				// Set rect area and fill the color of bitmap:
				RectI rect = new RectI(0, 0, targetWd, targetHt);
				bitmap.FillRect(0xffffffff, rect);

				// Get the page's matrix:
				Matrix2D mt = page.GetDisplayMatrix(0, 0, targetWd, targetHt, page.GetRotation());

				// Create a renderer based on a given bitmap, and page will be rendered to this bitmap:
				Renderer renderer = new Renderer(bitmap, false);
				renderer.SetRenderContentFlags((int)(ContentFlag.e_RenderAnnot | ContentFlag.e_RenderPage));
				bool bIsParsed = page.IsParsed();
				if (!bIsParsed)
					page.StartParse(0, null, false);

				renderer.StartRender(page, mt, null);

				byte[] buffer = bitmap.GetBuffer();

				return SoftwareBitmap.CreateCopyFromBuffer(buffer.AsBuffer(), BitmapPixelFormat.Rgba8,
					targetWd, targetHt, BitmapAlphaMode.Premultiplied);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot render page {0} due to {1}", pag, e.Message);
				return null;
			}
		}

		/// <summary>
		/// Renders the given portion of the page of the given document, using the given resolution.
		/// This method can be used if the action is invoked from a background thread (non-UI thread).
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="pag"></param>
		/// <param name="regionToRender"></param>
		/// <param name="targetSize"></param>
		/// <returns></returns>
		public static SoftwareBitmap RenderTile(this PDFDoc doc, int pag, LTRect regionToRender, LTSize targetSize)
		{
			if ((doc == null) || (pag < 0) || targetSize.Equals(Encoders.SIZE_ZERO))
				return null;

			try
			{
				PDFPage page = doc.GetParsedPage(pag);
				if (page == null)
					return null;

				int targetWd = (int)targetSize.Width;
				int targetHt = (int)targetSize.Height;

				Bitmap bitmap = new Bitmap(targetWd, targetHt, DIBFormat.e_DIBRgb32, null, 0);

				// Set rect area and fill the color of bitmap:
				RectI rect = new RectI(0, 0, targetWd, targetHt);
				bitmap.FillRect(0xffffffff, rect);

				// Get the page's matrix:
				float pageWd_pdf = page.GetWidth();
				float pageHt_pdf = page.GetHeight();
				float sx = (targetWd * pageWd_pdf) / (float)regionToRender.W;
				float sy = (targetHt * pageHt_pdf) / (float)regionToRender.H;
				float px = -1 * (float)(regionToRender.X / pageWd_pdf) * sx;
				float py = -1 * (float)(regionToRender.Y / pageHt_pdf) * sy;
				Matrix2D mt = page.GetDisplayMatrix((int)px, (int)py, (int)sx, (int)sy, page.GetRotation());

				// Create a renderer based on a given bitmap, and page will be rendered to this bitmap:
				Renderer renderer = new Renderer(bitmap, false);
				renderer.SetRenderContentFlags((int)(ContentFlag.e_RenderAnnot | ContentFlag.e_RenderPage));
				bool bIsParsed = page.IsParsed();
				if (!bIsParsed)
					page.StartParse(0, null, false);

				renderer.StartRender(page, mt, null);

				byte[] buffer = bitmap.GetBuffer();

				return SoftwareBitmap.CreateCopyFromBuffer(buffer.AsBuffer(), BitmapPixelFormat.Rgba8,
					targetWd, targetHt, BitmapAlphaMode.Premultiplied);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot render page {0} due to {1}", pag, e.Message);
				return null;
			}
		}

		/// <summary>
		/// Renders the given portion of the page of the given document, using the given resolution.
		/// This method can be used if the action is invoked from a background thread (non-UI thread).
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="pag"></param>
		/// <param name="regionToRender"></param>
		/// <param name="targetSize"></param>
		/// <param name="rawFormat"></param>
		/// <returns></returns>
		public static byte[] RenderTile(this PDFDoc doc, int pag, LTRect regionToRender, LTSize targetSize, bool rawFormat)
		{
			if ((doc == null) || (pag < 0) || targetSize.Equals(Encoders.SIZE_ZERO))
				return null;

			try
			{
				PDFPage page = doc.GetParsedPage(pag);
				if (page == null)
					return null;

				int targetWd = (int)targetSize.Width;
				int targetHt = (int)targetSize.Height;

				Bitmap bitmap = new Bitmap(targetWd, targetHt, DIBFormat.e_DIBRgb32, null, 0);

				// Set rect area and fill the color of bitmap:
				RectI rect = new RectI(0, 0, targetWd, targetHt);
				bitmap.FillRect(0xffffffff, rect);

				// Get the page's matrix:
				float pageWd_pdf = page.GetWidth();
				float pageHt_pdf = page.GetHeight();
				float sx = (targetWd * pageWd_pdf) / (float)regionToRender.W;
				float sy = (targetHt * pageHt_pdf) / (float)regionToRender.H;
				float px = -1 * (float)(regionToRender.X / pageWd_pdf) * sx;
				float py = -1 * (float)(regionToRender.Y / pageHt_pdf) * sy;
				Matrix2D mt = page.GetDisplayMatrix((int)px, (int)py, (int)sx, (int)sy, page.GetRotation());

				// Create a renderer based on a given bitmap, and page will be rendered to this bitmap:
				Renderer renderer = new Renderer(bitmap, false);
				renderer.SetRenderContentFlags((int)(ContentFlag.e_RenderAnnot | ContentFlag.e_RenderPage));
				bool bIsParsed = page.IsParsed();
				if (!bIsParsed)
					page.StartParse(0, null, false);

				renderer.StartRender(page, mt, null);
				if (rawFormat)
				{
					byte[] buffer = bitmap.GetBuffer();
					return buffer;
				}
				else
				{
					foxit.common.Image image = new foxit.common.Image();
					image.AddFrame(bitmap);
					//FoxitStreamCallback fileStream = new FoxitStreamCallback();
					//image.SaveAs(fileStream, "jpeg");
					//return fileStream.toByteArray();
					return null;
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot render page {0} due to {1}", pag, e.Message);
				return null;
			}
		}


		/// <summary>
		/// Returns the size of the preview image of the document (i.e. of the first page of the doc).
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		public static LTSize GetDocPreviewSize(this PDFDoc doc)
		{
			PDFPage page0 = doc?.GetParsedPage(0);

			// Get the original image size, and then compute the size we want to show:
			return (page0 == null) ? Encoders.SIZE_ZERO :
				Encoders.BuildLTSize(page0.GetWidth() / page0.GetHeight() * PREVIEW_SIZE, PREVIEW_SIZE);
		}

		/// <summary>
		/// Changes the rotation of the page of the given document, using the given value.
		/// </summary>
		/// <param name="doc"></param>
		/// <param name="page"></param>
		/// <param name="rotation">degrees to set (0, 90, 180, 270 or 360)</param>
		public static void SetPageRotation(this PDFDoc doc, int page, int rotation)
		{
			PDFPage p = doc.GetParsedPage(page);
			if (p == null)
				return;

			switch (rotation)
			{
				case 0:
					p.SetRotation(Rotation.e_Rotation0);
					break;
				case 90:
					p.SetRotation(Rotation.e_Rotation90);
					break;
				case 180:
					p.SetRotation(Rotation.e_Rotation180);
					break;
				case 270:
					p.SetRotation(Rotation.e_Rotation270);
					break;
				default:
					p.SetRotation(Rotation.e_Rotation0);
					break;
			}
		}

		/// <summary>
		/// Searches over the given document according to the given parameters, and report the results
		/// using the two given callbacks
		/// </summary>
		/// <param name="d">document to search</param>
		/// <param name="searchTerm">string to search</param>
		/// <param name="searchProgress">invoked every 500 millisec</param>
		/// <param name="maxResults"></param>
		/// <returns></returns>
		public static List<LTSelResult> Search(this PDFDoc d, string searchTerm, int maxResults, Action<List<LTSelResult>> searchProgress)
		{
			DateTime searchStart = DateTime.Now;
			List<LTSelResult> res = new List<LTSelResult>(maxResults);
			using (TextSearch search = new TextSearch(d, null))
			{
				search.SetStartPage(0);
				search.SetEndPage(d.GetPageCount() - 1);
				search.SetPattern(searchTerm);
				search.SetSearchFlags((int)SearchFlags.e_SearchNormal);
				int totResults = 0;
				while (search.FindNext())
				{
					RectFArray rectArray = search.GetMatchRects();
					if (rectArray == null)
						continue;
					int foundRects = rectArray.GetSize();
					if (foundRects > 0)
					{
						LTRect[] matchItem = new LTRect[foundRects];
						for (int i = 0; i < foundRects; i++)
						{
							matchItem[i] = ToRect(rectArray.GetAt(i));
						}

						int page = search.GetMatchPageIndex();
						res.Add(new LTSelResult(page, search.GetMatchStartCharIndex(), search.GetMatchEndCharIndex(), matchItem));
						if (++totResults >= maxResults)
							return res;
					}

					// Provide progress if needed:
					if (searchStart.DiffFromNow().TotalMilliseconds > 500)
					{
						searchProgress(res);
						searchStart = DateTime.Now;
					}
				}
			}

			return res;
		}

		public static TextLink[] ExtractLinks(this PDFDoc doc, int pag)
		{
			PDFPage page = doc?.GetParsedPage(pag);
			if (page == null)
				return null;
			using (TextPage textPage = new TextPage(page, (int)TextParseFlags.e_ParseTextNormal))
			{
				PageTextLinks pageTextLinks = new PageTextLinks(textPage);
				int linkCount = pageTextLinks.GetTextLinkCount();
				TextLink[] res = new TextLink[linkCount];
				for (int i = 0; i < linkCount; i++)
				{
					res[i] = pageTextLinks.GetTextLink(i);
				}

				return res;
			}
		}

		public static int GetGlyphIndex(this TextPage textPage, LTPoint point, float pixelTolerance = 25)
		{
			if ((textPage == null) || textPage.IsEmpty())
				return -1;
			try
			{
				return textPage.GetIndexAtPos((float)point.X, (float)point.Y, pixelTolerance);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot get glyph index due to {0}", e.Message);
				return -1;
			}
		}

		public static TextPage GetTextPage(this PDFDoc doc, int pag)
		{
			PDFPage page = doc.GetParsedPage(pag);
			if (page == null)
				return null;
			return new TextPage(page, (int)TextParseFlags.e_ParseTextNormal);
		}

		public static LTSelResult GetTextRectsBySelection(this PDFDoc doc, int pag, LTPoint startPos, LTPoint endPos, float pixelTolerance = 5, bool wholeWords = false)
		{
			PDFPage page = doc.GetParsedPage(pag);
			if (page == null)
				return LTSelResult.EMPTY;
			try
			{
				// Create a text page from the parsed PDF page:
				TextPage textPage = new TextPage(page, (int)TextParseFlags.e_ParseTextNormal);
				if (textPage.IsEmpty())
					return LTSelResult.EMPTY;

				int startCharIndex, endCharIndex;
				if (wholeWords)
				{
					Range r1 = textPage.GetWordAtPos((float)startPos.X, (float)startPos.Y, pixelTolerance);
					Range r2 = textPage.GetWordAtPos((float)endPos.X, (float)endPos.Y, pixelTolerance);
					startCharIndex = r1.GetSegmentStart(0);
					endCharIndex = r2.GetSegmentEnd(r2.GetSegmentCount() - 1);
				}
				else
				{
					startCharIndex = textPage.GetIndexAtPos((float)startPos.X, (float)startPos.Y, pixelTolerance);
					endCharIndex = textPage.GetIndexAtPos((float)endPos.X, (float)endPos.Y, pixelTolerance);
				}

				return GetTextRects(textPage, pag, startCharIndex, endCharIndex);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot retrieve text selection due to {0}", e.Message);
				return LTSelResult.EMPTY;
			}
		}

		public static LTSelResult GetTextRectsBySelection(this PDFDoc doc, int pag, int startCharIndex, int endCharIndex)
		{
			PDFPage page = doc.GetParsedPage(pag);
			if (page == null)
				return LTSelResult.EMPTY;
			try
			{
				// Create a text page from the parsed PDF page:
				TextPage textPage = new TextPage(page, (int)TextParseFlags.e_ParseTextNormal);
				return textPage.IsEmpty() ? LTSelResult.EMPTY : GetTextRects(textPage, pag, startCharIndex, endCharIndex);
			}
			catch (Exception e)
			{
				Debug.WriteLine("CAnnot retrieve text selection due to {0}", e.Message);
				return LTSelResult.EMPTY;
			}
		}

		public static LTRect GetRectOfGlyph(this PDFDoc doc, int pag, int glyphIndex)
		{
			PDFPage page = doc.GetParsedPage(pag);
			if (page == null)
				return Encoders.RECT_ZERO;
			try
			{
				// Create a text page from the parsed PDF page:
				TextPage textPage = new TextPage(page, (int)TextParseFlags.e_ParseTextNormal);
				if (textPage.IsEmpty())
					return Encoders.RECT_ZERO;

				// this call is needed to make the GetTextRect() work!!
				textPage.GetCharCount();

				int count = textPage.GetTextRectCount(glyphIndex, 1);
				if (count > 0)
				{
					RectF rectF = textPage.GetTextRect(0);
					return IsNullOrEmpty(rectF) ? Encoders.RECT_ZERO : ToRect(rectF);
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot retrieve text selection due to {0}", e.Message);
				return Encoders.RECT_ZERO;
			}
			return Encoders.RECT_ZERO;
		}

		public static List<LTOutlineElement> GetBookmarks(this PDFDoc doc)
		{
			if (doc == null)
				return null;

			using (Bookmark root = doc.GetRootBookmark())
			{
				if (root.IsEmpty())
					return null;

				List<LTOutlineElement> res = new List<LTOutlineElement>();
				GetBookmarks(root, ref res, doc, 0);
				return res;
			}
		}

		public static QuadPoints ToQuadPoint(this LTRect rect)
		{
			float left = (float)rect.X;
			float top = (float)rect.Y;
			float right = (float)rect.GetMaxX();
			float bottom = (float)rect.GetMaxY();

			using (PointF p1 = new PointF(left, top))
			using (PointF p2 = new PointF(right, top))
			using (PointF p3 = new PointF(left, bottom))
			using (PointF p4 = new PointF(right, bottom))
				return new QuadPoints(p1, p2, p3, p4);
		}

		private static void GetBookmarks(Bookmark bookmark, ref List<LTOutlineElement> outlineElems, PDFDoc doc, int depth)
		{
			if (depth > 32)
				return;
			if (bookmark.IsEmpty())
				return;
			//for (int i = 0; i < depth; i++)
			//	doc.Write("\t");

			// Show bookmark's info (title, page, etc):
			string title = bookmark.GetTitle();
			if (!string.IsNullOrEmpty(title))
			{
				LTOutlineElement elem = new LTOutlineElement()
				{
					IndentLevel = depth,
					Title = title
				};

				// Try to retrieve the page:
				Destination dest = bookmark.GetDestination();
				if ((dest != null) && !dest.IsEmpty())
					elem.Page = dest.GetPageIndex(doc);
				else
					elem.Page = -1;

				outlineElems.Add(elem);
			}

			//// Show bookmark's color.
			//int color = bookmark.GetColor();
			//doc.Write(string.Format("color: {0:X}\r\n", color));

			// Follow the children:
			using (Bookmark firstChild = bookmark.GetFirstChild())
			{
				GetBookmarks(firstChild, ref outlineElems, doc, depth + 1);
			}

			using (Bookmark nextSibling = bookmark.GetNextSibling())
			{
				GetBookmarks(nextSibling, ref outlineElems, doc, depth);
			}
		}

		private static bool IsNullOrEmpty(RectF rect)
		{
			if (rect == null)
				return true;

			// Note: IsEmpty doesn't work!!!
			return (rect.bottom >= rect.top) || (rect.left >= rect.right);
		}

		private static LTRect ToRect(RectF r)
		{
			return Encoders.BuildLTRect(r.left, r.top, r.Width(), r.Height());
		}

		private static int ToRot(Rotation angle)
		{
			switch (angle)
			{
				case Rotation.e_Rotation0: return 0;
				case Rotation.e_Rotation90: return 90;
				case Rotation.e_Rotation180: return 180;
				case Rotation.e_Rotation270: return 270;
				default: return 0;
			}
		}

		private static LTSelResult GetTextRects(TextPage textPage, int pag, int startCharIndex, int endCharIndex)
		{
			try
			{
				// API getTextRectCount requires that start character index must be lower than or equal to end character index.
				startCharIndex = startCharIndex < endCharIndex ? startCharIndex : endCharIndex;
				endCharIndex = endCharIndex > startCharIndex ? endCharIndex : startCharIndex;

				// this call is needed to make the GetTextRect() work!!
				textPage.GetCharCount();

				int count = textPage.GetTextRectCount(startCharIndex, endCharIndex - startCharIndex + 1);
				if (count > 0)
				{

					LTRect[] array = new LTRect[count];
					for (int i = 0; i < count; i++)
					{
						RectF rectF = textPage.GetTextRect(i);
						if (IsNullOrEmpty(rectF))
							continue;
						array[i] = ToRect(rectF);
					}

					// The return rects are in PDF unit, if caller need to highlight the text rects on the screen, then these rects should be converted in device unit first.
					return new LTSelResult(pag, startCharIndex, endCharIndex, array);
				}

			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot retrieve text selection due to {0}", e.Message);
				return LTSelResult.EMPTY;
			}
			return LTSelResult.EMPTY;
		}
	}

	/// <summary>
	/// Defines a structure that contains a selection inside the PDF Document.
	/// Defined with:
	/// 1) a page
	/// 2) a start and end position
	/// 3) optionally, an array of rectangles of the selected lines
	/// </summary>
	public struct LTSelResult
	{
		public static readonly LTSelResult EMPTY = new LTSelResult(-1, 0, 0, null);

		public LTPositionInDoc Start { get; set; }
		public LTPositionInDoc End { get; set; }
		public LTRect[] Rectangles { get; set; }
		public int Page { get; set; }

		public LTSelResult(int page, int startIndex, int endIndex, LTRect[] rectangles)
		{
			Page = page;
			Start = Encoders.BuildLTPositionInDoc(page, startIndex);
			End = Encoders.BuildLTPositionInDoc(page, endIndex);
			Rectangles = rectangles;
		}

		public bool IsEmpty()
		{
			return Page < 0;
		}
	}

	/// <summary>
	/// Defines a structure for the items in the Table of Content of a PDF Document.
	/// </summary>
	public struct LTOutlineElement
	{
		/// <summary>
		/// Title of the item
		/// </summary>
		public string Title { get; set; }

		/// <summary>
		/// Depth in the hierarchy of the tree of the ToC.
		/// </summary>
		public int IndentLevel { get; set; }

		/// <summary>
		/// Page of the document which this item points to. -1 if not available.
		/// </summary>
		public int Page { get; set; }
	}
}