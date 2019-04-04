using System;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PositionElemRightBottom
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

		private readonly Button m_zoomAllButton = new Button()
		{
			Name = "ZoomBtn",
			Background = new SolidColorBrush(Colors.Black),
			Height = 100,
			Width = 100,
		};

		public MainPage()
		{
			InitializeComponent();

			// Initialize the data structures:
			m_scrollView.Content = m_zoomView;
			m_zoomView.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY | ManipulationModes.TranslateInertia;
			m_zoomView.ManipulationDelta += OnManipulationDelta;
			m_zoomView.RenderTransform = m_zoomViewTransform;
			m_zoomView.Background = new ImageBrush()
			{
				ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/Anger.png", UriKind.RelativeOrAbsolute))
			};
			Workspace.Children.Add(m_scrollView);
			Workspace.Children.Add(m_zoomAllButton);
			m_zoomAllButton.Content = new Image()
			{
				Source = new BitmapImage(new Uri("ms-appx:///Assets/zoomout_teal_1.png", UriKind.RelativeOrAbsolute))
			};
			m_zoomAllButton.PointerPressed += ZoomoutButtonAction;
			m_zoomAllButton.SetValue(Canvas.ZIndexProperty, 100);

			// with this margin and alignment, I expect to position the button on the right bottom:
			m_zoomAllButton.Margin = new Thickness(0, 0, 45, 45);

			// if we use XXAlignment, this is not the appearance when we start the App!!
			//HorizontalAlignment = HorizontalAlignment.Right,
			//VerticalAlignment = VerticalAlignment.Bottom  

			// instead, this makes the trick of aligning:
			RelativePanel.SetAlignRightWithPanel(m_zoomAllButton, true);
			RelativePanel.SetAlignBottomWithPanel(m_zoomAllButton, true);
		}

		private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			SetEffectiveOffsetOfScrollView(new Point(m_zoomViewTransform.TranslateX + e.Delta.Translation.X,
				m_zoomViewTransform.TranslateY + e.Delta.Translation.Y));
			e.Handled = true;
		}

		private void SetEffectiveOffsetOfScrollView(Point newOffset)
		{
			m_zoomViewTransform.TranslateX = newOffset.X;
			m_zoomViewTransform.TranslateY = newOffset.Y;
		}

		private void ZoomoutButtonAction(object sender, PointerRoutedEventArgs e)
		{
			SetEffectiveOffsetOfScrollView(new Point(50, 200));
		}

		private void PositionZoomAllBtn(object sender, PointerRoutedEventArgs e)
		{
			// Position the zoom-all button in an absolute way:
			double width = Workspace.ActualWidth;
			if (IsValidSize(width)) // make sure we actually have a valid size
			{
				double height = Workspace.ActualHeight;
				m_zoomAllButton.Margin = new Thickness(width - m_zoomAllButton.Width - 10, 
					height - m_zoomAllButton.Height - 10, 0, 0);
			}
		}

		private static bool IsValidSize(double arg)
		{
			return (arg > 0) && !double.IsNaN(arg);
		}
	}
}
