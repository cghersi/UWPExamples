using System;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace ExtendIOSConcepts
{
	public class BaseView
	{
		public BaseView(bool useCanvas = false)
		{
			string name = GetType().Name;
			if (useCanvas)
				View = new ArrangeableCanvas();
			else
				View = new ArrangeableRelativePanel();
			ClipsToBounds = !useCanvas;
			View.Name = name;
			View.AccessKey = name;
		}

		protected void SetArrangeOverride(Func<Size, Size> arrangeOverride)
		{
			if (View is IArrangeablePanel arrPanel)
				arrPanel.ArrangeFunc = arrangeOverride;
		}

		public Panel View { get; set; }

		public bool ClipsToBounds { get; }

		protected double X
		{
			get => View.Margin.Left;
			set => SetMargin(value, null);
		}

		protected double Y
		{
			get => View.Margin.Top;
			set => SetMargin(null, value);
		}

		public Point Origin => new Point(X, Y);

		public Rect Frame => new Rect(X, Y, Width, Height);

		public Guid Id { get; protected set; }

		public virtual double Width => View.Width;

		public virtual double Height => View.Height;

		public virtual double ActualWidth => View.ActualWidth;

		public virtual double ActualHeight => View.ActualHeight;

		public virtual Panel ParentForChildren => View;

		public virtual Panel PanelForBackground => View;

		/// <summary>
		/// Sets the properties related to the border of the View.
		/// </summary>
		/// <param name="thickness">null to keep as is, the new value otherwise (it will apply an homogeneous border thickness)</param>
		/// <param name="cornerRadius">null to keep as is, the new value otherwise (it will apply an homogeneous border thickness)</param>
		/// <param name="borderColor"></param>
		public virtual void SetBorder(double? thickness, double? cornerRadius, Brush borderColor)
		{
			// nothing to do here, expected to be overriden in the children classes
		}

		public void RequestNewLayout()
		{
			LayoutSubviews();
			View.InvalidateMeasure();
		}

		/// <summary>
		/// Releases all the resources used by this view.
		/// </summary>
		public virtual void Dealloc()
		{
			// nothing to do here, but can be overriden by children
		}

		public virtual void LayoutSubviews()
		{
			// nothing to do here, but can be overriden by children
		}

		public override string ToString()
		{
			return string.Format("[{0} - {1}]", Id, View.Name);
		}

		public virtual void SetMargin(double? left, double? top)
		{
			View.SetMargin(left, top);
		}

		public virtual void SetSize(double? width, double? height)
		{
			View.SetSize(width, height);
		}

		public void SetFrame(Rect newFrame)
		{
			SetMargin(newFrame.X, newFrame.Y);
			SetSize(newFrame.Width, newFrame.Height);
		}

		public virtual void Move(double deltaX, double deltaY)
		{
			View.Move(deltaX, deltaY);
		}
	}

	/// <summary>
	/// Extends the Canvas control to provide an override of the ArrangeOverride method.
	/// </summary>
	public class ArrangeableCanvas : Canvas, IArrangeablePanel
	{
		public Func<Size, Size> ArrangeFunc { get; set; }

		protected override Size ArrangeOverride(Size finalSize)
		{
			return ArrangeFunc?.Invoke(finalSize) ?? base.ArrangeOverride(finalSize);
		}
	}

	/// <summary>
	/// Extends the RelativePanel control to provide an override of the ArrangeOverride method.
	/// </summary>
	public class ArrangeableRelativePanel : RelativePanel, IArrangeablePanel
	{
		public Func<Size, Size> ArrangeFunc { get; set; }

		protected override Size ArrangeOverride(Size finalSize)
		{
			return ArrangeFunc?.Invoke(finalSize) ?? base.ArrangeOverride(finalSize);
		}
	}

	public interface IArrangeablePanel
	{
		Func<Size, Size> ArrangeFunc { get; set; }
	}
}
