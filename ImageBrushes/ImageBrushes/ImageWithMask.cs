using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

namespace ImageBrushes
{
	public class ImageWithMask
	{
		private Path m_renderImage;
		private readonly Panel m_parent;
		private Uri m_imageUri;

		public ImageWithMask(Panel excView, Size desiredSize)
		{
			m_parent = excView;
			CurrentSize = desiredSize;
		}

		public Size CurrentSize { get; set; }

		public Path MaskPath => m_renderImage;

		public void SetImage(Uri imageUri)
		{
			m_imageUri = imageUri;
			m_renderImage = BuildRectPath(CurrentSize);
			m_parent.Children.Add(m_renderImage);

			m_renderImage.Width = CurrentSize.Width;
			m_renderImage.Height = CurrentSize.Height;
			m_renderImage.Fill = new ImageBrush
			{
				ImageSource = new BitmapImage(m_imageUri),
				Stretch = Stretch.Fill
			};
		}

		public void Resize(double newW, double newH)
		{
			CurrentSize = new Size(Math.Max(20, CurrentSize.Width + newW), 
				Math.Max(20, CurrentSize.Height + newH));
			m_renderImage.Width = CurrentSize.Width;
			m_renderImage.Height = CurrentSize.Height;
			PathFigure pathFigure = (m_renderImage.Data as PathGeometry)?.Figures.FirstOrDefault();
			if (pathFigure != null)
			{
				pathFigure.Segments.Clear();
				RefreshShape(pathFigure, CurrentSize);
			}
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

		private static Path BuildRectPath(Size size)
		{
			PathGeometry pGeom = new PathGeometry();
			PathFigure pathFigure = new PathFigure
			{
				StartPoint = new Point(0, 0),
				IsClosed = true
			};

			RefreshShape(pathFigure, size);

			pGeom.Figures.Add(pathFigure);
			return new Path { Data = pGeom };
		}

		private static void RefreshShape(PathFigure pathFigure, Size size)
		{
			double contW = size.Width;
			double contH = size.Height;

			// Add the points to the path:
			pathFigure.Segments.Add(new LineSegment { Point = new Point(contW, 0) });
			pathFigure.Segments.Add(new LineSegment { Point = new Point(contW, contH) });
			pathFigure.Segments.Add(new LineSegment { Point = new Point(0, contH) });
		}
	}
}
