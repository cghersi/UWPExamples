//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using DirectXPanels;

namespace InputManagerWithCoreIndependentInputSource
{
	public class LTGestureRecognizerEngine
	{
		private DateTime m_startTime;
		private int m_eventCount = 0;

		/// <summary>
		/// The current state of the pointer(s).
		/// Keys are the PointerId (PointerRoutedEventArgs.Pointer.PointerId);
		/// Values are the state of the touch.
		/// </summary>
		private readonly Dictionary<uint, LTTouch> m_pointerStates = new Dictionary<uint, LTTouch>();

		/// <summary>
		/// The last instant when the framework detected a PointerDown event for this pointer.
		/// This is used to detect a double click.
		/// </summary>
		private DateTime m_lastPointerDownTime = DateTime.MinValue;

		/// <summary>
		/// Keep track of the last touch that lost the capture, since UWP sometimes tends to "lose" some pointers...
		/// </summary>
		private LTTouch m_lastTouchLost;

		private readonly List<LTGestureRecognizer> m_gestureRecognizers = new List<LTGestureRecognizer>();

		/// <summary>
		/// The collection of all the touch events to manage in the Rendering event handler.
		/// </summary>
		private static readonly Queue<LTTouchEvent> s_eventStack = new Queue<LTTouchEvent>(50);

		public static LTClassProfiler ProfilerDown { get; } = new LTClassProfiler("LTGestureRecognizerEngine-Down", LTProfilerLevel.High);
		public static LTClassProfiler ProfilerMoved { get; } = new LTClassProfiler("LTGestureRecognizerEngine-Moved", LTProfilerLevel.High);
		public static LTClassProfiler ProfilerUp { get; } = new LTClassProfiler("LTGestureRecognizerEngine-Up", LTProfilerLevel.High);
		public static LTClassProfiler ProfilerCapLost { get; } = new LTClassProfiler("LTGestureRecognizerEngine-CapLost", LTProfilerLevel.High);
		public static LTClassProfiler ProfilerRendering { get; } = new LTClassProfiler("LTGestureRecognizerEngine-Rendering", LTProfilerLevel.High);


		#region Public API
		///////////////////////////////////////////////////////////////////////////

		public int MillisecondsForDoubleClick { get; set; } = 100;

		/// <summary>
		/// Adds a new gesture recognizer to the engine.
		/// Note: the order of the recognizers matters!! It determines their priority in the evaluation of simultaneous
		/// gestures, which means that if gestureA is added before gestureB, in case both gestureA and gestureB can begin
		/// but they cannot go along together, only gestureA will be fired and gestureB instead will be put in Failed state.
		/// </summary>
		/// <param name="gestureRec"></param>
		public void AddGestureRecognizer(LTGestureRecognizer gestureRec)
		{
			m_gestureRecognizers.Add(gestureRec);
		}

		/// <summary>
		/// Associates this engine to the given elements for event firing and coordinate computation.
		/// </summary>
		/// <param name="inputSource">source that fires the events</param>
		public void AttachEvents(DrawingPanel inputSource)
		{
			// Register for pointer events, which will be raised on the background thread:
			inputSource.OnLTPointerPressed += OnPointerDown;
			inputSource.OnLTPointerMoved += OnPointerMoved;
			inputSource.OnLTPointerReleased += OnPointerUp;
			inputSource.OnLTPointerCaptureLost += OnPointerCaptureLost;
		}

		/// <summary>
		/// Brings the state of the engine back at the beginning, with all
		/// the gesture recognizers ready to fire and no pointer states.
		/// </summary>
		public void ResetState()
		{
			m_pointerStates.Clear();
			foreach (LTGestureRecognizer recognizer in m_gestureRecognizers)
			{
				recognizer.State = LTGestureRecState.Possible;
			}
		}

		public LTTouch GetTouch(uint pointerId)
		{
			return m_pointerStates.ContainsKey(pointerId) ? m_pointerStates[pointerId] : null;
		}

		public static async void CompositionTarget_Rendering(object sender, object e)
		{
			int pointer = ProfilerRendering.Start();

			// 1) retrieve all the events generated since the last run:
			LTTouchEvent[] events = s_eventStack.ToArray();
			s_eventStack.Clear();

			if (events.IsNullOrEmpty())
				return;

			// 2) mange each single event in cascade:
			foreach (LTTouchEvent t in events)
			{
				await ManageLTTouchEvent(t);
			}

			ProfilerRendering.CollectResults(pointer);
		}

