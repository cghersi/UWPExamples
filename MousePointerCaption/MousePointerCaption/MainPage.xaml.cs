using System;
using System.Diagnostics;
using System.Threading;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;


namespace MousePointerCaption
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public static float CurrentZoom { get; set; }

		public static RelativePanel MainPanel { get; private set; }

		private readonly ScrollViewer m_scrollView = new ScrollViewer()
		{
			Name = "ScrollViewerForWrkPage",
			MaxZoomFactor = 5,
			MinZoomFactor = 0.1f,
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
		/// The main container view for the page.
		/// </summary>
		private readonly Canvas m_zoomView = new Canvas()
		{
			Name = "ZoomViewForWrkPage",
			Width = 100000,
			Height = 100000,
			IsDoubleTapEnabled = true,
			Background = new SolidColorBrush(Colors.Yellow)
		};

		private readonly CompositeTransform m_zoomViewTransform = new CompositeTransform();

		private static bool s_floatingElemIsMoving = false;

		private readonly RelativePanel m_floatingElement = new RelativePanel()
		{
			Height = 40,
			Width = 80,
			Background = new SolidColorBrush(Colors.Aqua),
			Margin = new Thickness(20),
			CornerRadius = new CornerRadius(4)
		};

		private readonly RelativePanel m_fixedElement = new RelativePanel()
		{
			Height = 40,
			Width = 80,
			Background = new SolidColorBrush(Colors.Red),
			Margin = new Thickness(300, 300, 20, 20),
			CornerRadius = new CornerRadius(4)
		};

		public MainPage()
		{
			this.InitializeComponent();

			MainPanel = MainView;
			CurrentZoom = 1;

			// Initialize the data structures:
			m_scrollView.Content = m_zoomView;
			m_zoomView.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
			m_zoomView.ManipulationStarted += OnManipulationStarted;
			m_zoomView.ManipulationDelta += OnManipulationDelta;
			m_zoomView.PointerReleased += OnPointerUp;
			m_zoomView.RenderTransform = m_zoomViewTransform;
			m_zoomView.PointerWheelChanged += OnPointerWheelChanged;
			m_scrollView.PointerWheelChanged += OnPointerWheelChanged;
			MainView.Children.Add(m_scrollView);

			m_zoomView.Children.Add(m_floatingElement);
			Canvas.SetZIndex(m_floatingElement, 20);
			m_floatingElement.AttachEventManager(new FloatingPointerManager(m_floatingElement, m_fixedElement));

			m_zoomView.Children.Add(m_fixedElement);
			Canvas.SetZIndex(m_fixedElement, 10);
		}



		public static void SetFloatingElemIsMoving(bool isMoving)
		{
			s_floatingElemIsMoving = isMoving;
		}

		#region Event Handling methods
		///////////////////////////////////////////////////////////////////////////

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

		private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			if (s_floatingElemIsMoving)
				return;
			SetEffectiveOffsetOfScrollView(new Point(m_zoomViewTransform.TranslateX + e.Delta.Translation.X,
					m_zoomViewTransform.TranslateY + e.Delta.Translation.Y), null);
			e.Handled = true;
		}

		private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
		{
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeAll, 0);
		}

		/// <summary>
		/// This is called by the event handlers to adjust the zoom the workspace AROUND a specific center.
		/// It combines the effect of the zoom with a translation.
		/// </summary>
		/// <param name="deltaZoom"></param>
		/// <param name="center">center of scale (optional)</param>
		private void ZoomWorkspaceAround(float deltaZoom, Point center)
		{
			// remember the current transformation matrix:
			Matrix currentMatrix = TransformMatrix();
			double curZoom = currentMatrix.M11;

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

		private Matrix TransformMatrix()
		{
			if ((m_zoomViewTransform.Inverse is MatrixTransform inverse) && (inverse.Inverse is MatrixTransform direct))
				return direct.Matrix;
			else
				return Matrix.Identity; //defensive
		}

		private void SetWorkspacePositionOffset(Point newOffset)
		{
			// Now move to the right place:
			Point co = GetEffectiveOffsetOfScrollView();
			if (!co.Equal(newOffset))
				SetEffectiveOffsetOfScrollView(newOffset, null);

		}

		#endregion Event Handling methods
		///////////////////////////////////////////////////////////////////////////

		private Point GetEffectiveOffsetOfScrollView()
		{
			return new Point(-m_zoomViewTransform.TranslateX, -m_zoomViewTransform.TranslateY);
		}

		private void SetEffectiveOffsetOfScrollView(Point newOffset, Action whenDone)
		{
			m_zoomViewTransform.TranslateX = newOffset.X;
			m_zoomViewTransform.TranslateY = newOffset.Y;
			whenDone?.Invoke();
		}
	}

	public class FloatingPointerManager : BasePointerManager
	{
		private readonly RelativePanel m_fixedElem;
		private readonly RelativePanel m_floatingElem;

		public bool IsDraggingNow { get; set; }

		public FloatingPointerManager(RelativePanel floatingElem, RelativePanel fixedElem) : base("TestManager")
		{
			m_floatingElem = floatingElem;
			m_fixedElem = fixedElem;
			IsDraggingNow = false;
		}

		public override void OnPointerExited(object sender, PointerRoutedEventArgs e)
		{
			base.OnPointerExited(sender, e);
			if (!IsDraggingNow)
				MainPage.SetFloatingElemIsMoving(false);
		}

		protected override bool OnPointerMovedDelegate(Point dragVector, FrameworkElement sender,
				Point currentPoint, Pointer currentPointer)
		{
			Debug.WriteLine("Dragging by {0}-{1}", dragVector.X, dragVector.Y);

			// move the element:
			m_floatingElem.Move(dragVector.X, dragVector.Y);

			if (PointerIsInsideFixedElement(currentPoint))
			{
				// if we execute this action, the pointer capture is lost and the floating element is no more dragged,
				// i.e. the dragging action is interrupted:
				//LosePointerAction();

				// get reference of the current parent:
				Panel previousParent = m_floatingElem.Parent as Panel;
				//m_floatingElem.RemoveFromSuperview(previousParent);
				//MainPage.MainPanel.Children.Add(m_floatingElem);

				// create a new element and attach to the current parent:
				Canvas newParent = new Canvas()
				{
					Background = new SolidColorBrush(Colors.LightYellow),
					Margin = m_floatingElem.Margin,
					Width = m_floatingElem.Width + 20,
					Height = m_floatingElem.Height + 20
				};
				m_floatingElem.Margin = new Thickness(0);
				previousParent.Children.Add(newParent);

				// change parent to the new element:
				// NOTE: make sure we keep the capture on the pointer!!
				//m_floatingElem.RemoveFromSuperview(MainPage.MainPanel);
				m_floatingElem.RemoveFromSuperview(previousParent);
				newParent.Children.Add(m_floatingElem);
				m_floatingElem.CapturePointer(currentPointer);
			}

			// make sure the resize handle is positioned correctly:
			//m_excerptView.PositionResizeHandle();

			return base.OnPointerMovedDelegate(dragVector, sender, currentPoint, currentPointer);
		}

		/// <summary>
		/// This action makes the floating element lose the capture on the mouse pointer
		/// </summary>
		private void LosePointerAction()
		{
			// change parent of the element:
			Panel previousParent = m_floatingElem.Parent as Panel;
			m_floatingElem.RemoveFromSuperview(previousParent);
			MainPage.MainPanel.Children.Add(m_floatingElem);

			// create a new element and attach as parent:
			Canvas newParent = new Canvas()
			{
				Background = new SolidColorBrush(Colors.LightYellow),
				Margin = m_floatingElem.Margin,
				Width = m_floatingElem.Width + 20,
				Height = m_floatingElem.Height + 20
			};
			m_floatingElem.Margin = new Thickness(0);
			previousParent.Children.Add(newParent);

			// change parent again:
			m_floatingElem.RemoveFromSuperview(MainPage.MainPanel);
			newParent.Children.Add(m_floatingElem);
		}

		protected override bool OnPointerDownDelegate(PointerRoutedEventArgs e)
		{
			IsDraggingNow = true;
			MainPage.SetFloatingElemIsMoving(true);
			return false;
		}

		protected override bool OnPointerUpDelegate(PointerRoutedEventArgs e)
		{
			IsDraggingNow = false;
			MainPage.SetFloatingElemIsMoving(false);
			return false;
		}

		private bool PointerIsInsideFixedElement(Point curPointer)
		{
			double minX = m_fixedElem.Margin.Left * (1 / MainPage.CurrentZoom);
			double maxX = minX + m_fixedElem.Width * (1 / MainPage.CurrentZoom);
			double minY = m_fixedElem.Margin.Top * (1 / MainPage.CurrentZoom);
			double maxY = minY + m_fixedElem.Height * (1 / MainPage.CurrentZoom);

			return (curPointer.X > minX) && (curPointer.X < maxX) && (curPointer.Y > minY) && (curPointer.Y < maxY);
		}
	}
}
