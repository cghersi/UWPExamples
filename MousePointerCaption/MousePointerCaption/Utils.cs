using System;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace MousePointerCaption
{
    public static class Utils
    {
        public const double REAL_MIN_PRECISION_FOR_EQUALS = 0.0000001F;

        /// <summary>
        /// Returns the current position of the pointer for the given event.
        /// </summary>
        /// <param name="e">event arguments</param>
        /// <param name="relativeTo">element which the position should be computed relative to</param>
        /// <returns></returns>
        public static Point GetCurPoint(this PointerRoutedEventArgs e, UIElement relativeTo = null)
        {
            PointerPoint pp = e.GetCurrentPoint(relativeTo);
            return pp.Position;
        }

        /// <summary>
        /// Moves the given element by the given quantities.
        /// </summary>
        /// <param name="panel">element to move</param>
        /// <param name="deltaX">distance to move on the X coordinate</param>
        /// <param name="deltaY">distance to move on the Y coordinate</param>
        public static void Move(this FrameworkElement panel, double deltaX, double deltaY)
        {
            if (panel == null)
                return;
            Thickness margin = panel.Margin;
            margin.Left += deltaX;
            margin.Top += deltaY;
            panel.Margin = margin;
        }

        /// <summary>
        /// Returns v1 - v2.
        /// </summary>
        /// <param name="v2">subtractor</param>
        /// <param name="v1">subtracting</param>
        /// <returns></returns>
        public static Point Subtract(this Point v2, Point v1)
        {
            return new Point(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Point TimesScalar(this Point vec, double s)
        {
            return new Point(vec.X * s, vec.Y * s);
        }

        public static void AttachEventManager(this UIElement element, IPointerEventManager manager)
        {
            element.PointerEntered += manager.OnPointerEntered;
            element.PointerExited += manager.OnPointerExited;
            element.PointerPressed += manager.OnPointerDown;
            element.PointerMoved += manager.OnPointerMoved;
            element.PointerReleased += manager.OnPointerUp;
        }

        public static bool Equal(this Point arg1, Point arg2, double? allowPrecLoss = null)
        {
            if (allowPrecLoss.HasValue)
                return (Math.Abs(arg1.X - arg2.X) < allowPrecLoss.Value) &&
                       (Math.Abs(arg1.Y - arg2.Y) < allowPrecLoss.Value);
            else
                return arg1.Equals(arg2);
        }

        public static bool Equal(this float arg1, float arg2)
        {
            return Math.Abs(arg1 - arg2) < REAL_MIN_PRECISION_FOR_EQUALS;
        }
    }
}
