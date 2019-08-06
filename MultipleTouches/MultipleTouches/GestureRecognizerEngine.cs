//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MultipleTouches
{
	public class GestureRecognizerEngine
	{
		private const float MAX_TIME_DIFF = 1f / 60f;
		private const double MIN_TOUCH_PRECISION = 5; 

		/// <summary>
		/// The current state of the pointer(s).
		/// A pointer defined in a new event is defined existing if it shares *almost* the same position of
		/// one of the pointers stored in this list.
		/// </summary>
		private readonly List<Touch> m_pointerStates = new List<Touch>();

		/// <summary>
		/// The last instant when the framework detected a PointerDown event for this pointer.
		/// This is used to detect a double click.
		/// </summary>
		private DateTime m_lastPointerDownTime = DateTime.MinValue;

		private static readonly TaskFactory s_taskFactory = new TaskFactory();

		public static List<string> Logs { get; } = new List<string>();

		#region Public API
		///////////////////////////////////////////////////////////////////////////

		public int MillisecondsForDoubleClick { get; set; } = 100;

		public void AttachEvents(UIElement element)
		{
			element.PointerPressed += OnPointerDown;
			element.PointerMoved += OnPointerMoved;
			element.PointerReleased += OnPointerUp;
			element.PointerCaptureLost += OnPointerCaptureLost;
			element.PointerCanceled += OnPointerUp;
			element.DoubleTapped += OnDoubleTapped;

			if (!(element is ScrollViewer scrollView))
				return;

			scrollView.ViewChanged += OnViewChanged;
		}

		/// <summary>
		/// Brings the state of the engine back at the beginning, with all
		/// the gesture recognizers ready to fire and no pointer states.
		/// </summary>
		public void ResetState()
		{
			m_pointerStates.Clear();
		}

		public Touch GetTouch(uint pointerId, Point position)
		{
			// first, try to find the touch with the same pointerId:
			Touch res = m_pointerStates.FirstOrDefault(t => t.PointerId == pointerId);
			if (res != null)
				return res;

			// second try to find the touch using the last know position:
			foreach (Touch touch in m_pointerStates)
			{
				if (position.Subtract(touch.Current).Magnitude() < MIN_TOUCH_PRECISION)
					return touch;
			}

			Log("current: {0}; states={1}", position, PrintPointerStates());

			return null;
		}

		#endregion Public API
		///////////////////////////////////////////////////////////////////////////

		#region Event Managers
		///////////////////////////////////////////////////////////////////////////

		private void OnPointerDown(object sender, PointerRoutedEventArgs e)
		{
			if (e.Handled)
				return;
			Log("OnPointerDown {0}", e.GetCurPoint(null));

			// A new pointer is starting a gesture.
			if (sender is FrameworkElement elem)
			{
				bool captured = elem.CapturePointer(e.Pointer);
				Log("OnPointerDown Captured {0}: {1}", e.Pointer.PointerId, captured);
			}

			// 1) Refresh the pointer state (or add a new one):
			Touch newTouch = RefreshPointerState(e, out bool isDoubleClick);
			m_lastPointerDownTime = DateTime.Now;

			// 2) decide if and which gesture recognizers should be fired:
			Touch[] touches = GetTouches();
			Log("OnPointerDown - Touches {0}; isDoubleClick {1}; newTouch {2}", 
				string.Join('|', touches.Select(t => t.ToString())), isDoubleClick, newTouch);

			e.Handled = true;
		}

		private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (e.Handled || !e.Pointer.IsInContact)
				return;
			Log("OnPointerMoved {1} {0} {2}", e.GetCurPoint(null), e.Pointer.PointerId, m_pointerStates.Count);

			// An existing pointer is continuing a gesture.

			// 1) Refresh the pointer state (or add a new one):
			Touch newTouch = RefreshPointerState(e, out _);

			// 2) decide if and which gesture recognizers should be fired:
			Touch[] touches = GetTouches();
			Log("OnPointerMoved - Touches {0}; newTouch {1}", string.Join('|', touches.Select(t => t.ToString())), newTouch);

			e.Handled = true;
		}

		private void OnPointerUp(object sender, PointerRoutedEventArgs e)
		{
			if (e.Handled)
				return;
			Log("OnPointerUp {0}", e.GetCurPoint(null));

			// An existing pointer has finished its gesture.

			// 1) Refresh the pointer state (or add a new one):
			Touch newTouch = RefreshPointerState(e, out _);
			newTouch.IsEnded = true;

			// 2) Decide if and which gesture recognizers should be fired:
			Touch[] touches = GetTouches();
			Log("OnPointerUp - Touches {0}", string.Join('|', touches.Select(t => t.ToString())));
	
			// 3) Clear the state of the pointer:
			m_pointerStates.Remove(newTouch);

			Log("OnPointerUp - Last pointer state: {0}", PrintPointerStates());

			// 4) If there's no more touch, let's reset the states of all the gesture recognizers:
			if (m_pointerStates.Count == 0)
				ResetState();

			e.Handled = true;
		}

		private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
		{
			if (e.Handled)
				return;

			// for some mysterious reason this is fired when the user puts a second finger on the screen.
			// So, disregard this case:
			if (e.Pointer.IsInContact)
			{
				if (sender is FrameworkElement elem)
				{
					bool captured = elem.CapturePointer(e.Pointer);
					Log("OnPointerCaptureLost ReCaptured {0}: {1}", e.Pointer.PointerId, captured);
				}
				//e.Handled = true;
				//return;
			}

			// keep track of the possible touch to remove:
			Touch lostCapture = GetTouch(e.Pointer.PointerId, e.GetCurPoint(null));

			// start a timer that eventually will remove the pointer and, if needed, reset the state:
			s_taskFactory.StartNew(() =>
			{
				// wait some time before resetting the state:
				Thread.Sleep(TimeSpan.FromSeconds(5));

				Log("OnPointerCaptureLost - Last pointer state: {0}", PrintPointerStates());

				if (lostCapture != null)
					m_pointerStates.Remove(lostCapture);

				// If there's no more touch, let's reset the states of all the gesture recognizers:
				if (m_pointerStates.Count == 0)
					ResetState();
			});

			e.Handled = true;
		}

		private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
		{
			Log("OnDoubleTapped {0}", e.GetPosition(null));

			// If there's no more touch, let's reset the states of all the gesture recognizers:
			if (m_pointerStates.Count == 0)
				ResetState();
		}

		private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (!e.IsIntermediate)
				Log("OnViewChanged {0}", e.IsIntermediate);

			// Check if this is the end of the gesture:
			if (!e.IsIntermediate)
			{
				// If there's no more touch, let's reset the states of all the gesture recognizers:
				if (m_pointerStates.Count == 0)
					ResetState();
			}
		}

		#endregion Event Managers
		///////////////////////////////////////////////////////////////////////////

		#region Private Utilities
		///////////////////////////////////////////////////////////////////////////

		protected virtual Touch[] GetTouches()
		{
			return m_pointerStates.ToArray();
		}

		private Touch RefreshPointerState(PointerRoutedEventArgs e, out bool isDoubleClick)
		{
			// 1) check if we already have this pointer under management:
			Point currentPos = e.GetCurPoint();
			Touch existingState = GetTouch(e.Pointer.PointerId, currentPos);

			// 2) Create an Touch to represent this event:
			Touch newTouch = ToTouch(e, currentPos, existingState);

			isDoubleClick = (newTouch.CurrentTime - m_lastPointerDownTime).TotalMilliseconds < MillisecondsForDoubleClick;

			// 3) Add to the state of the pointers:
			if (existingState == null)
				m_pointerStates.Add(newTouch);

			return newTouch;
		}

		public Touch ToTouch(PointerRoutedEventArgs args, Point currentPos, Touch existingState)
		{
			DateTime now = DateTime.Now;

			Touch res;
			if (existingState == null)
			{
				res = new Touch
				{
					PointerId = args.Pointer.PointerId,
					StartTime = now,
					IsEnded = false,
					Last = currentPos,
					LocationWhenGestureWasRec = currentPos,
					Start = currentPos,
					Velocity = new Point(0,0),
				};
			}
			else
			{
				res = existingState.Copy();
				res.CurrentTime = DateTime.Now;

				// compute the velocity:
				double timeDiff = Math.Max(MAX_TIME_DIFF, (now - existingState.CurrentTime).TotalSeconds); // in seconds
				res.Velocity = new Point((currentPos.X - existingState.X) / timeDiff, (currentPos.Y - existingState.Y) / timeDiff);

				res.Last = existingState.Current;
			}

			res.CurrentTime = now;
			res.Current = currentPos;
			res.EventArgs = args;

			return res;
		}

		public static Touch ToTouch(DoubleTappedRoutedEventArgs args)
		{
			Point currentPos = args.GetCurPoint();
			DateTime now = DateTime.Now;

			return new Touch
			{
				StartTime = now,
				IsEnded = false,
				Last = currentPos,
				LocationWhenGestureWasRec = currentPos,
				Start = currentPos,
				Velocity = new Point(0, 0),
				CurrentTime = now,
				Current = currentPos
			};
		}

		public static Touch ToTouch(HoldingRoutedEventArgs args)
		{
			Point currentPos = args.GetCurPoint();
			DateTime now = DateTime.Now;

			return new Touch
			{
				StartTime = now,
				IsEnded = false,
				Last = currentPos,
				LocationWhenGestureWasRec = currentPos,
				Start = currentPos,
				Velocity = new Point(0, 0),
				CurrentTime = now,
				Current = currentPos
			};
		}

		private static int s_count = 0;

		public static void Log(string format, params object[] args)
		{
			string log = string.Format(format, args);
			Logs.Append(log);
			Debug.WriteLine(s_count++ + ": " + log);
		}


		private string PrintPointerStates()
		{
			StringBuilder sb = new StringBuilder();
			foreach (Touch touch in m_pointerStates)
			{
				sb.AppendFormat("{0} |", touch);
			}

			return sb.ToString();
		}

		#endregion Private Utilities
		///////////////////////////////////////////////////////////////////////////
	}
}
