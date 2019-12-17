//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
//using LiquidTextUWP.Utils;
//using LTBaseSync;
using pdftron.PDF;
using pdftron.PDF.Annots;
using pdftron.SDF;
using Action = pdftron.PDF.Action;

// ReSharper disable once CheckNamespace
namespace LiquidTextUWP.PDFExport
{
	public sealed class PDFCreator : IDisposable
	{
		private static readonly Logger s_log = new Logger(typeof(PDFCreator));

		private readonly PDFDoc m_pdfDoc;
		private readonly StorageFile m_outputFile;

		private ElementBuilder m_builder;
		private ElementWriter m_writer;
		private Font m_currentFont;
		private const double PAGE_W = 612;
		private const double PAGE_H = 792;

		/// <summary>
		/// Apparently the rectangle seems not to be taken into account for ink...
		/// </summary>
		private static readonly Rect RECT_FOR_INK = new Rect(0, 0, 10, 10);

		public PDFCreator(StorageFile outputFile)
		{
			m_outputFile = outputFile;
			m_pdfDoc = new PDFDoc();
		}

		#region Public API
		///////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Import the given files into the in-memory PDF context.
		/// </summary>
		/// <param name="fileNames"></param>
		/// <returns></returns>
		public async Task<bool> ImportFiles(List<string> fileNames)
		{
			if (fileNames.IsNullOrEmpty())
				return false;

			int lastPage = 1;

			InitSecurity(); // initialize security management

			// loop over all the files and add their content to the resulting PDFDoc:
			foreach (string fileName in fileNames)
			{
				// get the file:
				StorageFile inputFile = await fileName.GetFile();
				if (inputFile == null)
					continue;

				try
				{
					// add the content to the resulting PDFDoc:
					using (PDFDoc input = new PDFDoc(inputFile))
					{
						int inputPageNum = input.GetPageCount();
						m_pdfDoc.InsertPages(lastPage, input, 1,
							inputPageNum, PDFDocInsertFlag.e_none);
						lastPage += inputPageNum;
					}
				}
				catch (Exception e)
				{
					s_log.ErrorFormat("Cannot import file {0} due to {1}", fileName, e.Message);
				}
			}

			return true;
		}

		public async Task<bool> Generate()
		{
			m_currentFont = Font.Create(m_pdfDoc, FontStandardType1Font.e_helvetica);
			using (m_builder = new ElementBuilder())
			{
				using (m_writer = new ElementWriter())
				{
					await CreatePage();
				}
			}

			return true;
		}

		/// <summary>
		/// Rotates the given page of the given amount
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="degrees">Positive for clockwise rotations, negative for counter-clockwise rotations.</param>
		/// <returns></returns>
		public bool RotatePage(int pageNumber, int degrees)
		{
			// check the input:
			switch (degrees)
			{
				case 0:
				case 360:
					return true;
				case 90:
				case 180:
				case 270:
					// these are the right inputs...
					break;
				default:
					s_log.ErrorFormat("Cannot rotate page {0} of degrees {1}", pageNumber, degrees);
					return false;
			}

			try
			{
				// retrieve the page:
				Page page = m_pdfDoc.GetPage(pageNumber);
				if (page == null)
					return false;
				
				// get the current rotation:
				PageRotate rotation = page.GetRotation();

				// apply the rotation:
				rotation = ApplyRotation(rotation, degrees);

				// finally, apply the rotation
				page.SetRotation(rotation);
				return true;
			}
			catch (Exception e)
			{
				s_log.ErrorFormat("Cannot rotate page {0} due to {1}", e.Message);
				return false;
			}
		}

		/// <summary>
		/// Saves the current PDF context onto the file system at the outputFile location.
		/// </summary>
		/// <returns></returns>
		public async Task<bool> SaveToFile()
		{
			try
			{
				await m_pdfDoc.SaveToNewLocationAsync(m_outputFile, SDFDocSaveOptions.e_remove_unused);
			}
			catch (Exception e)
			{
				s_log.ErrorFormat("Cannot save file {0} due to {1}", m_outputFile.Path, e.Message);
				return false;
			}

			return true;
		}

		#endregion Public API
		///////////////////////////////////////////////////////////////////////////

