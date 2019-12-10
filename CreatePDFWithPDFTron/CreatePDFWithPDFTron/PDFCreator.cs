using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using pdftron.PDF;
using pdftron.PDF.Annots;
using pdftron.SDF;

namespace CreatePDFWithPDFTron
{
	public class PDFCreator
	{
		private PDFDoc m_pdfDoc;
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
		}

		public async Task<bool> Generate()
		{
			using (m_pdfDoc = new PDFDoc())
			{
				m_currentFont = Font.Create(m_pdfDoc, FontStandardType1Font.e_helvetica);
				using (m_builder = new ElementBuilder())
				{
					using (m_writer = new ElementWriter())
					{
						await CreatePage();
					}
				}

				try
				{
					await m_pdfDoc.SaveToNewLocationAsync(m_outputFile, SDFDocSaveOptions.e_linearized);
				}
				catch (Exception e)
				{
					// TODO log
					return false;
				}
			}

			return true;
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

			// how can I clip the page to the Rectangle defined by pageBck?
			// I want to make sure nothing is drawn outside pageBck borders, from now on.

			Ink ink = CreateInkStroke(new List<Point>()
			{
				new Point(110, 10),
				new Point(190, 350),
				new Point(180, 150),
				new Point(280, 450),
				new Point(210, 110)
			});
			newPage.AnnotPushBack(ink);

			await CreateImage("ms-appx:///Anger.jpg");

			DrawPath(new Point(10, 50), new Point(40, 80), new Point(60, 95), new Point(120,150));

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
			m_builder.CurveTo(end.x + 50, end.y - 20, end.x + 80, end.y-40, end.x + 45, end.y-10);
			m_builder.CurveTo(end.x -10, end.y - 120, end.x -40, end.y - 140, end.x -145, end.y - 100);
			m_builder.ClosePath();
			Element path = m_builder.PathEnd();     // the path is now finished
			path.SetPathFill(true);    // the path should be filled

			// Set the path color space and color
			GState gstate = path.GetGState();
			gstate.SetFillColorSpace(ColorSpace.CreateDeviceRGB());
			gstate.SetFillColor(new ColorPt(1, 0, 0));  // red
			gstate.SetTransform(1, 0, 0, 1, 120, 300);
			m_writer.WritePlacedElement(path);
		}
	}
}
