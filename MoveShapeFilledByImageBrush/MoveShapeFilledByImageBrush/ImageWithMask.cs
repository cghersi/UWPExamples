using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace MoveShapeFilledByImageBrush
{
	public class ImageWithMask
	{
		private List<Point> m_maskInkStroke;
		private Shape m_renderImage;
		private Uri m_originalImageUri;
		private readonly Canvas m_parent;
		private Size m_desiredSize;

		public ImageWithMask(Canvas parentView, Uri imgUri, List<Point> maskInkStroke, Size desiredSize)
		{
			m_parent = parentView;
			m_originalImageUri = imgUri;
			m_desiredSize = desiredSize;
			if (!IsValidSize(m_desiredSize.Width))
				m_desiredSize.Width = 100;
			if (!IsValidSize(m_desiredSize.Height))
				m_desiredSize.Height = 30;
			SetMaskPath(maskInkStroke);
		}

		public Path MaskPath => m_renderImage as Path;

		public void SetImage(Uri imageUri, bool newMask = false)
		{
			// check if we are already in a good configuration:
			if (!newMask && (imageUri == m_originalImageUri))
				return;

			m_originalImageUri = imageUri;

			// check if there is a custom mask to apply:
			if (newMask)
			{
				bool imageAlreadyAttached = (m_renderImage != null) &&
					m_parent.Children.Contains(m_renderImage);
				if (m_maskInkStroke != null)
					m_renderImage = ToPath(m_maskInkStroke, m_desiredSize);
				else // otherwise we just paint a rectangle:
					m_renderImage = new Rectangle();

				if (!imageAlreadyAttached)
					m_parent.Children.Add(m_renderImage);
			}

			// create the new background brush, for the shape with the desired size:
			m_renderImage.Width = m_desiredSize.Width;
			m_renderImage.Height = m_desiredSize.Height;
			SetImageBrush();
		}

		private void SetImageBrush()
		{
			m_renderImage.Fill = new ImageBrush
			{
				ImageSource = new BitmapImage(m_originalImageUri)
			};
		}

		public void SetMaskPath(List<Point> maskStroke)
		{
			// check if we are already in a good configuration:
			if (maskStroke == null)
			{
				if (m_maskInkStroke == null)
					return;
			}
			else if (maskStroke.Equals(m_maskInkStroke))
				return;
			m_maskInkStroke = maskStroke;
			SetImage(m_originalImageUri, true);
		}

		public void SetFrame(double newX, double newY)
		{
			if (m_renderImage != null)
			{
				SetMargin(m_renderImage, newX, newY);
				//SetImageBrush();
			}
		}

		public static bool IsValidSize(double arg)
		{
			return (arg > 0) && !double.IsNaN(arg);
		}

		public static void SetMargin(FrameworkElement panel, double? left, double? top)
		{
			if (panel == null)
				return;
			Thickness margin = panel.Margin;
			if (top.HasValue && !double.IsNaN(top.Value))
				margin.Top = top.Value;
			if (left.HasValue && !double.IsNaN(left.Value))
				margin.Left = left.Value;
			panel.Margin = margin;
		}

		/// <summary>
		/// Transforms the given InkStroke into a Path shape.
		/// </summary>
		/// <param name="pl"></param>
		/// <param name="container">the size of the rectangle that contains this path</param>
		/// <returns></returns>
		public static Path ToPath(List<Point> pl, Size container)
		{
			if ((pl == null) || (pl.Count < 3))
				return null;

			double contW = container.Width;
			double contH = container.Height;

			PathGeometry pGeom = new PathGeometry();
			Point pt0Unit = pl[0];
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = new Point(pt0Unit.X * contW, pt0Unit.Y * contH)
			};

			// Add the points to the path:
			for (int c = 2; c < pl.Count; c += 2)
			{
				Point pt1Unit = pl[c - 1];
				Point pt1Self = new Point(pt1Unit.X * contW, pt1Unit.Y * contH);
				Point pt2Unit = pl[c];
				Point pt2Self = new Point(pt2Unit.X * contW, pt2Unit.Y * contH);
				pathFigure.Segments.Add(new QuadraticBezierSegment
				{
					Point1 = pt1Self,
					Point2 = pt2Self
				});
			}

			pGeom.Figures.Add(pathFigure);
			return new Path { Data = pGeom };
		}
	}
}