		#region Private Utilities
		///////////////////////////////////////////////////////////////////////////

		private PageRotate ApplyRotation(PageRotate currentRotation, int degrees)
		{
			switch (currentRotation)
			{
				case PageRotate.e_0:
					switch (degrees)
					{
						case 90: return PageRotate.e_90;
						case 180: return PageRotate.e_180;
						case 270: return PageRotate.e_270;
						default: return currentRotation;
					}
					
				case PageRotate.e_90:
					switch (degrees)
					{
						case 90: return PageRotate.e_180;
						case 180: return PageRotate.e_270;
						case 270: return PageRotate.e_0;
						default: return currentRotation;
					}

				case PageRotate.e_180:
					switch (degrees)
					{
						case 90: return PageRotate.e_270;
						case 180: return PageRotate.e_0;
						case 270: return PageRotate.e_90;
						default: return currentRotation;
					}

				case PageRotate.e_270:
					switch (degrees)
					{
						case 90: return PageRotate.e_0;
						case 180: return PageRotate.e_90;
						case 270: return PageRotate.e_180;
						default: return currentRotation;
					}

				default:
					return currentRotation;
			}
		}

		private void InitSecurity()
		{
			try
			{
				m_pdfDoc.InitSecurityHandler();
			}
			catch (Exception e)
			{
				s_log.ErrorFormat("Cannot init security due to {0}", e.Message);
			}
		}

		private async Task CreatePage()
		{
			Page newPage = m_pdfDoc.PageCreate(new Rect(0, 0, PAGE_W, PAGE_H));
			m_pdfDoc.PagePushBack(newPage);
			m_writer.Begin(newPage);

			// Add title and sub title:
			Element titleElem = m_builder.CreateTextRun("test title 16", m_currentFont, 16);
			titleElem.SetTextMatrix(1, 0, 0, 1, 30, PAGE_H - 30);
			m_writer.WriteElement(titleElem);
			Element subTitleElem = m_builder.CreateTextRun("sub title 12", m_currentFont, 12);
			subTitleElem.SetTextMatrix(1, 0, 0, 1, 30, PAGE_H - 50);
			m_writer.WriteElement(subTitleElem);

			// Add the background of the page:
			Element pageBck = m_builder.CreateRect(100, 140, 40, 80);
			pageBck.SetPathFill(true);
			pageBck.SetPathStroke(true);
			GState pageBckGState = pageBck.GetGState();
			pageBckGState.SetLineWidth(2);
			pageBckGState.SetStrokeColorSpace(ColorSpace.CreateDeviceRGB());
			pageBckGState.SetFillColorSpace(ColorSpace.CreateDeviceRGB());
			pageBckGState.SetFillColor(new ColorPt(1, 1, 0));
			pageBckGState.SetStrokeColor(new ColorPt(0, 0.5, 1));
			m_writer.WriteElement(pageBck);

			Highlight pha = Highlight.Create(m_pdfDoc.GetSDFDoc(), new Rect(0, 0, 50, 200));
			pha.SetColor(new ColorPt(1, 0.5, 1), 3);
			//pha.SetOpacity(0.95);
			pha.RefreshAppearance();
			//pha.SetContentRect(new Rect(500, 50, 100, 310));
			//pha.SetPage(newPage);
			newPage.AnnotPushBack(pha);

			Destination dest = Destination.CreateFit(m_pdfDoc.GetPage(1)); 
			Action goToAction = Action.CreateGoto(dest);
			Link link = Link.Create(m_pdfDoc.GetSDFDoc(), RECT_FOR_INK, goToAction);

			FreeText txtAnnot = FreeText.Create(m_pdfDoc.GetSDFDoc(), new Rect(300, 10, 550, 400));
			txtAnnot.SetContents("\n\nSome swift brown fox snatched a gray hare out of the air by freezing it with an angry glare." +
			                     "\n\nAha!\n\nAnd there was much rejoicing!");
			txtAnnot.SetBorderStyle(new AnnotBorderStyle(AnnotBorderStyleStyle.e_solid, 1, 10, 20));
			txtAnnot.SetColor(new ColorPt(0, 0, 1));
			txtAnnot.SetOpacity(0.9);
			txtAnnot.SetQuaddingFormat(2);
			newPage.AnnotPushBack(txtAnnot);
			txtAnnot.RefreshAppearance();

			//using (ElementBuilder clippingBuilder = new ElementBuilder())
			//{
			//	Element clippingGroup = clippingBuilder.CreateGroupBegin();
			//	m_writer.WriteElement(clippingGroup);
			//	clippingBuilder.PathBegin();
			//	clippingBuilder.MoveTo(10, 450); //10, 250, 200, 240
			//	clippingBuilder.LineTo(10, 250);
			//	clippingBuilder.LineTo(100, 250);
			//	clippingBuilder.LineTo(100, 450);
			//	clippingBuilder.ClosePath();

			//	Element pathElement = clippingBuilder.PathEnd(); // path is now built
			//	pathElement.SetPathClip(true); // this path is a clipping path
			//	pathElement.SetPathStroke(true); // this path is should be filled and stroked
			//	m_writer.WriteElement(pathElement);

			//	Ink ink = CreateInkStroke(new List<Point>()
			//	{
			//		new Point(110, 10),
			//		new Point(190, 350),
			//		new Point(180, 150),
			//		new Point(280, 450),
			//		new Point(210, 110)
			//	});
			//	newPage.AnnotPushBack(ink);

			//	await CreateImage("ms-appx:///Anger.jpg");

			//	DrawPath(new Point(10, 50), new Point(40, 80), new Point(60, 95), new Point(120, 150));

			//	m_writer.WriteElement(clippingBuilder.CreateGroupEnd()); // END Element clippingGroup
			//}

			m_writer.End();
		}

