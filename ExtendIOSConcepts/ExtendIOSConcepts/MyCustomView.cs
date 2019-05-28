using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ExtendIOSConcepts
{
	public class MyCustomView : BaseView
	{
		private readonly BaseView m_panel1; 
		private readonly BaseView m_panel2; 
		private readonly BaseView m_panel3; 
		private readonly BaseView m_panel4;

		private static int s_count = 0;

		public MyCustomView() : base(true)
		{
			SetArrangeOverride(s =>
			{
				LayoutSubviews(s);
				return s;
			});

			m_panel4 = new BaseView();
			m_panel4.View.Background = new SolidColorBrush(Colors.AntiqueWhite);
			View.Children.Add(m_panel4.View);
			m_panel1 = new BaseView();
			m_panel1.View.Background = new SolidColorBrush(Colors.Red);
			View.Children.Add(m_panel1.View);
			m_panel2 = new BaseView();
			m_panel2.View.Background = new SolidColorBrush(Colors.Green);
			ShowPanel2(true);
			m_panel3 = new BaseView();
			m_panel3.View.Background = new SolidColorBrush(Colors.Yellow);
			View.Children.Add(m_panel3.View);
		}

		private void ShowPanel2(bool show)
		{
			if (show && !View.Children.Contains(m_panel2.View))
				View.Children.Add(m_panel2.View);
			else if (!show && View.Children.Contains(m_panel2.View))
				View.Children.Remove(m_panel2.View);
		}

		public override void LayoutSubviews(Size s)
		{
			Debug.WriteLine("{1} - In LayoutSubviews with size {0}", s, s_count++);

			// Position the first panel:
			//m_panel1.SetMargin(0, 0);
			//m_panel1.SetSize(10, Height);
			//m_panel1.RequestNewLayout();
			m_panel1.View.Arrange(new Rect(0, 0 /*x,y*/, 10, s.Height));

			// For the width of the panel, you can either use the value you assigned in the Arrange call above, or you can
			// use the ActualWidth property which will be set after Arrange().  (Similar for Height/ActualHeight.)
			//double leftMarginFromThisView = m_panel1.Width;
			double leftMarginFromThisView = m_panel1.ActualWidth;

			// 1. Update the second panel component:

			// The margin values:
			Thickness containerToContent = new Thickness(10 + leftMarginFromThisView, 10, 10, 10);

			// Position the second panel and set the correct width:
			//m_panel2.SetMargin(containerToContent.Left, containerToContent.Top);
			//m_panel2.SetSize(Width - containerToContent.Left - containerToContent.Right, null);

			// Now set the final heights of the components:
			double lastKnownPanel2Height = m_panel2.ActualHeight;

			// How much added height do we need for the third panel:
			double allowedWidth = s.Width - containerToContent.Left - containerToContent.Right;
			double panel3Height = allowedWidth / m_panel3.Width * m_panel3.Height;

			// Now set the final height of the second panel:
			//m_panel2?.SetSize(null, Math.Max(10, lastKnownPanel2Height));
			m_panel2.View.Arrange(new Rect(containerToContent.Left, containerToContent.Top,
				Math.Max(s.Width - containerToContent.Left - containerToContent.Right, 10),
				Math.Max(10, lastKnownPanel2Height)));
			//ShowPanel2(lastKnownPanel2Height > 0.1);

			// Set the final height:
			Rect rect = Frame; 
			double panel4MinHeight = rect.Y + rect.Height;

			double userRequestedMinHeight = 30;
			double finalH = Math.Max(panel3Height + lastKnownPanel2Height + containerToContent.Top + containerToContent.Bottom, Math.Max(panel4MinHeight, userRequestedMinHeight));
			if (!double.IsNaN(finalH) && !finalH.Equal(View.ActualHeight))
			{
				Debug.WriteLine("Set Height 2 to {0}", finalH);
				View.Height = finalH;
			}
			//SetSize(null, finalH);
			// Usually the size of the current view should already be settled by the time we get here,
			// since this function is called by Arrange() which has already provided the intended final
			// position and size. It is also too late at this point to affect the desired size measurement
			// of the parent panel, if it needs to determine a size. You should override MeasureOverride
			// if the parent panel should consider that in determining overall size.
			// Otherwise, you can also *return* and updated size from ArrangeOverride(), which will
			// obviously require a small update to the signature of this function.

			// Position the 4th panel:
			m_panel4.View.Arrange(new Rect(0, 0, s.Width, s.Height));
			//m_panel4.SetSize(Width, Height);
			//m_panel4.SetMargin(X, Y); // for internal reasons we need to call SetMargin twice...
			//m_panel4.SetMargin(0, 0);

			// Update the third panel again:
			//m_panel3.SetMargin(containerToContent.Left, containerToContent.Top + lastKnownPanel2Height);
			m_panel3.View.Arrange(new Rect(containerToContent.Left, containerToContent.Top + lastKnownPanel2Height, 10, 10 /* desired width/height? */));
		}
	}
}
