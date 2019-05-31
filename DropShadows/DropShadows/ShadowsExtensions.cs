//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Composition;
// ReSharper disable ArrangeAccessorOwnerBody

namespace DropShadows
{
	/// <summary>
	/// Provides a Basic Rectangular DropShadow effect on a UIElement via an Attached Property.
	/// Must be in a parent container that is larger than the provide element to cast a shadow on.
	/// Note: Doesn't provide animation support. https://docs.microsoft.com/en-us/windows/uwp/composition/composition-shadows#animating
	/// </summary>
	public static class ShadowExtensions
	{
		private const int DEFAULT_SIZE = 1000;
		private const float BLUR_AMOUNT = 5.0f;
		private static readonly Matrix3x2 OFFSET_FOR_BLUR = Matrix3x2.CreateTranslation(new Vector2(2.0f * BLUR_AMOUNT, 2.0f * BLUR_AMOUNT));

		private static readonly Vector3 SHADOW_OFFSET = new Vector3(2.0f, 2.0f, 0.0f);
		public static readonly Vector3 DEFAULT_OFFSET = new Vector3 { X = 0, Y = 0, Z = 0 }; 

		private static readonly CanvasDevice s_canvasDevice = CanvasDevice.GetSharedDevice();

		private static readonly Compositor s_compositor = Window.Current.Compositor;

		private static readonly CompositionGraphicsDevice s_compositionGraphicsDevice =
			CanvasComposition.CreateCompositionGraphicsDevice(s_compositor, s_canvasDevice);

		private static readonly float ADJUST_SIZE = 1.25f;

		public static float CurrentZoom { get; set; } = 1f;

		#region Public API
		///////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Decides whether to show / hide the shadow for the given element
		/// </summary>
		/// <param name="excView">target of the shadow</param>
		/// <param name="showShadow">true to show the shadow, false to hide</param>
		/// <param name="shadowContainer">Panel that should actually host the shadow visual objects</param>
		/// <param name="shadowOptions">graphical options to draw the shadow</param>
		/// <returns>returns true if the <see cref="shadowOptions"/> has been updated and needs to be
		/// saved in the dictionary of shadow states; false otherwise</returns>
		public static bool ManageShadow(this IShadowPresenter excView, bool showShadow, Panel shadowContainer,
			ref DropShadowMetadata shadowOptions)
		{
			// decide whether to create or hide the shadow:
			if (showShadow)
			{
				// ok, we need to show the shadow; let's check the current status of what we are showing:
				if (shadowOptions.ShowShadow)
				{
					// just change the appearance, if needed:
					if (excView.View.ShadowNeedsChange(shadowOptions, excView.ShapeForShadow))
						excView.View.ChangeFullDropShadow(shadowContainer, shadowOptions);
				}
				else
				{
					// recreate the shadow:
					shadowOptions.Shape = excView.ShapeForShadow;
					excView.View.AddFullDropShadow(shadowContainer, ref shadowOptions);
					return true;
				}
			}
			else
			{
				// remove the graphical elements of the shadow:
				shadowContainer.RemoveFullDropShadow(ref shadowOptions);
			}

			return false;
		}

		private static void RemoveFullDropShadow(this Panel element, ref DropShadowMetadata options)
		{
			// TODO requires LiquidText infrastructure here...
			//options.HostElem.RemoveFromSuperview(element);
			//options.MaskPanel.RemoveFromSuperview(element);
			options.ShowShadow = false;
			
		}

		#endregion Public API
		///////////////////////////////////////////////////////////////////////////

		#region Private Render Shadow methods
		///////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Determines whether the shadow needs to be re-drawn
		/// </summary>
		/// <param name="element">target of the shadow</param>
		/// <param name="options">options to draw the shadow</param>
		/// <param name="elemShape">optional custom path that the shadow should follow</param>
		/// <returns>true if the shadow needs to be re-drawn</returns>
		private static bool ShadowNeedsChange(this FrameworkElement element, DropShadowMetadata options, Path elemShape)
		{
			// check if a shadow has been ever created at all for this element:
			if ((options.HostElem == null) || (options.MaskPanel == null))
				return true;

			// compare the shapes:
			if (CompareShapes(options.Shape, elemShape))
				return true;

			// compare the size of the element:
			if (!element.Width.Equal(options.HostElem.Width) ||
					!element.Height.Equal(options.HostElem.Height))
				return true;

			// compare the position of the element, if the shadow is hosted by the parent:
			if (options.HostInParent &&
					(!options.MarginLeft.Equal(element.Margin.Left) || !options.MarginTop.Equal(element.Margin.Top)))
				return true;

			// check the zoom:
			return !options.Zoom.Equal(CurrentZoom);
		}