		private static async Task ManageLTTouchEvent(LTTouchEvent touchEvent)
		{
			// 1) Retrieve the updated set of current touches:
			LTGestureRecognizerEngine engine = touchEvent.Engine;
			LTTouch[] touches = engine.GetTouches();
			if (touches.IsNullOrEmpty())
				return;   // defensive, it should never happen!

			switch (touchEvent.EventType)
			{
				case LTTouchEventType.Down:
					await engine.OnPointerDown(touches, touchEvent.Touch, touchEvent.IsDoubleClick);
					break;
				case LTTouchEventType.Moved:
					await engine.OnPointerMoved(touches, touchEvent.Touch);
					break;
				case LTTouchEventType.Up:
					engine.OnPointerUp(touches, touchEvent.Touch);
					break;
			}
		}

		#endregion Public API
		///////////////////////////////////////////////////////////////////////////

		#region Event Managers
		///////////////////////////////////////////////////////////////////////////

		private void OnPointerDown(object sender, PointerEventArgs e)
		{
			m_startTime = DateTime.Now;
			m_eventCount = 0;
			//int pointer = ProfilerDown.Start();

			// A new pointer is starting a gesture.
			// for starters, let's capture the pointer:
			//m_inputSource.SetPointerCapture();

			// 1) Refresh the pointer state (or add a new one):
			LTTouch newTouch = RefreshPointerState(e, true, out bool isDoubleClick);
			if (newTouch == null)
			{
				e.Handled = true;
				return;
			}
			m_lastPointerDownTime = DateTime.Now;

			// 2) Enqueue the event so that it can be processed in the next Rendering event:
			s_eventStack.Enqueue(new LTTouchEvent(this, LTTouchEventType.Down, newTouch, isDoubleClick));
		}

		private void OnPointerMoved(object sender, PointerEventArgs e)
		{
			if (!e.CurrentPoint.IsInContact)
				return;
			//s_log.InfoFormat("OnPointerMoved {1} {0}", e.GetCurPoint(null), e.Pointer.PointerId);

			m_eventCount++;
			//int pointer = ProfilerMoved.Start();

			// An existing pointer is continuing a gesture.

			// 1) Refresh the pointer state (or add a new one):
			LTTouch newTouch = RefreshPointerState(e, true, out _);
			if (newTouch == null)
			{
				e.Handled = true;
				return;
			}

			// 2) Enqueue the event so that it can be processed in the next Rendering event:
			s_eventStack.Enqueue(new LTTouchEvent(this, LTTouchEventType.Moved, newTouch, false));
		}

		private void OnPointerUp(object sender, PointerEventArgs e)
		{
			if (m_eventCount > 0)
			{
				TimeSpan diffFromStart = DateTime.Now - m_startTime;
				Debug.WriteLine("in {0}ms there has been {1} events, avg of 1 event every {2}ms",
					diffFromStart.TotalMilliseconds, m_eventCount, diffFromStart.TotalMilliseconds / m_eventCount);
			}

			//int pointer = ProfilerUp.Start();
			//s_log.InfoFormat("OnPointerUp {0}", e.GetCurPoint(null));

			// An existing pointer has finished its gesture.

			// 1) Refresh the pointer state (or add a new one):
			LTTouch newTouch = RefreshPointerState(e, true, out _);
			if (newTouch == null)
			{
				e.Handled = true;
				return;
			}

			// 2) Enqueue the event so that it can be processed in the next Rendering event:
			s_eventStack.Enqueue(new LTTouchEvent(this, LTTouchEventType.Up, newTouch, false));
		}

