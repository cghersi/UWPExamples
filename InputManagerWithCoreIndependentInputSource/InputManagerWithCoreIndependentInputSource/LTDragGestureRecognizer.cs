//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System.Threading.Tasks;

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// Defines a gesture recognizer that takes care of the dragging of an excerpt
	/// from a document to the workspace.
	/// </summary>
	public class LTDragGestureRecognizer : LTCancellableGestureRecognizer
	{
		private readonly ILTExcerptDraggable m_controller;

		public LTDragGestureRecognizer(ILTExcerptDraggable controller) : base(controller.CancelExcerptGesture, LTGestureType.ExcerptCreator)
		{
			m_controller = controller;
		}

		public override async Task<LTGestureRecState> TouchesBegan(LTTouch[] touches)
		{
			// Make sure the number of pointers is correct:
			if (touches.IsNullOrEmpty())
				return State;
			else if (touches.Length > 1)
				return LTGestureRecState.Failed;

			return (await m_controller.StartExcerptGesture(touches[0])) ? LTGestureRecState.Began : LTGestureRecState.Possible;
		}

		public override async Task<LTGestureRecState> TouchesMoved(LTTouch[] touches)
		{
			// Make sure the number of pointers is correct:
			if (touches.IsNullOrEmpty())
				return State;
			else if (touches.Length > 1)
				return LTGestureRecState.Failed;

			switch (State)
			{
				case LTGestureRecState.Possible:
					if (await m_controller.StartExcerptGesture(touches[0]))
						return LTGestureRecState.Began;
					break;
				case LTGestureRecState.Began:
				case LTGestureRecState.Changed:
					m_controller.ContinueExcerptGesture(touches[0]);
					break;
			}

			return State;
		}

		public override LTGestureRecState TouchesEnded(LTTouch[] touches)
		{
			// Make sure the number of pointers is correct:
			if (touches.IsNullOrEmpty())
				return State;
			else if (touches.Length > 1)
				return LTGestureRecState.Failed;

			if ((State == LTGestureRecState.Began) || (State == LTGestureRecState.Changed))
			{
				m_controller.EndExcerptGesture(touches[0]);
			}

			return LTGestureRecState.Ended;
		}

		public override LTGestureRecState TouchesCancelled(LTTouch[] touches)
		{
			m_controller.CancelExcerptGesture();
			return LTGestureRecState.Cancelled;
		}
	}

	/// <summary>
	/// Defines a controller that is able to manage the Pull-out-excerpt gesture.
	/// </summary>
	public interface ILTExcerptDraggable
	{
		/// <summary>
		/// Starts the gesture of Pull-out-excerpt.
		/// </summary>
		/// <param name="touch">touch point for this gesture</param>
		/// <returns>Returns true if the gesture is accepted;
		/// false if the gesture is NOT accepted (i.e. the scenario is not prepared to accept the gesture.)</returns>
		Task<bool> StartExcerptGesture(LTTouch touch);

		/// <summary>
		/// Continues the gesture of Pull-out-excerpt.
		/// </summary>
		/// <param name="touch">touch point for this gesture</param>
		/// <returns>Returns true if the gesture is correctly performed;
		/// false if there has been an error that prevents the gesture to continue</returns>
		bool ContinueExcerptGesture(LTTouch touch);

		/// <summary>
		/// Ends the gesture of Pull-out-excerpt.
		/// </summary>
		/// <param name="touch">touch point for this gesture</param>
		void EndExcerptGesture(LTTouch touch);

		/// <summary>
		/// Cancels the gesture of Pull-out-excerpt and releases all the related resources.
		/// </summary>
		void CancelExcerptGesture();
	}
}