		private static void AddFullDropShadow(this Panel element, Panel parent, ref DropShadowMetadata options)
		{
			//if (parent?.FindName(element.Name) == null)
			//	element.Loaded += (sender, args) => AddFullDropShadowInternal(element, parent, options, false);
			//else
			options.HostInParent = (element != parent);
			AddFullDropShadowInternal(element, parent, ref options, true);
		}

		private static void ChangeFullDropShadow(this Panel element, Panel parent, DropShadowMetadata options)
		{

			// make sure we have the graphical elements (defensive):
			if ((options.HostElem == null) || (options.MaskPanel == null)) // || (options.VisualShadow == null))
			{
				CreateGraphicalElements(element, parent, options);
				if ((options.HostElem == null) || (options.MaskPanel == null)) // || (options.VisualShadow == null))
					return; // very defensive!!
			}

			//if (!(options.VisualShadow?.Shadow is DropShadow shadow))
			//	return; // defensive!!

			float elemWidth = (float)element.Width;
			float elemHeight = (float)element.Height;
			

			// Create a drawing-surface brush what we can use as a mask for the shadow.
			// Draw the shadow mask into our new drawing surface (which in turn sets the brush with which we paint our visual):
			// TODO: consider using the default option for the shadow rather than setting always the mask, to increase performances.
			//if (options.Shape == null)
			//{
			//	options.VisualShadow.Size = new Vector2(elemWidth * ADJUST_SIZE, elemHeight * ADJUST_SIZE);
			//	shadow.Mask = null;
			//}
			//else
			//{
			CompositionDrawingSurface drawingSurface = DrawShadow(elemWidth, elemHeight, options);
			if (drawingSurface == null)
				return; //defensive

			//// Set the shadow mask to be a new "brush" that fills with our drawing surface:
			//shadow.Mask = s_compositor.CreateSurfaceBrush(drawingSurface);
			////}

			// change the size of the hosting elements:
			options.HostElem.Width = elemWidth;
			options.HostElem.Height = elemHeight;
			options.MaskPanel.Width = elemWidth;
			options.MaskPanel.Height = elemHeight;

			// change the margin of the hosting elements:
			if (options.HostInParent)
			{
				// for debug purpose, we can add +300 to element.Margin.Left to show the shadow on the right of the element
				double marginLeft = element.Margin.Left;
				double marginTop = element.Margin.Top;
				Thickness newMargin = new Thickness(marginLeft, marginTop, 0, 0);

				options.HostElem.Margin = newMargin;
				options.MaskPanel.Margin = newMargin;
				options.MarginLeft = marginLeft;
				options.MarginTop = marginTop;
			}

			// update the resulting shadow metadata:
			options.ShowShadow = true;
			options.Zoom = CurrentZoom;
			
		}

		private static void AddFullDropShadowInternal(Panel element, Panel parent, ref DropShadowMetadata options,
			bool alreadyChecked)
		{
			// integrity checks:
			if (!alreadyChecked)
			{
				if (parent == null)
					parent = element.Parent as Panel;
				if (parent == null)
					return; // cannot do anything here...
			}

			float elemWidth = (float)element.Width;
			float elemHeight = (float)element.Height;
		

			// Create a child host for shadow to insert in parent next to / behind element:
			DropShadow shadow = CreateGraphicalElements(element, parent, options);

			// Create a drawing-surface brush what we can use as a mask for the shadow.
			// Draw the shadow mask into our new drawing surface (which in turn sets the brush with which we paint our visual):
			CompositionDrawingSurface drawingSurface = DrawShadow(elemWidth, elemHeight, options);
			if (drawingSurface == null)
				return; //defensive

			// Set the shadow mask to be a new "brush" that fills with our drawing surface:
			//shadow.Mask = s_compositor.CreateSurfaceBrush(drawingSurface);

			AssignDrawingSurface(drawingSurface, options.HostElem);

			// update the resulting shadow metadata:
			options.ShowShadow = true;
			options.Zoom = CurrentZoom;
		}

