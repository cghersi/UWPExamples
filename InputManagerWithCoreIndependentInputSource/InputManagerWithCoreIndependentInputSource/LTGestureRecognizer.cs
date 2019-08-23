//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// Defines a gesture recognizer, capable of maintaining the state of the gesture and decide
	/// which behavior to take according to the state and the interaction with other gesture recognizers.
	/// </summary>
	public abstract class LTGestureRecognizer
	{
		public virtual LTGestureRecState State { get; set; }

		public LTGestureType Type { get; }

		/// <summary>
		/// Defines whether this recognizer works with a single action (e.g. Tap, double tap, etc.)
		/// or requires a continuous interaction.
		/// </summary>
		public bool IsSingleAction { get; protected set; } = false;

		/// <summary>
		/// Determines the number of concurrent touches that this gesture needs
		/// in order to be successfully executed.
		/// </summary>
		public int SupportedTouchNumber { get; protected set; } = 1;

		protected LTGestureRecognizer(LTGestureType type)
		{
			Type = type;
			// ReSharper disable once VirtualMemberCallInConstructor
			State = LTGestureRecState.Possible;
		}

		/// <summary>
		/// Determines whether the gesture recognizer is performing an action (useful to
		/// check the behavior of other simultaneous gesture recognizers).
		/// </summary>
		public bool IsRunning => (State == LTGestureRecState.Began) || (State == LTGestureRecState.Changed);

		/// <summary>
		/// Determines whether the gesture recognizer is in a non-final state.
		/// </summary>
		public bool NonFinalState => (State == LTGestureRecState.Began) || (State == LTGestureRecState.Changed) || (State == LTGestureRecState.Possible);

		public virtual Task<LTGestureRecState> TouchesBegan(LTTouch[] touches)
		{
			// nothing to do here, extend in the children if needed
			return Task.FromResult(LTGestureRecState.Began);
		}

		public virtual Task<LTGestureRecState> TouchesMoved(LTTouch[] touches)
		{
			// nothing to do here, extend in the children if needed
			return Task.FromResult(LTGestureRecState.Changed);
		}

		public virtual LTGestureRecState TouchesEnded(LTTouch[] touches)
		{
			// nothing to do here, extend in the children if needed
			return LTGestureRecState.Ended;
		}

		public virtual LTGestureRecState TouchesCancelled(LTTouch[] touches)
		{
			// nothing to do here, extend in the children if needed
			return LTGestureRecState.Cancelled;
		}

		public override string ToString()
		{
			return string.Format("{0} => {1}", Type, State);
		}
	}
}