		private async Task OnPointerDown(LTTouch[] touches, LTTouch newTouch, bool isDoubleClick)
		{
			int pointer = ProfilerDown.Start();
			//s_log.InfoFormat("OnPointerDown {0}", touch.Current);

			// 1) Make sure we are in an healthy recognizers configuration:
			if (m_gestureRecognizers.All(gr => !gr.NonFinalState))
			{
				// this should never happen. Sadly, it could happen instead if we lose some finalization event,
				// like the PointerRelease or Cancelled.
				ResetState();
			}

			// 2) decide if and which gesture recognizers should be fired:

			//s_log.InfoFormat("OnPointerDown - {0}", PrintEngineState());

			// 2.1) first, only the recognizers with the right number of touches (and in a suitable state) should be fired:
			IEnumerable<LTGestureRecognizer> interestingRecognizers = m_gestureRecognizers.Where(gr =>
				(gr.SupportedTouchNumber == touches.Length) && (gr.State == LTGestureRecState.Possible));

			// 2.2) then, let's detect the recognizers whose supported touch number is wrong:
			MakeTransitionsForWrongTouchNumbers(touches);

			// 2.3) go thru the list of interesting recognizers, and check the specific cases:
			foreach (LTGestureRecognizer recognizer in interestingRecognizers)
			{
				// for a double click, make sure the Tap fails:
				if (isDoubleClick && (recognizer.Type == LTGestureType.Tap))
				{
					recognizer.State = LTGestureRecState.Failed;
					continue;
				}
				else if (!isDoubleClick && (recognizer.Type == LTGestureType.DoubleTap))
				{
					// on the contrary, if it is NOT a double click, make sure the DoubleTap fails:
					recognizer.State = LTGestureRecState.Failed;
					continue;
				}

				// If the recognizer is a single action, here there is nothing to do, so let's go to the next one directly:
				if (recognizer.IsSingleAction)
					continue;

				//s_log.InfoFormat("OnPointerDown - ManageBeganGesture for {0}", recognizer);
				LTGestureRecState newState = await ManageBeganGesture(recognizer, touches, newTouch);

				// update the state of the recognizer:
				//s_log.InfoFormat("OnPointerDown - ManageBeganGesture for {0} returned {1}", recognizer, newState);
				recognizer.State = newState;
			}

			ProfilerDown.CollectResults(pointer);
		}

		private async Task OnPointerMoved(LTTouch[] touches, LTTouch newTouch)
		{
			int pointer = ProfilerMoved.Start();

			// 1) first, only the recognizers with the right number of touches (and not discrete) should be fired:
			IEnumerable<LTGestureRecognizer> interestingRecognizers =
					m_gestureRecognizers.Where(gr => !gr.IsSingleAction && (gr.SupportedTouchNumber == touches.Length));

			// 2) then, let's detect the recognizers whose supported touch number is wrong:
			MakeTransitionsForWrongTouchNumbers(touches);

			// 3) check simultaneous recognizers:
			IEnumerable<LTGestureRecognizer> readyToFireRecognizers = ReadyToFireRecognizers(interestingRecognizers, false);

			// 3) go thru the list of recognizers ready to fire:
			foreach (LTGestureRecognizer recognizer in readyToFireRecognizers)
			{
				LTGestureRecState newState = recognizer.State;
				switch (recognizer.State)
				{
					case LTGestureRecState.Began:
					case LTGestureRecState.Changed:
						//s_log.InfoFormat("OnPointerMoved - TouchesMoved for {0}", recognizer);
						newState = await recognizer.TouchesMoved(touches);
						//s_log.InfoFormat("OnPointerMoved - TouchesMoved for {0} returned {1}", recognizer, newState);
						break;

					case LTGestureRecState.Possible:
						//s_log.InfoFormat("OnPointerMoved - ManageBeganGesture for {0}", recognizer);
						newState = await ManageBeganGesture(recognizer, touches, newTouch);
						//s_log.InfoFormat("OnPointerMoved - ManageBeganGesture for {0} returned {1}", recognizer, newState);
						break;

					case LTGestureRecState.Cancelled:
					case LTGestureRecState.Ended:
					case LTGestureRecState.Failed:
						// nothing to do here...
						break;
				}

				// update the state of the recognizer:
				recognizer.State = newState;
			}

			ProfilerMoved.CollectResults(pointer);
		}