		private static void AssignDrawingSurface(CompositionDrawingSurface drawingSurface, UIElement element)
		{
			Visual gridVisual = ElementCompositionPreview.GetElementVisual(element);
			Compositor compositor = gridVisual.Compositor;
			SpriteVisual shadowVisual = compositor.CreateSpriteVisual();
			CompositionSurfaceBrush surfaceBrush = compositor.CreateSurfaceBrush();
			surfaceBrush.Surface = drawingSurface;
			Size surfaceSize = drawingSurface.Size;
			shadowVisual.Offset = SHADOW_OFFSET;
			shadowVisual.Brush = surfaceBrush;
			shadowVisual.Size = new Vector2((float)surfaceSize.Width, (float)surfaceSize.Height);
			ElementCompositionPreview.SetElementChildVisual(element, shadowVisual);
		}

		private static DropShadow CreateGraphicalElements(Panel element, Panel parent, DropShadowMetadata options)
		{
			double elemWidth = element.Width;
			double elemHeight = element.Height;

			options.HostElem = new ShadowHost(elemWidth, elemHeight)
				{ Name = "Host_" + element.Name };
			options.MaskPanel = new ShadowMaskPanel(elemWidth, elemHeight, element.Background)
				{ Name = "MaskPanel_" + element.Name };

			if (options.HostInParent)
			{
				options.HostElem.Margin = element.Margin;
				options.MaskPanel.Margin = element.Margin;
				options.MarginLeft = element.Margin.Left;
				options.MarginTop = element.Margin.Top;
				int idx = parent.Children.IndexOf(element);
				parent.Children.Insert(idx, options.HostElem);
				parent.Children.Insert(idx + 1, options.MaskPanel);
			}
			else
			{
				element.Children.Insert(0, options.HostElem);
				element.Children.Insert(1, options.MaskPanel);
			}

			// Create LayerVisual, with a size that should be equal to the size of the drawing surface,
			// adjusted with a coefficient to make the shadows of the right size, with respect to the target element: 
			// TODO: it is not yet perfectly clear how to programmatically retrieve such coefficient,
			// a possible option is to check for the native resolution of the screen vs. the actual one.
			// For now, it can be tuned in configuration, in ltSettings.xml file, "ResolutionAdjustFactor" key.
			//options.VisualShadow = s_compositor.CreateLayerVisual();
			//float dimension = DEFAULT_SIZE * ADJUST_SIZE;
			//options.VisualShadow.Size = new Vector2(dimension, dimension);

			//// Create DropShadow
			//DropShadow shadow = s_compositor.CreateDropShadow();
			//shadow.Color = options.Color;
			//shadow.Offset = options.Offset;
			//shadow.BlurRadius = options.BlurRadius;
			//shadow.SourcePolicy = CompositionDropShadowSourcePolicy.Default;

			//// Associate Shadow with LayerVisual:
			//options.VisualShadow.Shadow = shadow;

			//// Inject the visual as a child of the grid:
			//ElementCompositionPreview.SetElementChildVisual(options.HostElem, options.VisualShadow);

			return null; //shadow;
		}

