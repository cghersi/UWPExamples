//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml.Input;

namespace MultipleTouches
{
	public class Touch
	{
		public Touch()
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
		public Point Current { get; set; }

		/// <summary>
		/// Current X position of the pointer.
		/// </summary>
		public double X => Current.X;

		/// <summary>
		/// Current Y position of the pointer.
		/// </summary>
		public double Y => Current.Y;

		/// <summary>
		/// Initial position of the pointer, when the gesture began.
		/// i.e. where the user put the finger down.
		/// </summary>
		public Point Start { get; set; }

		/// <summary>
		/// Initial X position of the pointer, when the gesture began.
		/// i.e. where the user put the finger down.
		/// </summary>
		public double StartX => Start.X;

		/// <summary>
		/// Initial Y position of the pointer, when the gesture began.
		/// i.e. where the user put the finger down.
		/// </summary>
		public double StartY => Start.Y;

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
		public Point Last { get; set; }

		/// <summary>
		/// Last known X position of the pointer (before this action).
		/// i.e. where the pointer was when the gesture has passed to state "Finished". 
		/// </summary>
		public double LastX => Last.X;

		/// <summary>
		/// Last known Y position of the pointer (before this action).
		/// i.e. where the pointer was when the gesture has passed to state "Finished". 
		/// </summary>
		public double LastY => Last.Y;

		/// <summary>
		/// Current vector speed of the pointer. X is horizontal, Y is vertical.
		/// </summary>
		public Point Velocity { get; set; }

		/// <summary>
		/// Current vertical speed of the pointer.
		/// </summary>
		public double VelocityY => Velocity.Y;

		/// <summary>
		/// Current horizontal speed of the pointer.
		/// </summary>
		public double VelocityX => Velocity.X;

		/// <summary>
		/// The location of the pointer when the gesture passed from state Possible to Recognized / Began.
		/// i.e. where the pointer was when the gesture has passed to state "Began".
		/// </summary>
		public Point LocationWhenGestureWasRec { get; set; }

		/// <summary>
		/// The X location of the pointer when the gesture passed from state Possible to Recognized / Began.
		/// i.e. where the pointer was when the gesture has passed to state "Began".
		/// </summary>
		public double XWhenGestureWasRec => LocationWhenGestureWasRec.X;

		/// <summary>
		/// The Y location of the pointer when the gesture passed from state Possible to Recognized / Began.
		/// i.e. where the pointer was when the gesture has passed to state "Began".
		/// </summary>
		public double YWhenGestureWasRec => LocationWhenGestureWasRec.Y;

		/// <summary>
		/// Defines whether the gesture has finished.
		/// </summary>
		public bool IsEnded { get; set; }

		/// <summary>
		/// Original event that triggered this LTTouch.
		/// </summary>
		public PointerRoutedEventArgs EventArgs { get; set; }

		public Point DiffFromStart => new Point(Current.X - Start.X, Current.Y - Start.Y);

		/// <summary>
		/// Performs a deep copy of the current object.
		/// </summary>
		/// <returns></returns>
		[SuppressMessage("ReSharper", "ArrangeThisQualifier")]
		public Touch Copy()
		{
			return new Touch
			{
				TouchUID = this.TouchUID,
				Current = this.Current,
				CurrentTime = this.CurrentTime,
				EventArgs = this.EventArgs,
				IsEnded = this.IsEnded,
				Start = this.Start,
				Last = this.Last,
				LocationWhenGestureWasRec = this.LocationWhenGestureWasRec,
				StartTime = this.StartTime,
				Velocity = this.Velocity,
				PointerId = this.PointerId
			};
		}

		public override string ToString()
		{
			return string.Format("[{0} ; {1} ; {2}]", PointerId, Current, Start);
		}
	}
}
