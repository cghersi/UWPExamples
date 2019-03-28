using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace MousePointerCaption
{
    public class BasePointerManager : IPointerEventManager
    {
        public int EventCount { get; private set; }

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
        protected static readonly CoreCursorType s_initialCursor = CoreCursorType.Arrow;

        protected readonly CoreCursorType m_cursorType;

        protected string m_name;

        /// <summary>
        /// The point that was clicked relative to the content that is contained within the parent control.
        /// </summary>
        protected Point m_origContentPointerDownPoint;

        /// <summary>
        /// Element which the position should be computed relative to
        /// </summary>
        protected UIElement PositionRelativeTo { get; set; }

        /// <summary>
        /// Keeps track of number of fingers that are currently touching the screen
        /// </summary>
        protected int m_pointerCount = 0;

        public BasePointerManager(string name, PointerHandlingMode actionMode = PointerHandlingMode.Dragging,
            CoreCursorType cursorType = CoreCursorType.Hand)
        {
            m_name = name;
            m_actionMode = actionMode;
            m_cursorType = cursorType;
        }

        /// <inheritdoc />
        public virtual void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            m_pointerCount++;
        }

        /// <inheritdoc />
        public virtual void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            m_pointerCount--;
        }

        /// <inheritdoc />
        public virtual void OnPointerDown(object sender, PointerRoutedEventArgs e)
        {
            bool correctDeviceType = IsCorrectDeviceType(e);
            if ((m_pointerMode != PointerHandlingMode.None) || !correctDeviceType)
                return; // We are in some other mouse handling mode, don't do anything.

            m_pointerMode = m_actionMode;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(m_cursorType, 0);
            m_origContentPointerDownPoint = e.GetCurPoint(PositionRelativeTo);
            FrameworkElement elem = sender as FrameworkElement;
            elem?.CapturePointer(e.Pointer);
            EventCount = 0; // start counting events

            // perform the required action:
            OnPointerDownDelegate(e);

            e.Handled = true;
        }

        /// <inheritdoc />
        public virtual void OnPointerUp(object sender, PointerRoutedEventArgs e)
        {
            bool correctDeviceType = IsCorrectDeviceType(e);
            if ((m_pointerMode != m_actionMode) || !correctDeviceType)
                return;   // let's manage only the active mode.

            m_pointerMode = PointerHandlingMode.None;
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(s_initialCursor, 0);
            FrameworkElement elem = sender as FrameworkElement;
            elem?.ReleasePointerCapture(e.Pointer);

            // perform the required action:
            OnPointerUpDelegate(e);

            e.Handled = true;
        }

        /// <inheritdoc />
        public virtual void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            bool correctDeviceType = IsCorrectDeviceType(e);
            //if (m_pointerMode != PointerHandlingMode.None)
            //	s_log.InfoFormat("OnPointerMoved for {0}; pointerMode={1}; deviceType={2}", m_name, m_pointerMode,
            //		correctDeviceType);
            if ((m_pointerMode != m_actionMode) || !correctDeviceType)
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

            Point curContentPoint = e.GetCurPoint(PositionRelativeTo);
            Point dragVector = m_origContentPointerDownPoint.Subtract(curContentPoint)
                .TimesScalar(1 / MainPage.CurrentZoom);
            FrameworkElement senderElem = sender as FrameworkElement;
            EventCount++;

            // perform the required action:
            OnPointerMovedDelegate(dragVector, senderElem, curContentPoint);

            // update the position of the element as the user drags it.
            m_origContentPointerDownPoint = curContentPoint;

            e.Handled = true;
        }

        /// <summary>
        /// The actual action to perform when an event of PointerDown is detected.
        /// </summary>
        /// <returns>true if the action should be stopped now (no further needs for actions in the parent)</returns>
        protected virtual bool OnPointerDownDelegate(PointerRoutedEventArgs e)
        {
            // nothing to do here...
            return false;
        }

        /// <summary>
        /// The actual action to perform when an event of PointerUp is detected.
        /// </summary>
        /// <returns>true if the action should be stopped now (no further needs for actions in the parent)</returns>
        protected virtual bool OnPointerUpDelegate(PointerRoutedEventArgs e)
        {
            // nothing to do here...
            return false;
        }

        /// <summary>
        /// The actual action to perform when an event of PointerMove is detected.
        /// </summary>
        /// <param name="dragVector">the differential value between the last recorded position and the current one</param>
        /// <param name="sender">target element of the event</param>
        /// <param name="currentPoint">the point where happened the pointer hit</param>
        /// <returns>true if the action should be stopped now (no further needs for actions in the parent)</returns>
        protected virtual bool OnPointerMovedDelegate(Point dragVector, FrameworkElement sender, Point currentPoint)
        {
            // nothing to do here...
            return false;
        }

        protected virtual bool IsCorrectDeviceType(PointerRoutedEventArgs e)
        {
            // by default, we accept anything
            return true;
        }

        public bool IsActive => m_pointerMode != PointerHandlingMode.None;
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