		private static CompositionDrawingSurface DrawShadow(float elemWidth, float elemHeight, DropShadowMetadata options)
		{
			// Make the drawing surface we can draw the shadow mask onto:
			//CompositionDrawingSurface drawingSurface = s_compositionGraphicsDevice.CreateDrawingSurface(
			//	new Size(DEFAULT_SIZE, DEFAULT_SIZE), DirectXPixelFormat.B8G8R8A8UIntNormalized,
			//	DirectXAlphaMode.Premultiplied);

			CanvasCommandList cl = new CanvasCommandList(s_canvasDevice);
			using (CanvasDrawingSession ds = cl.CreateDrawingSession())
				//using (CanvasDrawingSession ds = CanvasComposition.CreateDrawingSession(drawingSurface))
			{
				ds.Clear(Colors.Transparent);
				CanvasPathBuilder pathBuilder = new CanvasPathBuilder(s_canvasDevice);
				if ((options.Shape == null) || !(options.Shape.Data is PathGeometry pathGeom) || (pathGeom.Figures.Count != 1))
				{
					// consider using the default rectangular shape instead of a mask here for performance:
					//https://docs.microsoft.com/gl-es/windows/uwp/composition/composition-shadows?view=azurebatch-7.0.0

					// default choice, a rectangle:
					const float minX = 0;
					float maxX = (elemWidth + 2 * options.Offset.X) * CurrentZoom;
					const float minY = 0;
					float maxY = (elemHeight + 2 * options.Offset.Y) * CurrentZoom;
					//float minX = -options.Offset.X * CurrentZoom;
					//float maxX = (elemWidth + options.Offset.X) * CurrentZoom;
					//float minY = -options.Offset.Y * CurrentZoom;
					//float maxY = (elemHeight + options.Offset.Y) * CurrentZoom;

					pathBuilder.BeginFigure(minX, minY);
					pathBuilder.AddLine(minX, maxY);
					pathBuilder.AddLine(maxX, maxY);
					pathBuilder.AddLine(maxX, minY);
					pathBuilder.EndFigure(CanvasFigureLoop.Closed);
				}
				else
				{
					options.MaskPanel.Background = new SolidColorBrush(Colors.Transparent); // force transparent mask

					// here we assume "simple" Paths with only 1 Figure, as we create elsewhere in the code for the kind of shapes we use in LiquidText:
					PathFigure figure = pathGeom.Figures[0];
					if (figure == null)
						return null; // defensive

					// compute the negative offset, if any, in order not to have a "cut" shape:
					Vector2 negOffset = ComputeNegativeOffset(figure);

					// follow the path, recreating the exact shape:
					Vector2 start = ToVec2(figure.StartPoint, negOffset);
					pathBuilder.BeginFigure(start);
					foreach (PathSegment segment in figure.Segments)
					{
						switch (segment)
						{
							case LineSegment line:
								Vector2 endLine = ToVec2(line.Point, negOffset);
								pathBuilder.AddLine(endLine);
								//s_log.DebugFormat("Line: from {0} to {1}; Path from {2} to {3}. Neg Offset is {4}", 
								//	start, endLine, figure.StartPoint, line.Point, negOffset);
								break;
							case BezierSegment cubicBez:
								Vector2 cp1 = ToVec2(cubicBez.Point1, negOffset);
								Vector2 cp2 = ToVec2(cubicBez.Point2, negOffset);
								Vector2 endCubic = ToVec2(cubicBez.Point3, negOffset);
								pathBuilder.AddCubicBezier(cp1, cp2, endCubic);
								//s_log.DebugFormat("Cubic: start {0}, cp {1} and {2}, end {3}; Path start {4}, cp {5} and {6}, end {7}. Neg Offset {8}",
								//	start, cp1, cp2, endCubic, figure.StartPoint, cubicBez.Point1, cubicBez.Point2, cubicBez.Point3, negOffset);
								break;
							case QuadraticBezierSegment quadBez:
								Vector2 cp = ToVec2(quadBez.Point1, negOffset);
								Vector2 endQuad = ToVec2(quadBez.Point2, negOffset);
								pathBuilder.AddQuadraticBezier(cp, endQuad);
								//s_log.DebugFormat("Quad: start {0}, cp {1}, end {2}; Path start {3}, cp {4}, end {5}. Neg Offset {6}",
								//	start, cp, endQuad, figure.StartPoint, quadBez.Point1, quadBez.Point2, negOffset);
								break;

						}
					}

					pathBuilder.EndFigure(CanvasFigureLoop.Closed);
				}

				ds.FillGeometry(CanvasGeometry.CreatePath(pathBuilder), Colors.Black);
			}

			// Make the drawing surface we can draw the shadow mask onto:
			CompositionDrawingSurface drawingSurface = s_compositionGraphicsDevice.CreateDrawingSurface(
				new Size(DEFAULT_SIZE, DEFAULT_SIZE), DirectXPixelFormat.B8G8R8A8UIntNormalized,
				DirectXAlphaMode.Premultiplied);

			GaussianBlurEffect blurEffect = new GaussianBlurEffect
			{
				BlurAmount = BLUR_AMOUNT,
				Source = cl
			};

			using (CanvasDrawingSession ds = CanvasComposition.CreateDrawingSession(drawingSurface))
			{
				ds.Transform = OFFSET_FOR_BLUR;
				ds.DrawImage(blurEffect);
			}

			return drawingSurface;
		}

		#endregion Private Render Shadow methods
		///////////////////////////////////////////////////////////////////////////

		#region Private Utilities
		///////////////////////////////////////////////////////////////////////////
		private static bool Equal(this double arg1, double arg2)
		{
			return Math.Abs(arg1 - arg2) < 0.01;
		}
		private static bool Equal(this float arg1, float arg2)
		{
			return Math.Abs(arg1 - arg2) < 0.01;
		}

		private static Vector2 ComputeNegativeOffset(PathFigure figure)
		{
			List<Vector2> points = new List<Vector2>
			{
				ToVec2(figure.StartPoint)
			};
			foreach (PathSegment segment in figure.Segments)
			{
				switch (segment)
				{
					case LineSegment line:
						points.Add(ToVec2(line.Point));
						break;
					case BezierSegment cubicBez:
						points.Add(ToVec2(cubicBez.Point1));
						points.Add(ToVec2(cubicBez.Point2));
						points.Add(ToVec2(cubicBez.Point3));
						break;
					case QuadraticBezierSegment quadBez:
						points.Add(ToVec2(quadBez.Point1));
						points.Add(ToVec2(quadBez.Point2));
						break;
				}
			}

			return ComputeNegativeOffset(points);
		}