		private void OnPointerUp(LTTouch[] touches, LTTouch newTouch)
		{
			int pointer = ProfilerUp.Start();
			//s_log.InfoFormat("OnPointerUp {0} - {1}", e.GetCurPoint(null), PrintEngineState());

			// An existing pointer has finished its gesture.

			// 1) first, only the recognizers with the right number of touches (and not discrete) should be fired:
			IEnumerable<LTGestureRecognizer> interestingRecognizers = m_gestureRecognizers.Where(gr => gr.IsRunning && (gr.SupportedTouchNumber == touches.Length));

			// 2) then, let's detect the recognizers whose supported touch number is wrong:
			MakeTransitionsForWrongTouchNumbers(touches);

			// 3) check simultaneous recognizers:
			IEnumerable<LTGestureRecognizer> readyToFireRecognizers = ReadyToFireRecognizers(interestingRecognizers.Where(gr => !gr.IsSingleAction), true);

			// 4) manage the continuous recognizers to fire:
			foreach (LTGestureRecognizer recognizer in readyToFireRecognizers)
			{
				//s_log.InfoFormat("OnPointerUp - TouchesEnded for {0}", recognizer);
				LTGestureRecState newState = recognizer.TouchesEnded(touches);

				// update the state of the recognizer:
				//s_log.InfoFormat("OnPointerUp - TouchesEnded for {0} returned {1}", recognizer, newState);
				recognizer.State = newState;

				// 4.1) Reset the state of the gesture recognizer so that it is ready for another gesture:
				recognizer.State = LTGestureRecState.Possible;
			}

			// 5) now, check the single action recognizers (avoid managing the DoubleTap that is already managed in OnDoubleTapped):
			IEnumerable<LTGestureRecognizer> singleActionRecognizers = m_gestureRecognizers.Where(gr => gr.IsSingleAction && gr.NonFinalState &&
					(gr.Type != LTGestureType.DoubleTap) && (gr.SupportedTouchNumber == touches.Length));

			foreach (LTGestureRecognizer recognizer in singleActionRecognizers)
			{
				// avoid managing the DoubleTap that is already managed in OnDoubleTapped,
				// and the continuous recognizers which are still in Possible state:
				//s_log.InfoFormat("OnPointerUp - SingleAction TouchesEnded for {0}", recognizer);
				LTGestureRecState newStateSingleAction = recognizer.TouchesEnded(touches);

				// update the state of the recognizer:
				//s_log.InfoFormat("OnPointerUp - SingleAction TouchesEnded for {0} returned {1}", recognizer, newStateSingleAction);
				recognizer.State = newStateSingleAction;

				// 2.5.1) Reset the state of the gesture recognizer so that it is ready for another gesture:
				recognizer.State = LTGestureRecState.Possible;
			}

			// 6) Clear the state of the pointer:
			m_pointerStates.Remove(newTouch.PointerId);

			//s_log.InfoFormat("OnPointerUp - Last pointer state: {0}", PrintPointerStates());

			// 7) If there's no more touch, let's reset the states of all the gesture recognizers:
			if (m_pointerStates.Count == 0)
				ResetState();

			ProfilerUp.CollectResults(pointer);
		}

		private void OnPointerCaptureLost(object sender, PointerEventArgs e)
		{
			int pointer = ProfilerCapLost.Start();
			//if (e.Handled) TODO CHECK
			//	return;

			// for some mysterious reason this is fired when the user puts a second finger on the screen.
			// So, disregard this case:
			if (e.CurrentPoint.IsInContact)
			{
				//m_inputSource.SetPointerCapture();
				e.Handled = true;
				return;
			}

			uint pointerId = e.CurrentPoint.PointerId;
			//s_log.InfoFormat("OnPointerCaptureLost {0}, {1}", e.GetCurPoint(null), pointerId);

			// Clear the state of the pointer (and keep track of this touch:
			m_lastTouchLost = RefreshPointerState(e, false, out _);
			if (m_lastTouchLost != null)
				m_pointerStates.Remove(pointerId);

			// If there's no more touch, let's reset the states of all the gesture recognizers:
			if (m_pointerStates.Count == 0)
				ResetState();

			e.Handled = true;

			ProfilerCapLost.CollectResults(pointer);
		}

		///// <summary>
		///// This is the only event manager done directly on the UI Thread, "old style".
		///// </summary>
		///// <param name="sender"></param>
		///// <param name="e"></param>
		//private void OnDoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
		//{
		//	//s_log.InfoFormat("OnDoubleTapped {0}", e.GetPosition(null));

