//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace MultipleTouches
{
	/// <summary>
	/// Provides utilities to deal with events and pointers.
	/// </summary>
	public static class EventManagerUtils
	{
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
		/// Returns the current position of the pointer for the given event.
		/// </summary>
		/// <param name="e">event arguments</param>
		/// <param name="relativeTo">element which the position should be computed relative to</param>
		/// <returns></returns>
		public static Point GetCurPoint(this HoldingRoutedEventArgs e, UIElement relativeTo = null)
		{
			return e.GetPosition(relativeTo);
		}

		/// <summary>
		/// Returns the current position of the pointer for the given event.
		/// </summary>
		/// <param name="e">event arguments</param>
		/// <param name="relativeTo">element which the position should be computed relative to</param>
		/// <returns></returns>
		public static Point GetCurPoint(this DoubleTappedRoutedEventArgs e, UIElement relativeTo = null)
		{
			return e.GetPosition(relativeTo);
		}

		/// <summary>
		/// Avoids the manipulation event from bubbling up from this element.
		/// </summary>
		/// <param name="elem"></param>
		public static void DisableManipulationEvents(this UIElement elem)
		{
			if (elem == null)
				return;
			elem.ManipulationCompleted += (sender, args) =>
			{
				args.Handled = true;
			};
			elem.ManipulationDelta += (sender, args) =>
			{
				args.Handled = true;
			};
			elem.ManipulationInertiaStarting += (sender, args) =>
			{
				args.Handled = true;
			};
			elem.ManipulationStarted += (sender, args) =>
			{
				args.Handled = true;
			};
			elem.ManipulationStarting += (sender, args) =>
			{
				args.Handled = true;
			};
		}

		public static Point Subtract(this Point v2, Point v1)
		{
			return new Point(v1.X - v2.X, v1.Y - v2.Y);
		}
		public static double Magnitude(this Point vec)
		{
			return Math.Sqrt((vec.X * vec.X) + (vec.Y * vec.Y));
		}
	}
}
