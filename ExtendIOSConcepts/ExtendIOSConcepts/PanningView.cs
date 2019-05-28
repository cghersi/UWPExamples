using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace ExtendIOSConcepts
{
	public class PanningView : BaseView
	{
		private static int s_count = 0;

		public static float CurrentZoom { get; set; }

		private readonly ScrollViewer m_scrollView = new ScrollViewer
		{
			Name = "ScrollViewerForWrkPage",
			MaxZoomFactor = 5,      //UWP allows at least 5000...
			MinZoomFactor = 0.1f,   //UWP doesn't allow less than 0.1
			HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
			VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
			IsHorizontalRailEnabled = true,
			IsVerticalRailEnabled = true,
			ZoomMode = ZoomMode.Enabled,
			IsScrollInertiaEnabled = true,
			IsZoomInertiaEnabled = true,
			AllowDrop = true
		};

		/// <summary>
		/// The main container view for the page (it sits in the scroll view, and also provides the gradient background).
		/// </summary>
		private readonly Canvas m_zoomView = new Canvas
		{
			Name = "ZoomViewForWrkPage",
			Width = 100000,
			Height = 100000,
			IsDoubleTapEnabled = true,
			Background = new SolidColorBrush(Colors.Aqua)  //this must be set for the manipulation events to be fired!!
		};


		public override Panel ParentForChildren => m_zoomView;

		private bool m_childIsMoving = false;

		private readonly CompositeTransform m_zoomViewTransform = new CompositeTransform();

		public PanningView() : base(true)
		{
			SetArrangeOverride(s =>
			{
				LayoutSubviews(s);
				return s;
			});

			m_scrollView.Content = m_zoomView;
			m_zoomView.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia |
			                              ManipulationModes.Scale;
			m_zoomView.ManipulationStarted += OnManipulationStarted;
			m_zoomView.ManipulationDelta += OnManipulationDelta;
			m_zoomView.ManipulationCompleted += ScrollViewDidEndDecelerating;
			m_zoomView.PointerReleased += OnPointerUp;
			m_zoomView.RenderTransform = m_zoomViewTransform;
			m_zoomView.PointerWheelChanged += OnPointerWheelChanged;
			m_scrollView.PointerWheelChanged += OnPointerWheelChanged;
			
			View.Children.Add(m_scrollView);

			CurrentZoom = 1;
		}

		private void OnPointerUp(object sender, PointerRoutedEventArgs e)
		{
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
		}

		private void OnPointerWheelChanged(object sender, PointerRoutedEventArgs e)
		{
			PointerPoint pp = e.GetCurrentPoint(m_zoomView);
			HandleZoomWithWheel(pp);
			e.Handled = true;
		}

		private void HandleZoomWithWheel(PointerPoint pp)
		{
			if (pp == null)
				return;

			Point center = pp.Position;
			if ((center.X < 0) || (center.Y < 0))
				return; // we are not in zoomView!!

			float deltaScroll = (pp.Properties.MouseWheelDelta > 0) ? 1.05f : 0.95f;

			// set transform for zoom:
			ZoomWorkspaceAround(deltaScroll, center);
		}

		private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		{
			if (m_childIsMoving || e.Handled)
				return;
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeAll, 0);
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			if (m_childIsMoving || e.Handled)
				return;

			// handle zoom for pinch & stretch:
			if (!e.Cumulative.Expansion.IsZero() && (Math.Abs(e.Velocities.Expansion) > 0.03))
			{
				// check that we are not in zoomView:
				Point center = e.Position;
				if ((center.X >= 0) && (center.Y >= 0))
				{
					float deltaScroll = (e.Cumulative.Expansion > 0) ? 1.01f : 0.99f;

					// set transform for zoom:
					ZoomWorkspaceAround(deltaScroll, center);
				}
			}

			SetEffectiveOffsetOfScrollView(new Point(m_zoomViewTransform.TranslateX + e.Delta.Translation.X,
				m_zoomViewTransform.TranslateY + e.Delta.Translation.Y), null);
			e.Handled = true;
		}

		private void ScrollViewDidEndDecelerating(object sender, ManipulationCompletedRoutedEventArgs e)
		{
			if (m_childIsMoving || e.Handled)
				return;
			RequestNewLayout(); 
			e.Handled = true;
		}

		private void SetEffectiveOffsetOfScrollView(Point newOffset, Action whenDone)
		{
			m_zoomViewTransform.TranslateX = newOffset.X;
			m_zoomViewTransform.TranslateY = newOffset.Y;
			whenDone?.Invoke();
		}

		private void ZoomWorkspaceAround(float deltaZoom, Point center)
		{
			if (deltaZoom.IsZero() || deltaZoom.Equals(1))
				return;

			// remember the current transformation matrix:
			Matrix currentMatrix = TransformMatrix();

			// set the new zoom:
			float newZoom = CurrentZoom * deltaZoom;
			SetWorkspaceZoom(newZoom);

			// set the new offset:
			double curOffsetX = center.X * (1 - deltaZoom);
			double curOffsetY = center.Y * (1 - deltaZoom);
			double newOffsetX = curOffsetX + (currentMatrix.OffsetX * deltaZoom);
			double newOffsetY = curOffsetY + (currentMatrix.OffsetY * deltaZoom);
			SetWorkspacePositionOffset(new Point(newOffsetX, newOffsetY));
		}

		private void SetWorkspaceZoom(float newZoom)
		{
			// Should be simple--just change the zoom level:
			if (!newZoom.Equal(CurrentZoom))
			{
					m_zoomViewTransform.ScaleX = newZoom;
					m_zoomViewTransform.ScaleY = newZoom;
					CurrentZoom = newZoom;
			}
		}

		private void SetWorkspacePositionOffset(Point newOffset)
		{
			// Now move to the right place:
			Point co = GetEffectiveOffsetOfScrollView();
			if (!co.Equals(newOffset))
				SetEffectiveOffsetOfScrollView(newOffset, null);
		}

		private Point GetEffectiveOffsetOfScrollView()
		{
			return new Point(-m_zoomViewTransform.TranslateX, -m_zoomViewTransform.TranslateY);
		}

		private Matrix TransformMatrix()
		{
			if ((m_zoomViewTransform.Inverse is MatrixTransform inverse) && (inverse.Inverse is MatrixTransform direct))
				return direct.Matrix;
			else
				return Matrix.Identity; //defensive
		}

		public override void LayoutSubviews(Size s)
		{
			Debug.WriteLine("{1} - In Panning.LayoutSubviews with size {0}", s, s_count++);

			m_scrollView.Arrange(Bounds);
		}
	}
}
