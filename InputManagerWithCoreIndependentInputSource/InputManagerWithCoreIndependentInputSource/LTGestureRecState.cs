//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------


namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// Defines the possible states of a gesture recognizer.
	/// </summary>
	public enum LTGestureRecState
	{
		Began,
		Possible,
		Cancelled,
		Failed,
		Ended,
		Changed
	}

	/// <summary>
	/// Defines the kind of gesture recognizer (faster than check the class).
	/// </summary>
	public enum LTGestureType
	{
		PinchAndZoom,
		TextSelect,
		ExcerptCreator,
		Tap,
		DoubleTap,
		HoldPageDown,
		PencilEraser,
		Pan
	}
}
