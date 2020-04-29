using Syncfusion.Pdf;
using Syncfusion.Pdf.Parsing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportPDFWithSyncFusion
{
		public class LTPDFDrawingContext
		{
			//The syncfusion pdf document object.
			PdfDocument m_pdfDoc;

			//The current page we're drawing onto.
			PdfPage m_pdfPage;

			////The graphics context for the current page we're on.
			//PdfGraphics m_graphicsContext;

			////The current pen and brush.
			//PdfPen m_currentPen;
			//PdfBrush m_currentBrush;

			//The size for new pages.
			//LTSize m_defaultPageSize = new LTSize(600, 776);

			//Page rotations we retrieved from the original files of a merged PDF.
			List<PdfPageRotateAngle> m_retrievedRotationAngles = new List<PdfPageRotateAngle>();

			//--------------------------------------------------------------------------------------------------------

			/// <summary>
			/// The creator method.
			/// </summary>
			public LTPDFDrawingContext()
			{
				//Make a new pdf document which we'll draw into.
				m_pdfDoc = new PdfDocument();

			}

			//--------------------------------------------------------------------------------------------------------

			private LTPDFDrawingContext(PdfDocument fromPDFDocument)
			{
				m_pdfDoc = fromPDFDocument;
			}

			//--------------------------------------------------------------------------------------------------------

			//public LTSize DefaultPageSize { get => m_defaultPageSize; set => m_defaultPageSize = value; }

			////--------------------------------------------------------------------------------------------------------
			////--  Methods for setting the coordinate system transform.
			////--------------------------------------------------------------------------------------------------------
			//public void saveContextState()
			//{
			//	m_graphicsContext.Save();
			//}

			////--------------------------------------------------------------------------------------------------------

			//public void restoreContextState()
			//{
			//	m_graphicsContext.Restore();
			//}

			//--------------------------------------------------------------------------------------------------------

			////public void translate(LTPoint translation)
			////{
			////	m_graphicsContext.TranslateTransform((float)translation.X, (float)translation.Y);
			////}

			////--------------------------------------------------------------------------------------------------------

			//public void scale(double sx, double sy)
			//{
			//	m_graphicsContext.ScaleTransform((float)sx, (float)sy);
			//}

			////--------------------------------------------------------------------------------------------------------

			//public void setClip(LTRect rct)
			//{
			//	PdfPath path = new PdfPath();
			//	path.AddRectangle((float)rct.X, (float)rct.Y, (float)rct.W, (float)rct.H);
			//	m_graphicsContext.SetClip(path);
			//}

			////--------------------------------------------------------------------------------------------------------

			//public void setClip(List<LTPoint> pointList)
			//{

			//	PointF[] fpts = new PointF[pointList.Count];
			//	for (int c = 0; c < pointList.Count; c++)
			//	{
			//		fpts[c] = new PointF((float)pointList[c].X, (float)pointList[c].Y);
			//	}

			//	PdfPath path = new PdfPath();
			//	path.AddPolygon(fpts);

			//	m_graphicsContext.SetClip(path, PdfFillMode.Winding);

			//}

			//--------------------------------------------------------------------------------------------------------

			//public void setClip(LTPDFDrawingPath path)
			//{
			//	m_graphicsContext.SetClip(path.finishAndGetPDFPath());
			//}

			//--------------------------------------------------------------------------------------------------------
			//--  Methods for setting drawing parameters  ------------------------------------------------------------
			//--------------------------------------------------------------------------------------------------------

			//public void setBrushToColor(LTColor color)
			//{
			//	//byte alpha = color.A != 0 ? color.A : Convert.ToByte(1);
			//	//Syncfusion.Drawing.Color clr = Syncfusion.Drawing.Color.FromArgb(alpha, color.R, color.G, color.B);
			//	//m_currentBrush = new PdfSolidBrush(clr);

			//	Color clr = Color.FromArgb(color.A, color.R, color.G, color.B);
			//	m_currentBrush = new PdfSolidBrush(new PdfColor(clr));

			//}

			////--------------------------------------------------------------------------------------------------------

			//public void setStroke(LTColor color, float width)
			//{
			//	if (width > 0)
			//	{
			//		Syncfusion.Drawing.Color clr = Syncfusion.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
			//		m_currentPen = new PdfPen(clr, width);
			//	}
			//	else
			//	{
			//		m_currentPen = null;
			//	}
			//}

			////--------------------------------------------------------------------------------------------------------
			////--  Methods for drawing  -------------------------------------------------------------------------------
			////--------------------------------------------------------------------------------------------------------

			//public void drawRect(float x, float y, float wd, float ht)
			//{
			//	m_graphicsContext.DrawRectangle(m_currentPen, m_currentBrush, x, y, wd, ht);
			//}

			//public void drawRect(LTRect rct)
			//{
			//	drawRect((float)rct.X, (float)rct.Y, (float)rct.W, (float)rct.H);
			//}

			////--------------------------------------------------------------------------------------------------------

			//public void drawEllipse(float x, float y, float wd, float ht)
			//{
			//	m_graphicsContext.DrawEllipse(m_currentPen, m_currentBrush, x, y, wd, ht);
			//}

			////--------------------------------------------------------------------------------------------------------

			//public void drawPolygon(LTPoint[] points)
			//{
			//	PointF[] pts = new PointF[points.Length];
			//	for (int c = 0; c < points.Length; c++)
			//	{
			//		pts[c] = new PointF((float)points[c].X, (float)points[c].Y);
			//	}
			//	m_graphicsContext.DrawPolygon(m_currentPen, m_currentBrush, pts);
			//}

			//public void drawPath(LTPDFDrawingPath myPath, bool autoCloseFigure = true)
			//{
			//	PdfPath p = null;
			//	if (autoCloseFigure)
			//	{
			//		m_graphicsContext.DrawPath(m_currentPen, m_currentBrush, myPath.finishAndGetPDFPath());
			//		return;
			//	}

			//	m_graphicsContext.DrawPath(m_currentPen, null, myPath.toPdfPath());
			//}

			//--------------------------------------------------------------------------------------------------------

			//public void drawImage(Uri imageUri, LTRect imageRect)
			//{
			//	PdfImage img = new PdfBitmap(new System.IO.StreamReader(imageUri.LocalPath).BaseStream);
			//	m_graphicsContext.DrawImage(img, (float)imageRect.X, (float)imageRect.Y, (float)imageRect.W, (float)imageRect.H);
			//}

			//public async Task drawElement(UIElement element, LTRect elementRect)
			//{
			//	if (element == null)
			//	{
			//		return;
			//	}
			//	try
			//	{
			//		RenderTargetBitmap renderTarget = new RenderTargetBitmap();
			//		await renderTarget.RenderAsync(element);

			//		// Create SoftwareBitmap
			//		IBuffer pixelBuffer = await renderTarget.GetPixelsAsync();
			//		SoftwareBitmap sBitmap = SoftwareBitmap.CreateCopyFromBuffer(pixelBuffer, BitmapPixelFormat.Bgra8,
			//			renderTarget.PixelWidth, renderTarget.PixelHeight, BitmapAlphaMode.Premultiplied);

			//		// Create an encoder with the desired format
			//		IRandomAccessStream stream = new InMemoryRandomAccessStream();
			//		BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

			//		// Set the software bitmap
			//		encoder.SetSoftwareBitmap(sBitmap);
			//		await encoder.FlushAsync();

			//		// Draw image in context
			//		PdfImage img = new PdfBitmap(stream.AsStreamForRead());
			//		m_graphicsContext.DrawImage(img, (float)elementRect.X, (float)elementRect.Y, (float)elementRect.W, (float)elementRect.H);
			//	}
			//	catch (Exception ex)
			//	{
			//		s_log.ErrorFormat("DrawElement: an error occurred.\n{0}", ex.Message);
			//	}
			//}

			////--------------------------------------------------------------------------------------------------------

			//public async Task drawImage(Uri imageUri, LTRect imageRect, Windows.UI.Xaml.Shapes.Path mask)
			//{
			//	if (mask != null)
			//	{
			//		await internal_drawImageWithMask(imageUri, imageRect, mask);
			//	}

			//	else
			//	{
			//		drawImage(imageUri, imageRect);
			//	}
			//}

			////--------------------------------------------------------------------------------------------------------
			//public void drawBezierPath(List<LTPoint> bezierPathPoints)
			//{
			//	if (bezierPathPoints.Count < 4)
			//	{
			//		return;
			//	}
			//	m_graphicsContext.DrawBezier(m_currentPen,
			//		(float)bezierPathPoints[0].X, (float)bezierPathPoints[0].Y,
			//		(float)bezierPathPoints[1].X, (float)bezierPathPoints[1].Y,
			//		(float)bezierPathPoints[2].X, (float)bezierPathPoints[2].Y,
			//		(float)bezierPathPoints[3].X, (float)bezierPathPoints[3].Y);

			//}
			////--------------------------------------------------------------------------------------------------------

			//private async Task<bool> internal_drawImageWithMask(Uri imageUri, LTRect imageRect, Windows.UI.Xaml.Shapes.Path mask)
			//{

			//	if (mask != null && mask.Data is PathGeometry pathGeo)
			//	{

			//		//Open the original jpeg as a software bitmap.
			//		FileStream stream1 = new FileStream(imageUri.LocalPath, FileMode.OpenOrCreate);
			//		BitmapDecoder bd = await BitmapDecoder.CreateAsync(stream1.AsRandomAccessStream());
			//		SoftwareBitmap sb = await bd.GetSoftwareBitmapAsync();
			//		stream1.Close();

			//		//Make sure we have initialized the compositor and canvas device.
			//		//if (s_canvasDevice == null)
			//		//{
			//		//    s_compositor = Window.Current.Compositor;
			//		//    s_canvasDevice = CanvasDevice.GetSharedDevice();
			//		//}

			//		//Make the composition graphics device.
			//		//CompositionGraphicsDevice s_compositionGraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(s_compositor, s_canvasDevice);

			//		//Convert the software bitmap into a canvas bitmap (there's a direct method, but it doesn't work).
			//		byte[] imageBytes = new byte[4 * sb.PixelWidth * sb.PixelHeight];
			//		sb.CopyToBuffer(imageBytes.AsBuffer());
			//		DirectXPixelFormat format = DirectXPixelFormat.B8G8R8A8UIntNormalized;
			//		CanvasBitmap cb = CanvasBitmap.CreateFromBytes(Win2DUtils.CanvasDev, imageBytes, sb.PixelWidth, sb.PixelHeight, format);

			//		//Make a memory bitmap that we can draw to.
			//		CanvasRenderTarget rt = new CanvasRenderTarget(Win2DUtils.CanvasDev, sb.PixelWidth, sb.PixelHeight, cb.Dpi, DirectXPixelFormat.B8G8R8A8UIntNormalized, CanvasAlphaMode.Premultiplied);
			//		CanvasGeometry cg = pathGeo.ToCanvasGeometry();

			//		using (CanvasDrawingSession ses = rt.CreateDrawingSession())
			//		{
			//			//Fill in the path geometry with alpha=1.
			//			ses.FillGeometry(cg, Colors.Green);
			//			ses.DrawGeometry(cg, Colors.BlueViolet);
			//			//Draw the image on top with the right blend mode so only the parts with background alpha=1 are drawn.
			//			ses.DrawImage(cb, new Rect(0, 0, imageRect.W, imageRect.H),
			//				new Rect(0, 0, cb.SizeInPixels.Width, cb.SizeInPixels.Height), 1,
			//				CanvasImageInterpolation.HighQualityCubic, CanvasComposite.SourceIn);
			//		}

			//		//Make a memory stream where we can save our new memory bitmap.
			//		MemoryStream mems = new MemoryStream();
			//		await rt.SaveAsync(mems.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);

			//		PdfImage img = new PdfBitmap(mems);
			//		m_graphicsContext.DrawImage(img, (float)imageRect.X, (float)imageRect.Y, sb.PixelWidth, sb.PixelHeight);
			//		mems.Close();

			//	}

			//	return true;
			//}

			//--------------------------------------------------------------------------------------------------------
			//--  Methods combining and modifying PDFs.  -------------------------------------------------------------
			//--------------------------------------------------------------------------------------------------------

			public static LTPDFDrawingContext createFromPDFPaths(IList<string> paths)
			{
				//Load all the docs we want to merge.
				List<PdfLoadedDocument> pdfDocs = new List<PdfLoadedDocument>();
				List<PdfPageRotateAngle> rotationAngles = new List<PdfPageRotateAngle>();
				foreach (string path in paths)
				{
					PdfLoadedDocument ld = new PdfLoadedDocument(new System.IO.StreamReader(path).BaseStream);
					pdfDocs.Add(ld);

					foreach (PdfPageBase page in ld.Pages) { rotationAngles.Add(page.Rotation); }
				}

				//Make the destination, resulting document.
				PdfDocument resDoc = new PdfDocument();

				//Merge all the docs together into the destination.
				PdfDocumentBase.Merge(resDoc, pdfDocs.ToArray());

				//Make a drawing context object that houses this.
				LTPDFDrawingContext context = new LTPDFDrawingContext(resDoc);

				//Note the page rotation angles since they are messed up when we do a merge like this.
				context.m_retrievedRotationAngles = rotationAngles;

				return context;

			}

			//--------------------------------------------------------------------------------------------------------

			//public LTPDFDrawingAnnotation addHiglightAnnotation(int pageNbr, List<LTRect> rects, LTColor color, LTPoint popupPos, string popupText)
			//{
			//	PdfColor clr = new PdfColor(color.R, color.G, color.B);

			//	foreach (LTRect rct in rects)
			//	{
			//		Syncfusion.Pdf.Interactive.PdfTextMarkupAnnotation ma = new Syncfusion.Pdf.Interactive.PdfTextMarkupAnnotation();
			//		ma.TextMarkupColor = clr;
			//		ma.Location = new PointF((float)rct.X, (float)rct.Y);
			//		ma.Size = new SizeF((float)rct.W, (float)rct.H);
			//		ma.TextMarkupAnnotationType = Syncfusion.Pdf.Interactive.PdfTextMarkupAnnotationType.Highlight;
			//		m_pdfDoc.Pages[pageNbr].Annotations.Add(ma);
			//	}

			//	if (popupText != null)
			//	{
			//		Syncfusion.Pdf.Interactive.PdfPopupAnnotation pa = new Syncfusion.Pdf.Interactive.PdfPopupAnnotation(new RectangleF((float)popupPos.X, (float)popupPos.Y, 32, 32), popupText);
			//		m_pdfDoc.Pages[pageNbr].Annotations.Add(pa);
			//	}

			//	return new LTPDFDrawingAnnotation();
			//}

			////--------------------------------------------------------------------------------------------------------

			//public LTPDFDrawingAnnotation addPageLinkAnnotation(int inPageNbr, LTRect rect, int pointsToPage)
			//{
			//	Syncfusion.Pdf.Interactive.PdfLinkAnnotation la = new Syncfusion.Pdf.Interactive.PdfDocumentLinkAnnotation(new RectangleF((float)rect.X, (float)rect.Y, (float)rect.W, (float)rect.H), new Syncfusion.Pdf.Interactive.PdfDestination(m_pdfDoc.Pages[pointsToPage]));
			//	m_pdfDoc.Pages[inPageNbr].Annotations.Add(la);

			//	return new LTPDFDrawingAnnotation();
			//}

			////--------------------------------------------------------------------------------------------------------

			//public LTPDFDrawingAnnotation addPolygonAnnotation(int pageNbr, LTPoint[] points, LTColor fillColor, float opacity)
			//{
			//	this.prepareToDrawOnPage(pageNbr);
			//	LTColor color = new LTColor(fillColor.R, fillColor.G, fillColor.B, (int)(255.0 * opacity));
			//	this.setBrushToColor(fillColor);
			//	this.setStroke(LTColor.CLEAR_COLOR, 0);
			//	this.drawPolygon(points);

			//	return new LTPDFDrawingAnnotation();
			//}

			////--------------------------------------------------------------------------------------------------------

			//public void rotatePage(int pageNbr, int rotationDegrees)
			//{
			//	m_pdfDoc.Pages[pageNbr].Rotation = rotationDegrees == 0 ? PdfPageRotateAngle.RotateAngle0 : rotationDegrees == 90 ? PdfPageRotateAngle.RotateAngle90 : rotationDegrees == 180 ? PdfPageRotateAngle.RotateAngle180 : PdfPageRotateAngle.RotateAngle270;
			//}

			////--------------------------------------------------------------------------------------------------------

			//public LTRect pageRect(int page)
			//{
			//	SizeF sze = m_pdfDoc.Pages[page].Size;
			//	return new LTRect(0, 0, sze.Width, sze.Height);
			//}

			////--------------------------------------------------------------------------------------------------------

			//public int pageRotation(int page)
			//{
			//	//Get the value stored in the PDF. Will sometimes be wrong for merged PDFs.
			//	int r = m_pdfDoc.Pages[page].Rotation == PdfPageRotateAngle.RotateAngle0 ? 0 : m_pdfDoc.Pages[page].Rotation == PdfPageRotateAngle.RotateAngle180 ? 180 : m_pdfDoc.Pages[page].Rotation == PdfPageRotateAngle.RotateAngle270 ? 270 : 90;

			//	if (m_retrievedRotationAngles != null && m_retrievedRotationAngles.Count > page)
			//	{
			//		r += m_retrievedRotationAngles[page] == PdfPageRotateAngle.RotateAngle0 ? 0 : m_retrievedRotationAngles[page] == PdfPageRotateAngle.RotateAngle180 ? 180 : m_retrievedRotationAngles[page] == PdfPageRotateAngle.RotateAngle270 ? 270 : 90;
			//	}

			//	return r;
			//}

			//--------------------------------------------------------------------------------------------------------
			//--  Methods for outputting the results.  ---------------------------------------------------------------
			//--------------------------------------------------------------------------------------------------------

			public void saveAs(string path)
			{
				System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.OpenOrCreate);
				m_pdfDoc.Save(fs);
				fs.Close();
			}

			//--------------------------------------------------------------------------------------------------------

			public void closeDocument(bool closeCompletely)
			{
				m_pdfDoc.Close(closeCompletely);
			}

			//--------------------------------------------------------------------------------------------------------

			public int pageCount()
			{
				return m_pdfDoc.PageCount;
			}

			//--------------------------------------------------------------------------------------------------------



		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//internal class drawingContextState
		//{
		//	public Matrix transform = Matrix.Identity;
		//	public LTRect clip = Encoders.BuildLTRect(-double.MaxValue, -double.MaxValue, double.MaxValue, double.MaxValue);
		//}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class LTPDFDrawingPath
		//{
		//	PdfPath m_path;
		//	LTPoint lastPt = Encoders.POINT_ZERO;


		//	//-------------------------------------------------------------------------------------------------------------

		//	public LTPDFDrawingPath()
		//	{
		//		m_path = new PdfPath();
		//		m_path.StartFigure();
		//	}


		//	public LTPDFDrawingPath(LTPoint[] points) : this()
		//	{
		//		if (points.Length > 0)
		//		{
		//			//LTPDFDrawingPath strokePath = new LTPDFDrawingPath();
		//			moveTo(points.First());
		//			for (int i = 1; i < points.Length; i++)
		//			{
		//				addLine(points[i]);
		//			}
		//		}
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void startNewFigure()
		//	{
		//		m_path.StartFigure();
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void closeCurrentFigure()
		//	{
		//		m_path.CloseFigure();
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public PdfPath finishAndGetPDFPath()
		//	{
		//		m_path.CloseFigure();
		//		return m_path;
		//	}

		//	//-------------------------------------------------------------------------------------------------------------
		//	public PdfPath toPdfPath()
		//	{
		//		return m_path;
		//	}
		//	//-------------------------------------------------------------------------------------------------------------

		//	public void moveTo(LTPoint pt)
		//	{
		//		lastPt = pt;
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void addLine(LTPoint toPt)
		//	{
		//		m_path.AddLine(toPtF(lastPt), toPtF(toPt));
		//		lastPt = toPt;
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void addBezier(LTPoint controlPt1, LTPoint controlPt2, LTPoint endPt)
		//	{
		//		m_path.AddBezier(toPtF(lastPt), toPtF(controlPt1), toPtF(controlPt2), toPtF(endPt));
		//		lastPt = endPt;
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void addRect(LTRect rect)
		//	{
		//		m_path.AddRectangle(new RectangleF((float)rect.X, (float)rect.Y, (float)rect.W, (float)rect.H));
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void addArc(LTRect rect, double startAngle, double sweepAngle)
		//	{
		//		m_path.AddArc((float)rect.X, (float)rect.Y, (float)rect.W, (float)rect.H, (float)startAngle, (float)sweepAngle);
		//		lastPt = new LTPoint(m_path.LastPoint.X, m_path.LastPoint.Y);
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public PointF toPtF(LTPoint pt)
		//	{
		//		return new PointF((float)pt.X, (float)pt.Y);
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public void translate(float dx, float dy)
		//	{
		//		PointF[] origPts = m_path.PathPoints;
		//		byte[] origTypes = m_path.PathTypes;

		//		for (int c = 0; c < origPts.Length; c++)
		//		{
		//			PointF pt0 = origPts[c];
		//			origPts[c] = new PointF(pt0.X + dx, pt0.Y + dy);
		//		}

		//		PdfPath newPath = new PdfPath(origPts, origTypes);
		//		m_path = newPath;
		//	}

		//	//-------------------------------------------------------------------------------------------------------------

		//	public static LTPDFDrawingPath pathFromUWPPath(Windows.UI.Xaml.Shapes.Path uwpPath)
		//	{
		//		Geometry data = uwpPath?.Data;
		//		if (data == null)
		//			throw new Exception("Tried to convert unknown geometry type into an LTPDFDrawingPath");

		//		Transform ptTransform = new MatrixTransform();
		//		if (data.Transform != null)
		//			ptTransform = data.Transform;

		//		//If this is a rectangle geometry, then it's easy.
		//		if (data is RectangleGeometry rg)
		//		{
		//			LTPDFDrawingPath dp = new LTPDFDrawingPath();
		//			dp.addRect(ptTransform.TransformBounds(rg.Rect).ToLTRect());
		//			dp.finishAndGetPDFPath();
		//			return dp;
		//		}
		//		//If it's a path geometry, then it's harder!
		//		else if (data is PathGeometry pathGeoUWP)
		//		{
		//			//Make the new LTPDFdrawingPath object where we'll recreate the UWP path.
		//			LTPDFDrawingPath dp = new LTPDFDrawingPath();

		//			//Go through each "figure" in the path. I assume there should generally only be one for the kinds of paths we deal with.
		//			if (pathGeoUWP.Figures != null)
		//			{
		//				foreach (PathFigure pfUWP in pathGeoUWP.Figures)
		//				{
		//					//Tell our new path that we're starting a new figure.
		//					dp.startNewFigure();

		//					//Move to the start point for that figure.
		//					dp.moveTo(ptTransform.TransformPoint(pfUWP.StartPoint).ToLTPoint());

		//					//Now go through each segment of this figure and add it to the path we're building.
		//					foreach (PathSegment psUWP in pfUWP.Segments)
		//					{
		//						//Is this a line segment?
		//						if (psUWP is Windows.UI.Xaml.Media.LineSegment lsUWP)
		//						{
		//							dp.addLine(ptTransform.TransformPoint(lsUWP.Point).ToLTPoint());
		//						}

		//						//Is this a polyline segment?ParseTokenFragment
		//						else if (psUWP is Windows.UI.Xaml.Media.PolyLineSegment plsUWP)
		//						{
		//							foreach (Windows.Foundation.Point pt in plsUWP.Points) { dp.addLine(ptTransform.TransformPoint(pt).ToLTPoint()); }
		//						}

		//						//Is this a bezier curve segment?
		//						if (psUWP is Windows.UI.Xaml.Media.BezierSegment beUWP)
		//						{
		//							dp.addBezier(ptTransform.TransformPoint(beUWP.Point1).ToLTPoint(), ptTransform.TransformPoint(beUWP.Point2).ToLTPoint(), ptTransform.TransformPoint(beUWP.Point3).ToLTPoint());
		//						}

		//						//Is this a poly-bezier segment?
		//						else if (psUWP is Windows.UI.Xaml.Media.PolyBezierSegment pbUWP)
		//						{
		//							for (int c = 0; c < pbUWP.Points.Count; c += 3) { dp.addBezier(ptTransform.TransformPoint(pbUWP.Points[c]).ToLTPoint(), ptTransform.TransformPoint(pbUWP.Points[c + 1]).ToLTPoint(), ptTransform.TransformPoint(pbUWP.Points[c + 2]).ToLTPoint()); }
		//						}

		//						//Is this an arc segment? Raise an error: arcs are pretty hard because they are described very differently in UWP versus SyncFusion. 
		//						if (psUWP is Windows.UI.Xaml.Media.ArcSegment asUWP)
		//						{
		//							throw new Exception("Tried to draw arc in path--not yet supported - LTError.");
		//						}
		//					}

		//					//Say that we're done with this figure and close it if the original is closed.
		//					if (pfUWP.IsClosed)
		//						dp.closeCurrentFigure();
		//				}
		//			}

		//			//Now return the result.
		//			return dp;
		//		}
		//		else  //Otherwise, if this is some other kind of path, we don't yet support it.
		//		{
		//			throw new Exception("Tried to convert unknown geometry type into an LTPDFDrawingPath");
		//		}
		//	}
		//}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public class LTPDFDrawingAnnotation
		//{

		//}

		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//public enum LTfontStyle
		//{
		//	Regular = 0,
		//	Bold = 1,
		//	Italic = 2,
		//	Underline = 4
		//}

}
