using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace MoveShapeFilledByImageBrush
{
	public class PointerManager
	{
		/// <summary>
		/// Specifies the current state of the mouse handling logic.
		/// </summary>
		protected PointerHandlingMode m_pointerMode = PointerHandlingMode.None;

		/// <summary>
		/// Specifies the state of the mouse handling logic when an action is properly triggered.
		/// </summary>
		protected readonly PointerHandlingMode m_actionMode;

		/// <summary>
		/// Stores the normal cursor in order to reintegrate it once the gesture is finished
		/// </summary>
		// ReSharper disable once InconsistentNaming
		protected static readonly CoreCursorType s_initialCursor = CoreCursorType.Arrow;

		protected readonly CoreCursorType m_cursorType;

		/// <summary>
		/// The point that was clicked relative to the content that is contained within the parent control.
		/// </summary>
		protected Point m_origContentPointerDownPoint;

		private ImageWithMask m_image;

		/// <summary>
		/// Element which the position should be computed relative to
		/// </summary>
		protected UIElement PositionRelativeTo { get; set; }

		public PointerManager(ImageWithMask image, PointerHandlingMode actionMode = PointerHandlingMode.Dragging,
			CoreCursorType cursorType = CoreCursorType.Hand)
		{
			m_image = image;
			m_actionMode = actionMode;
			m_cursorType = cursorType;
		}

		public virtual void OnPointerDown(object sender, PointerRoutedEventArgs e)
		{
			if (m_pointerMode != PointerHandlingMode.None)
				return; // We are in some other mouse handling mode, don't do anything.

			m_pointerMode = m_actionMode;
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(m_cursorType, 0);
			m_origContentPointerDownPoint = GetCurPoint(e, PositionRelativeTo);
			if (sender is FrameworkElement elem)
				elem.CapturePointer(e.Pointer);

			e.Handled = true;
		}

		public virtual void OnPointerUp(object sender, PointerRoutedEventArgs e)
		{
			if (m_pointerMode != m_actionMode)
				return;   // let's manage only the active mode.

			m_pointerMode = PointerHandlingMode.None;
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(s_initialCursor, 0);
			FrameworkElement elem = sender as FrameworkElement;
			elem?.ReleasePointerCapture(e.Pointer);

			e.Handled = true;
		}

		public virtual void OnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (m_pointerMode != m_actionMode)
				return; // let's manage only the active mode.

			if (!e.Pointer.IsInContact)
			{
				// something fishy here... for some unknown reason we didn't fully performed the actions in the 
				// OnPointerUp, therefore let's release the action now:
				m_pointerMode = PointerHandlingMode.None;
				Window.Current.CoreWindow.PointerCursor = new CoreCursor(s_initialCursor, 0);
				FrameworkElement elem = sender as FrameworkElement;
				elem?.ReleasePointerCapture(e.Pointer);
				e.Handled = true;
				return;
			}

			Point curContentPoint = GetCurPoint(e, PositionRelativeTo);
			Point dragVector = Subtract(m_origContentPointerDownPoint, curContentPoint);

			FrameworkElement senderElem = sender as FrameworkElement;

			// perform the required action:
			Move(senderElem, dragVector.X, dragVector.Y);
			m_image?.SetFrame(0, 0);

			// update the position of the element as the user drags it.
			m_origContentPointerDownPoint = curContentPoint;

			e.Handled = true;
		}

		public static Point GetCurPoint(PointerRoutedEventArgs e, UIElement relativeTo = null)
		{
			PointerPoint pp = e.GetCurrentPoint(relativeTo);
			return pp.Position;
		}

		public static Point Subtract(Point v2, Point v1)
		{
			return new Point(v1.X - v2.X, v1.Y - v2.Y);
		}

		public static void Move(FrameworkElement panel, double deltaX, double deltaY)
		{
			if (panel == null)
				return;
			Thickness margin = panel.Margin;
			margin.Left += deltaX;
			margin.Top += deltaY;
			panel.Margin = margin;
		}
	}

	public enum PointerHandlingMode
	{
		/// <summary>
		/// Not in any special mode.
		/// </summary>
		None,

		/// <summary>
		/// The user is left-dragging elements with the pointer (mouse or touch).
		/// </summary>
		Dragging,

		/// <summary>
		/// The user is left-mouse-button-dragging to pan the viewport.
		/// </summary>
		Panning,

		/// <summary>
		/// The user is left-dragging the border of an element to resize it.
		/// </summary>
		Resizing
	}
}