		private Ink CreateInkStroke(List<Point> points)
		{
			Ink ink = Ink.Create(m_pdfDoc.GetSDFDoc(), RECT_FOR_INK);
			for (int i = 0; i < points.Count; i++)
			{
				ink.SetPoint(0, i, points[i]);
			}
			ink.SetColor(new ColorPt(0, 1, 1), 3); // how to set again ink color to Windows.UI.Color.FromArgb(128, 0, 128, 210)

			return ink;
		}

		public Highlight CreateHighlight()
		{
			Highlight pha = Highlight.Create(m_pdfDoc.GetSDFDoc(), new Rect(10, 50, 100, 310));
			pha.SetRect(new Rect(300, 50, 100, 310));
			pha.SetColor(new ColorPt(1, 0, 1), 3);
			pha.SetPage(m_pdfDoc.GetPage(1));
			return pha;
		}

		private async Task CreateImage(string fileName)
		{
			StorageFile imgFile =
				await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileName, UriKind.RelativeOrAbsolute));
			IBuffer buffer = await FileIO.ReadBufferAsync(imgFile);
			byte[] imgData = buffer.ToArray();
			Image img = Image.Create(m_pdfDoc.GetSDFDoc(), imgData);
			Element res = m_builder.CreateImage(img, 10, 250, 200, 240);
			m_writer.WritePlacedElement(res);
		}

		private void DrawPath(Point start, Point cp1, Point cp2, Point end)
		{
			m_builder.PathBegin();   // start constructing the path				                            
			m_builder.MoveTo(start.x, start.y);
			m_builder.CurveTo(cp1.x, cp1.y, cp2.x, cp2.y, end.x, end.y);
			m_builder.CurveTo(end.x + 50, end.y - 20, end.x + 80, end.y - 40, end.x + 45, end.y - 10);
			m_builder.CurveTo(end.x - 10, end.y - 120, end.x - 40, end.y - 140, end.x - 145, end.y - 100);
			m_builder.ClosePath();
			Element path = m_builder.PathEnd();     // the path is now finished
			path.SetPathFill(true);    // the path should be filled

			// Set the path color space and color
			GState gState = path.GetGState();
			gState.SetFillColorSpace(ColorSpace.CreateDeviceRGB());
			gState.SetFillColor(new ColorPt(1, 0, 0));  // red
			gState.SetTransform(1, 0, 0, 1, 120, 300);
			m_writer.WritePlacedElement(path);
		}

		#endregion Private Utilities
		///////////////////////////////////////////////////////////////////////////

		public void Dispose()
		{
			m_pdfDoc.Dispose();
		}
	}
}
