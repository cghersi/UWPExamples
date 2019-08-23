//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// This class contains all the information related to a generic touch event that could result
	/// useful for a gesture recognizer.
	/// All the positions refers to the SwapChainPanel that generated the event.
	/// </summary>
	public struct LTTouchEvent
	{
		public LTTouchEvent(LTGestureRecognizerEngine engine, LTTouchEventType type, LTTouch touch, bool isDoubleClick)
		{
			Engine = engine;
			Touch = touch;
			EventType = type;
			IsDoubleClick = isDoubleClick;
		}
		
		/// <summary>
		/// The Gesture Recognizer Engine that should manage this event.
		/// </summary>
		public LTGestureRecognizerEngine Engine { get; set; }

		/// <summary>
		/// Container of all the information needed from a Pointer.
		/// </summary>
		public LTTouch Touch { get; set; }

		/// <summary>
		/// The type of event to manage.
		/// </summary>
		public LTTouchEventType EventType { get; set; }

		/// <summary>
		/// If the current touch is the result of a double tap.
		/// </summary>
		public bool IsDoubleClick { get; set; }

		public override string ToString()
		{
			return string.Format("{0} => {1}", EventType, Touch);
		}
	}

	public enum LTTouchEventType
	{
		Down,
		Moved, 
		Up
	}
}
