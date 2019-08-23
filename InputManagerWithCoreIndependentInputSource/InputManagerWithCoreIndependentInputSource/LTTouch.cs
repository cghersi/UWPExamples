//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// This class contains all the information related to a generic touch that could result useful for a gesture recognizer.
	/// </summary>
	public class LTTouch
	{
		public LTTouch()
		{
			TouchUID = Guid.NewGuid();
		}

		/// <summary>
		/// ID of the physical pointer (e.g. Mouse is always 1, fingers and pens vary).
		/// </summary>
		public uint PointerId { get; set; }

		/// <summary>
		/// Unique Identifier for a touch that is valid in a single session of a gesture.
		/// </summary>
		public Guid TouchUID { get; private set; }

		/// <summary>
		/// Current position of the pointer.
		/// </summary>
		public LTPoint Current { get; set; } = Encoders.POINT_NULL;

		/// <summary>
		/// Current X position of the pointer.
		/// </summary>
		public double X => Current.Equals(Encoders.POINT_NULL) ? 0 : Current.X;

		/// <summary>
		/// Current Y position of the pointer.
		/// </summary>
		public double Y => Current.Equals(Encoders.POINT_NULL) ? 0 : Current.Y;

		/// <summary>
		/// Initial position of the pointer, when the gesture began.
		/// i.e. where the user put the finger down.
		/// </summary>
		public LTPoint Start { get; set; } = Encoders.POINT_NULL;

		/// <summary>
		/// Initial X position of the pointer, when the gesture began.
		/// i.e. where the user put the finger down.
		/// </summary>
		public double StartX => Start.Equals(Encoders.POINT_NULL) ? 0 : Start.X;

		/// <summary>
		/// Initial Y position of the pointer, when the gesture began.
		/// i.e. where the user put the finger down.
		/// </summary>
		public double StartY => Start.Equals(Encoders.POINT_NULL) ? 0 : Start.Y;

		/// <summary>
		/// The initial moment when the gesture began.
		/// </summary>
		public DateTime StartTime { get; set; }

		/// <summary>
		/// The instant when this current touch has been recognized.
		/// </summary>
		public DateTime CurrentTime { get; set; }

		/// <summary>
		/// Last known position of the pointer (before this action).
		/// </summary>
		public LTPoint Last { get; set; } = Encoders.POINT_NULL;

		/// <summary>
		/// Last known X position of the pointer (before this action).
		/// i.e. where the pointer was when the gesture has passed to state "Finished". 
		/// </summary>
		public double LastX => Last.Equals(Encoders.POINT_NULL) ? 0 : Last.X;

		/// <summary>
		/// Last known Y position of the pointer (before this action).
		/// i.e. where the pointer was when the gesture has passed to state "Finished". 
		/// </summary>
		public double LastY => Last.Equals(Encoders.POINT_NULL) ? 0 : Last.Y;

		/// <summary>
		/// Current vector speed of the pointer. X is horizontal, Y is vertical.
		/// </summary>
		public LTPoint Velocity { get; set; } = Encoders.POINT_NULL;

		/// <summary>
		/// Current horizontal speed of the pointer.
		/// </summary>
		public double VelocityX => Velocity.Equals(Encoders.POINT_NULL) ? 0 : Velocity.X;

		/// <summary>
		/// Current vertical speed of the pointer.
		/// </summary>
		public double VelocityY => Velocity.Equals(Encoders.POINT_NULL) ? 0 : Velocity.Y;

		/// <summary>
		/// The location of the pointer when the gesture passed from state Possible to Recognized / Began.
		/// i.e. where the pointer was when the gesture has passed to state "Began".
		/// </summary>
		public LTPoint LocationWhenGestureWasRec { get; set; } = Encoders.POINT_NULL;

		/// <summary>
		/// The X location of the pointer when the gesture passed from state Possible to Recognized / Began.
		/// i.e. where the pointer was when the gesture has passed to state "Began".
		/// </summary>
		public double XWhenGestureWasRec => LocationWhenGestureWasRec.Equals(Encoders.POINT_NULL) ? 0 : LocationWhenGestureWasRec.X;

		/// <summary>
		/// The Y location of the pointer when the gesture passed from state Possible to Recognized / Began.
		/// i.e. where the pointer was when the gesture has passed to state "Began".
		/// </summary>
		public double YWhenGestureWasRec => LocationWhenGestureWasRec.Equals(Encoders.POINT_NULL) ? 0 : LocationWhenGestureWasRec.Y;

		/// <summary>
		/// Defines whether the gesture has finished.
		/// </summary>
		public bool IsEnded { get; set; }

		/// <summary>
		/// Type of pointer for this current touch.
		/// </summary>
		public PointerType PointerType { get; set; }

		/// <summary>
		/// Pressure applied by the pointer to the screen. Valid only for Touch and Pen.
		/// </summary>
		public float Pressure { get; set; }

		/// <summary>
		/// Angle measured between the Pen and the screen, along X axis.
		/// </summary>
		public float XTilt { get; set; }

		/// <summary>
		/// Angle measured between the Pen and the screen, along Y axis.
		/// </summary>
		public float YTilt { get; set; }

		/// <summary>
		/// Tells whether the current point is in contact or not. Valid only for Pen.
		/// </summary>
		public bool IsInContact { get; set; }

		/// <summary>
		/// Which button has been pressed/released.
		/// </summary>
		public LTPointerUpdateKind UpdateKind { get; set; }

		public LTPoint DiffFromStart
		{
			get
			{
				if (Start.Equals(Encoders.POINT_NULL))
					return Current;
				else if (Current.Equals(Encoders.POINT_NULL))
					return Start;
				else
					return Encoders.BuildLTPoint(Current.X - Start.X, Current.Y - Start.Y);
			}
		}

		/// <summary>
		/// Performs a deep copy of the current object.
		/// </summary>
		/// <returns></returns>
		public virtual void Copy(LTTouch input)
		{
			TouchUID = input.TouchUID;
			Current = input.Current;
			CurrentTime = input.CurrentTime;
			IsEnded = input.IsEnded;
			Start = input.Start;
			Last = input.Last;
			LocationWhenGestureWasRec = input.LocationWhenGestureWasRec;
			StartTime = input.StartTime;
			Velocity = input.Velocity;
			PointerId = input.PointerId;
			PointerType = input.PointerType;
			Pressure = input.Pressure;
			XTilt = input.XTilt;
			YTilt = input.YTilt;
		}

		public override string ToString()
		{
			return string.Format("[{0} ; {1} ; {2}]", PointerId, Current, Start);
		}
	}

	public enum PointerType
	{
		Mouse,
		Pen,
		Eraser,
		Finger
	}

	public enum LTPointerUpdateKind
	{
		/// <summary>Pointer updates not identified by other PointerUpdateKind values.</summary>
		Other,
		/// <summary>Left button pressed.</summary>
		LeftButtonPressed,
		/// <summary>Left button released.</summary>
		LeftButtonReleased,
		/// <summary>Right button pressed.</summary>
		RightButtonPressed,
		/// <summary>Right button released.</summary>
		RightButtonReleased,
		/// <summary>Middle button pressed.</summary>
		MiddleButtonPressed,
		/// <summary>Middle button released.</summary>
		MiddleButtonReleased,
		/// <summary>XBUTTON1 pressed.</summary>
		XButton1Pressed,
		/// <summary>XBUTTON1 released.</summary>
		XButton1Released,
		/// <summary>XBUTTON2 pressed.</summary>
		XButton2Pressed,
		/// <summary>XBUTTON2 released.</summary>
		XButton2Released,
	}
}