		private static Vector2 ComputeNegativeOffset(IEnumerable<Vector2> points)
		{
			Vector2 res = new Vector2(float.MaxValue, float.MaxValue);
			foreach (Vector2 p in points)
			{
				if (res.X > p.X)
					res.X = p.X;
				if (res.Y > p.Y)
					res.Y = p.Y;
			}

			return res;
		}

		/// <summary>
		/// Checks if the given shapes are different
		/// </summary>
		/// <param name="shape1"></param>
		/// <param name="shape2"></param>
		/// <returns>true if the shapes are different</returns>
		private static bool CompareShapes(Path shape1, Path shape2)
		{
			if (shape1 == null)
				return shape2 != null;
			else if (shape2 == null)
				return true;
			return !shape1.Equals(shape2);
		}

		private static Vector2 ToVec2(Point p, Vector2 negOffset)
		{
			return new Vector2((float)p.X * CurrentZoom - negOffset.X,
				(float)p.Y * CurrentZoom - negOffset.Y);
		}

		private static Vector2 ToVec2(Point p)
		{
			return new Vector2((float)p.X * CurrentZoom, (float)p.Y * CurrentZoom);
		}

		#endregion Private Utilities
		///////////////////////////////////////////////////////////////////////////
	}

	public class ShadowHost : FrameworkElement
	{
		public ShadowHost(double width, double height) : base()
		{
			Width = width;
			Height = height;
		}
	}

	public class ShadowMaskPanel : Panel
	{
		public ShadowMaskPanel(double width, double height, Brush background) : base()
		{
			Width = width;
			Height = height;
			Background = background;
			// Background = UWPUtils.CLEAR_COLOR; for debug purpose, to show the shadow without the mask
		}
	}

	public class DropShadowMetadata
	{
		public DropShadowMetadata(Path shadowShape)
		{
			Shape = shadowShape;
		}

		/// <summary>
		/// Defines whether the shadow is being displayed or not
		/// </summary>
		public bool ShowShadow { get; set; } = false;

		public float BlurRadius { get; set; } = 15;
		public Color Color { get; set; } = Colors.Black;
		public Vector3 Offset { get; set; } = ShadowExtensions.DEFAULT_OFFSET;
		public double Opacity { get; set; } = 0.3;

		/// <summary>
		/// Determines if the <see cref="HostElem"/> and <see cref="MaskPanel"/> elements
		/// should be attached as children of the target element (HostInParent = false),
		/// or of the parent of the target element (HostInParent = true).
		/// </summary>
		public bool HostInParent { get; set; } = false;

		/// <summary>
		/// If the <see cref="HostElem"/> and <see cref="MaskPanel"/> elements are hosted
		/// in the parent of the target, it defines the margin from the left of the parent container.
		/// </summary>
		public double MarginLeft { get; set; } = 0;

		/// <summary>
		/// If the <see cref="HostElem"/> and <see cref="MaskPanel"/> elements are hosted
		/// in the parent of the target, it defines the margin from the top of the parent container.
		/// </summary>
		public double MarginTop { get; set; } = 0;

		/// <summary>
		/// Level of Zoom adopted to draw the shadow
		/// </summary>
		public float Zoom { get; set; }

		/// <summary>
		/// Null if the shadow should be rectangular; otherwise, it defines the custom Path that
		/// the shadow should follow.
		/// </summary>
		public Path Shape { get; set; }

		/// <summary>
		/// FrameworkElement that hosts the Visual object of the shadow <see cref="VisualShadow"/>.
		/// </summary>
		public ShadowHost HostElem { get; set; }

		/// <summary>
		/// A Panel used to place the shadow in the background (ZIndex = -1).
		/// </summary>
		public ShadowMaskPanel MaskPanel { get; set; }

		/// <summary>
		/// The actual VisualLayer that draws the shadow.
		/// </summary>
		public LayerVisual VisualShadow { get; set; }

		public override string ToString()
		{
			return string.Format("left:{0}, top:{1}, zoom:{2}, hostInParent:{3}", MarginLeft, MarginTop, Zoom, HostInParent);
		}
	}

	public interface IShadowPresenter
	{
		Path ShapeForShadow { get; }

		Panel View { get; }
	}
}
