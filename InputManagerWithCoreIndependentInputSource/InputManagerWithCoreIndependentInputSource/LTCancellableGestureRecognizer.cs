//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// Defines a gesture recognizer whose gesture can be cancelled.
	/// Useful e.g. for system gestures like Pan and ScrollView management, which we only want to cancel.
	/// </summary>
	public class LTCancellableGestureRecognizer : LTGestureRecognizer
	{
		protected Action m_cancelAction;
		private LTGestureRecState m_state;

		public override LTGestureRecState State
		{
			get => m_state;
			set
			{
				m_state = value;
				if ((m_state == LTGestureRecState.Cancelled) || (m_state == LTGestureRecState.Failed))
					m_cancelAction?.Invoke();
			}
		}

		public LTCancellableGestureRecognizer(Action cancelAction, LTGestureType type) : base(type)
		{
			m_cancelAction = cancelAction;
		}

		public override Task<LTGestureRecState> TouchesBegan(LTTouch[] touches)
		{
			return Task.FromResult(LTGestureRecState.Began);
		}

		public override Task<LTGestureRecState> TouchesMoved(LTTouch[] touches)
		{
			return Task.FromResult(LTGestureRecState.Changed);
		}

		public override LTGestureRecState TouchesEnded(LTTouch[] touches)
		{
			return LTGestureRecState.Ended;
		}

		public override LTGestureRecState TouchesCancelled(LTTouch[] touches)
		{
			m_cancelAction?.Invoke();
			return LTGestureRecState.Cancelled;
		}
	}
}