		//	// check if there is a recognizer for the double tap:
		//	LTGestureRecognizer doubleTapGR = m_gestureRecognizers.FirstOrDefault(gr => gr.Type == LTGestureType.DoubleTap);
		//	if (doubleTapGR != null)
		//	{
		//		doubleTapGR.TouchesEnded(new LTTouch[] { LTEventUtils.ToLTTouch(e) });

		//		// make sure the Tap, if any, recognizer fails:
		//		LTGestureRecognizer tapGR = m_gestureRecognizers.FirstOrDefault(gr => gr.Type == LTGestureType.Tap);
		//		if (tapGR != null)
		//			tapGR.State = LTGestureRecState.Failed;
		//	}

		//	// If there's no more touch, let's reset the states of all the gesture recognizers:
		//	if (m_pointerStates.Count == 0)
		//		ResetState();
		//}

		#endregion Event Managers
		///////////////////////////////////////////////////////////////////////////

		#region Private Utilities
		///////////////////////////////////////////////////////////////////////////

		protected virtual LTTouch[] GetTouches()
		{
			LTTouch[] res = new LTTouch[m_pointerStates.Count];
			int index = 0;
			foreach (LTTouch touch in m_pointerStates.Values)
			{
				res[index++] = touch;
			}

			return res;
		}

		private LTTouch RefreshPointerState(PointerEventArgs e, bool createIfNotExisting, out bool isDoubleClick)
		{
			// 1) check if we already have this pointer under management:
			uint pointerId = e.CurrentPoint.PointerId;
			LTTouch existingState = GetTouch(pointerId);
			if ((existingState == null) && !createIfNotExisting)
			{
				isDoubleClick = false;
				return null;
			}

			// 2) Create an LTTouch to represent this event:
			LTTouch newTouch = e.ToLTTouch(existingState, null);
			if (newTouch == null)
			{
				isDoubleClick = false;
				return null;
			}
			isDoubleClick = (newTouch.CurrentTime - m_lastPointerDownTime).TotalMilliseconds < MillisecondsForDoubleClick;

			// 3) Add to the state of the pointers:
			m_pointerStates[pointerId] = newTouch;

			return newTouch;
		}

		private static async Task<LTGestureRecState> ManageBeganGesture(LTGestureRecognizer recognizer, LTTouch[] touches, LTTouch newTouch)
		{
			LTGestureRecState newState = await recognizer.TouchesBegan(touches);
			if (newState == LTGestureRecState.Began)
			{
				// update the info of the touch:
				newTouch.LocationWhenGestureWasRec = newTouch.Current;
				newTouch.StartTime = DateTime.Now;
			}

			return newState;
		}

		private void MakeTransitionsForWrongTouchNumbers(LTTouch[] touches)
		{
			IEnumerable<LTGestureRecognizer> recognizersToEnd = m_gestureRecognizers.Where(gr => gr.NonFinalState && (gr.SupportedTouchNumber < touches.Length));
			foreach (LTGestureRecognizer recognizer in recognizersToEnd)
			{
				MakeTransition(recognizer, touches);
			}
		}

