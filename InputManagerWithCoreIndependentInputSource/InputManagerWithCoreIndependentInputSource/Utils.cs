using System;
using System.Collections.Generic;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Xaml;

namespace InputManagerWithCoreIndependentInputSource
{
	public static class Utils
	{
		public const float MAX_TIME_DIFF = 1f / 60f;
		public const double REAL_MIN_PRECISION_FOR_EQUALS = 0.0000001F;

		public static bool Equal(this double arg1, double arg2)
		{
			return Math.Abs(arg1 - arg2) < REAL_MIN_PRECISION_FOR_EQUALS;
		}

		public static bool IsNullOrEmpty<T>(this ICollection<T> list)
		{
			return (list == null) || (list.Count <= 0);
		}


		public static LTTouch ToLTTouch(this PointerEventArgs args, LTTouch existingState, UIElement relativeTo = null)
		{
			PointerPoint pp = args.CurrentPoint;
			if (pp == null)
				return null;
			Point curPos = pp.Position;
			LTPoint currentPos = Encoders.BuildLTPoint(curPos.X, curPos.Y);
			//if (currentPos == null)
			//	return null;
			DateTime now = DateTime.Now;

			//// if there's isn't an existing state, check if the last lost touch is in the same area of the current pointer:
			//if ((existingState == null) && (m_lastTouchLost != null) && (m_lastTouchLost.Current.Subtract(currentPos).Magnitude() < 1))
			//{
			//	existingState = m_lastTouchLost;
			//	existingState.PointerId = args.Pointer.PointerId;
			//	m_lastTouchLost = null;
			//}

			LTTouch res;
			if (existingState == null)
			{
				res = new LTTouch
				{
					PointerId = pp.PointerId,
					StartTime = now,
					IsEnded = false,
					Last = currentPos,
					LocationWhenGestureWasRec = currentPos,
					Start = currentPos,
					Velocity = Encoders.POINT_ZERO,
					PointerType = pp.PointerDevice.PointerDeviceType.ToPointerType(pp.Properties.IsEraser)
				};
			}
			else
			{
				res = new LTTouch();
				res.Copy(existingState);
				res.CurrentTime = DateTime.Now;

				// compute the velocity:
				double timeDiff = Math.Max(MAX_TIME_DIFF, (now - existingState.CurrentTime).TotalSeconds); // in seconds
				res.Velocity = Encoders.BuildLTPoint((currentPos.X - existingState.X) / timeDiff,
					(currentPos.Y - existingState.Y) / timeDiff);

				res.Last = existingState.Current;
			}

			res.CurrentTime = now;
			res.Current = currentPos;
			res.IsInContact = pp.IsInContact;
			PointerPointProperties props = pp.Properties;
			if (props != null)
			{
				res.Pressure = props.Pressure;
				res.XTilt = props.XTilt;
				res.YTilt = props.YTilt;
				res.UpdateKind = props.PointerUpdateKind.ToLTUpdateKind();
			}

			return res;
		}

		public static PointerType ToPointerType(this PointerDeviceType type, bool isEraser)
		{
			switch (type)
			{
				case PointerDeviceType.Mouse:
					return PointerType.Mouse;
				case PointerDeviceType.Pen:
					if (isEraser)
						return PointerType.Eraser;
					else
						return PointerType.Pen;
				case PointerDeviceType.Touch:
					return PointerType.Finger;
			}

			return PointerType.Mouse;
		}

		public static LTPointerUpdateKind ToLTUpdateKind(this PointerUpdateKind kind)
		{
			switch (kind)
			{
				case PointerUpdateKind.LeftButtonPressed:
					return LTPointerUpdateKind.LeftButtonPressed;
				case PointerUpdateKind.LeftButtonReleased:
					return LTPointerUpdateKind.LeftButtonReleased;
				case PointerUpdateKind.RightButtonPressed:
					return LTPointerUpdateKind.RightButtonPressed;
				case PointerUpdateKind.RightButtonReleased:
					return LTPointerUpdateKind.RightButtonReleased;
				case PointerUpdateKind.MiddleButtonPressed:
					return LTPointerUpdateKind.MiddleButtonPressed;
				case PointerUpdateKind.MiddleButtonReleased:
					return LTPointerUpdateKind.MiddleButtonReleased;
				case PointerUpdateKind.XButton1Pressed:
					return LTPointerUpdateKind.XButton1Pressed;
				case PointerUpdateKind.XButton1Released:
					return LTPointerUpdateKind.XButton1Released;
				case PointerUpdateKind.XButton2Pressed:
					return LTPointerUpdateKind.XButton2Pressed;
				case PointerUpdateKind.XButton2Released:
					return LTPointerUpdateKind.XButton2Released;
			}

			return LTPointerUpdateKind.Other;
		}

		public static void Move(this FrameworkElement panel, double deltaX, double deltaY)
		{
			if (panel == null)
				return;
			Thickness margin = panel.Margin;
			margin.Left += deltaX;
			margin.Top += deltaY;
			panel.Margin = margin;
		}
	}

	public static class Encoders
	{
		public static readonly LTPoint POINT_NULL = BuildLTPoint(double.MaxValue, double.MaxValue);
		public static readonly LTPoint POINT_ZERO = BuildLTPoint(0, 0);

		public static LTPoint BuildLTPoint(double x, double y)
		{
			return new LTPoint(x, y);
		}
	}
}
