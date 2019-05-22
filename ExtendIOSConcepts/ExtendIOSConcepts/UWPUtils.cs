using System;
using Windows.UI.Xaml;

namespace ExtendIOSConcepts
{
	public static class UWPUtils
	{
		public const double REAL_MIN_PRECISION_FOR_EQUALS = 0.0000001F;

		/// <summary>
		/// Sets the Margin for the given element.
		/// </summary>
		/// <param name="panel">element to set the margin for</param>
		/// <param name="left">distance from left side</param>
		/// <param name="top">distance from top ceiling</param>
		public static void SetMargin(this FrameworkElement panel, double? left, double? top)
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
		/// Sets the Size for the given element.
		/// </summary>
		/// <param name="panel">element to set the size for</param>
		/// <param name="width">exact width</param>
		/// <param name="height">exact height</param>
		public static void SetSize(this FrameworkElement panel, double? width, double? height)
		{
			if (panel == null)
				return;

			if (width.HasValue && (width >= 0))
				panel.Width = width.Value;

			if (height.HasValue && (height >= 0))
				panel.Height = height.Value;
		}

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		public static bool Equal(this double? arg1, double? arg2)
		{
			if (arg1 == null)
				return (arg2 == null);
			if (arg2 == null)
				return false;
			return Math.Abs(arg1.Value - arg2.Value) < REAL_MIN_PRECISION_FOR_EQUALS;
		}

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		public static bool Equal(this float? arg1, float? arg2)
		{
			if (arg1 == null)
				return (arg2 == null);
			if (arg2 == null)
				return false;
			return Math.Abs(arg1.Value - arg2.Value) < REAL_MIN_PRECISION_FOR_EQUALS;
		}

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		public static bool Equal(this double arg1, double arg2)
		{
			return Math.Abs(arg1 - arg2) < REAL_MIN_PRECISION_FOR_EQUALS;
		}

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <returns></returns>
		public static bool Equal(this float arg1, float arg2)
		{
			return Math.Abs(arg1 - arg2) < REAL_MIN_PRECISION_FOR_EQUALS;
		}
	}
}
