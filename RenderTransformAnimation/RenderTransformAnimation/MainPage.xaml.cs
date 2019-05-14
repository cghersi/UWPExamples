using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RenderTransformAnimation
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly ScrollViewer m_scrollView = new ScrollViewer()
		{
			Name = "ScrollViewerForWrkPage",
			MaxZoomFactor = 5,      
			MinZoomFactor = 0.1f, 
			HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
			VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
			IsHorizontalRailEnabled = true,
			IsVerticalRailEnabled = true,
			ZoomMode = ZoomMode.Enabled,
			IsScrollInertiaEnabled = true,
			IsZoomInertiaEnabled = true,
			AllowDrop = true
		};

		private readonly Canvas m_zoomView = new Canvas()
		{
			Name = "ZoomViewForWrkPage",
			Width = 1000,
			Height = 1000,
			IsDoubleTapEnabled = true
		};

		private readonly CompositeTransform m_zoomViewTransform = new CompositeTransform();

		public MainPage()
		{
			this.InitializeComponent();

			// Initialize the data structures:
			m_scrollView.Content = m_zoomView;
			m_zoomView.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
			m_zoomView.ManipulationDelta += OnManipulationDelta;
			m_zoomView.RenderTransform = m_zoomViewTransform;
			m_zoomView.Background = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Anger.png", UriKind.RelativeOrAbsolute))
			};
			this.Workspace.Children.Add(m_scrollView);
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			SetEffectiveOffsetOfScrollView(new Point(m_zoomViewTransform.TranslateX + e.Delta.Translation.X,
				m_zoomViewTransform.TranslateY + e.Delta.Translation.Y), false);
			e.Handled = true;
		}

		private void SetEffectiveOffsetOfScrollView(Point newOffset, bool isAnimated)
		{
			if (isAnimated)
			{
				//LTAnimate(m_zoomViewTransform.TranslateX, newOffset.X, 200, v => m_zoomViewTransform.TranslateX = v);
				//LTAnimate(m_zoomViewTransform.TranslateY, newOffset.Y, 200, v => m_zoomViewTransform.TranslateY = v);

				TimeSpan dur = TimeSpan.FromSeconds(0.2);
				Storyboard sb = new Storyboard { Duration = dur };
				DoubleAnimation animationX = new DoubleAnimation
				{
					To = newOffset.X,
					Duration = dur,
					AutoReverse = false
				};
				DoubleAnimation animationY = new DoubleAnimation
				{
					To = newOffset.Y,
					Duration = dur,
					AutoReverse = false
				};
				sb.Children.Add(animationX);
				sb.Children.Add(animationY);
				Storyboard.SetTarget(animationX, m_zoomViewTransform);
				Storyboard.SetTarget(animationY, m_zoomViewTransform);
				Storyboard.SetTargetProperty(animationX, "CompositeTransform.TranslateX");
				Storyboard.SetTargetProperty(animationY, "CompositeTransform.TranslateY");

				//Storyboard.SetTarget(animationX, m_zoomView);
				//Storyboard.SetTarget(animationY, m_zoomView);
				//Storyboard.SetTargetProperty(animationX, "(UIElement.RenderTransform).(CompositeTransform.TranslateX)");
				//Storyboard.SetTargetProperty(animationY, "(UIElement.RenderTransform).(CompositeTransform.TranslateY)");

				sb.Completed += (sender, o) =>
				{
					sb.Stop();
					m_zoomViewTransform.TranslateX = newOffset.X;
					m_zoomViewTransform.TranslateY = newOffset.Y;
				};

				sb.Begin();

			}
			else
			{
				m_zoomViewTransform.TranslateX = newOffset.X;
				m_zoomViewTransform.TranslateY = newOffset.Y;
			}
		}

		private const int ANIMATION_TICK = 10; // in milliseconds

		private void LTAnimate(double initialValue, double finalValue, int durationInMillis, Action<double> updater, Action whenDone = null)
		{
			// compute the number of executions of the updater action:
			double delta = finalValue - initialValue;
			int execNum = durationInMillis / ANIMATION_TICK;
			DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(ANIMATION_TICK) };
			int execCount = 0;
			timer.Tick += (sender, o) =>
			{
				// execute the action of the animation:
				updater(initialValue + (delta * execCount) / execNum);

				// check if we are done with the animation:
				if (++execCount >= execNum)
				{
					// we are done: 
					// 1) make sure we set the very final value:
					updater(finalValue);

					// 2) execute the whenDone action, if any:
					whenDone?.Invoke();

					// 3) stop the timer:
					timer.Stop();
				}
			};
			timer.Start();
		}

		private void UIElement_OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			SetEffectiveOffsetOfScrollView(new Point(20, 50), true);
		}
	}
}