		/// <summary>
		/// Returns the list of recognizers that can continue the action.
		/// </summary>
		/// <param name="interestingRecognizers"></param>
		/// <param name="isEndAction"></param>
		/// <returns></returns>
		private IEnumerable<LTGestureRecognizer> ReadyToFireRecognizers(IEnumerable<LTGestureRecognizer> interestingRecognizers, bool isEndAction)
		{
			// get all running gesture recognizers:
			LTGestureRecognizer[] runningGR = m_gestureRecognizers.Where(gr => gr.IsRunning).ToArray();

			// if nothing is running, obviously all the recognizers in input can fire:
			if (runningGR.IsNullOrEmpty())
				return interestingRecognizers;

			List<LTGestureRecognizer> res = new List<LTGestureRecognizer>();

			// compare all the recognizers against each other:
			foreach (LTGestureRecognizer toStart in interestingRecognizers)
			{
				// let the HoldPageDown finish its job if we are at the end of the gesture:
				if (!(isEndAction && (toStart.Type == LTGestureType.HoldPageDown)))
				{
					// nobody can fire if pinch-and-collapse is on (except the pinch-and-collapse itself):
					if ((toStart.Type != LTGestureType.PinchAndZoom) && runningGR.Any(gr => gr.Type == LTGestureType.PinchAndZoom))
						continue;

					// nobody can fire if pull-excerpt is on (except the pull-excerpt itself):
					if ((toStart.Type != LTGestureType.ExcerptCreator) && runningGR.Any(gr => gr.Type == LTGestureType.ExcerptCreator))
						continue;
				}

				switch (toStart.Type)
				{
					case LTGestureType.Pan:
						if (runningGR.All(gr => gr.Type != LTGestureType.TextSelect))
							res.Add(toStart);
						break;

					case LTGestureType.PinchAndZoom:
						// the pinch-and-collapse-and-zoom can start always (except when there is already a PullExcerpt action on, which is covered previously):
						res.Add(toStart);
						break;

					case LTGestureType.TextSelect:
						if (runningGR.All(gr => gr.Type != LTGestureType.Pan))
							res.Add(toStart);
						break;

					case LTGestureType.ExcerptCreator:
						if (runningGR.All(gr => gr.Type != LTGestureType.TextSelect))
							res.Add(toStart);
						break;

					case LTGestureType.HoldPageDown:
						// let the HoldPageDown finish its job if we are at the end of the gesture:
						if (isEndAction || runningGR.All(gr => gr.Type != LTGestureType.TextSelect))
							res.Add(toStart);
						break;

					case LTGestureType.DoubleTap:
						if (runningGR.IsNullOrEmpty() || runningGR.All(gr => gr.Type == LTGestureType.DoubleTap))
							res.Add(toStart);
						break;

					case LTGestureType.Tap:
						// the tap can only start if the HoldPageDown is the only running recognizer:
						if (runningGR.IsNullOrEmpty() ||
								runningGR.All(gr => (gr.Type == LTGestureType.Tap) || (gr.Type == LTGestureType.HoldPageDown)))
							res.Add(toStart);
						break;

					case LTGestureType.PencilEraser:
						res.Add(toStart);
						break;
				}
			}

			return res;
		}

		//private void FillTouch(LTTouchEvent touchEv, PointerEventArgs args, LTTouch existingState)
		//{
		//	Point curPos = args.CurrentPoint.Position;
		//	LTPoint currentPos = Encoders.BuildLTPoint(curPos.X, curPos.Y);
		//	DateTime now = DateTime.Now;

		//	if (existingState == null)
		//	{
		//		touchEv.StartTime = now;
		//		touchEv.IsEnded = false;
		//		touchEv.Last = currentPos;
		//		touchEv.LocationWhenGestureWasRec = currentPos;
		//		touchEv.Start = currentPos;
		//		touchEv.Velocity = Encoders.POINT_ZERO;
		//	}
		//	else
		//	{
		//		touchEv.Copy(existingState);
		//		touchEv.CurrentTime = now;

		//		// compute the velocity:
		//		double timeDiff = Math.Max(LTEventUtils.MAX_TIME_DIFF, (now - existingState.CurrentTime).TotalSeconds); // in seconds
		//		touchEv.Velocity = Encoders.BuildLTPoint((currentPos.X - existingState.X) / timeDiff,
		//			(currentPos.Y - existingState.Y) / timeDiff);

		//		touchEv.Last = existingState.Current;
		//	}

		//	touchEv.CurrentTime = now;
		//	touchEv.Current = currentPos;
		//}

		private static void MakeTransition(LTGestureRecognizer currentGR, LTTouch[] touches)
		{
			currentGR.TouchesEnded(touches);
			currentGR.State = LTGestureRecState.Ended;
		}

		private string PrintRecognizers()
		{
			StringBuilder sb = new StringBuilder();
			foreach (LTGestureRecognizer recognizer in m_gestureRecognizers)
			{
				sb.AppendFormat("[{0}] |", recognizer);
			}

			return sb.ToString();
		}

		private string PrintPointerStates()
		{
			StringBuilder sb = new StringBuilder();
			foreach ((uint pId, LTTouch touch) in m_pointerStates)
			{
				sb.AppendFormat("{0}={1} |", pId, touch);
			}

			return sb.ToString();
		}

		public string PrintEngineState()
		{
			return string.Format("Touches {0} ; Recognizers: {1}", PrintPointerStates(), PrintRecognizers());
		}

		#endregion Private Utilities
		///////////////////////////////////////////////////////////////////////////
	}
}
