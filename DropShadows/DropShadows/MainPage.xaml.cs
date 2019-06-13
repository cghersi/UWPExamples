using System;
using System.Diagnostics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DropShadows
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();

			ShadowElem elem = new ShadowElem();
			MainContainer.Children.Add(elem.View);
		}
	}

	public class ShadowElem : IShadowPresenter
	{
		private DropShadowMetadata m_shadowOptions = new DropShadowMetadata(null);
		private Path m_bezierPath = null; //TODO in case you want to try with custom shapes

		private Point m_lastPoint = new Point(0, 0);
		private bool m_dragging = false;
		private readonly Image m_handle;

		public ShadowElem()
		{
			View = new Canvas()
			{
				Name = "ShadowPres1",
				Margin = new Thickness(200, 100, 0, 0),
				Background = new SolidColorBrush(Colors.Bisque),
				Width = 100,
				Height = 200, 
				AccessKey = "ShadowPres1"
			};

			m_handle = new Image()
			{
				Source = new BitmapImage(new Uri("ms-appx:///Assets/handle.png", UriKind.RelativeOrAbsolute)),
				Width = 40,
				Height = 40,
				Margin = new Thickness(80, 180, 0, 0)
			};
			View.Children.Add(m_handle);
			m_handle.PointerPressed += OnPointerPressed;
			m_handle.PointerReleased += OnPointerReleased;
			m_handle.PointerMoved += OnPointerMoved;

			// set up the shadow for the element:
			m_shadowOptions.Shape = m_bezierPath;
			this.ManageShadow(true, View, ref m_shadowOptions);
		}

		public Path ShapeForShadow => m_bezierPath;
		public Panel View { get; }

		private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			m_dragging = true;
			(sender as FrameworkElement)?.CapturePointer(e.Pointer);
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.SizeNortheastSouthwest, 0);
		}

		private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
		{
			m_dragging = false;
			Release(sender, e);
		}

		private static void Release(object sender, PointerRoutedEventArgs e)
		{
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
			FrameworkElement elem = sender as FrameworkElement;
			elem?.ReleasePointerCapture(e.Pointer);
		}

        static bool shadowInitialSizeEvaluated = false;

		private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
		{
			if (!m_dragging)
			{
				Release(sender, e);
				return;
			}

			// update the size of the element:
			Point curPoint = e.GetCurrentPoint(null).Position;

			if (shadowInitialSizeEvaluated)
			{
				double deltaX = curPoint.X - m_lastPoint.X;
				double deltaY = curPoint.Y - m_lastPoint.Y;

                // Only update the size if it's a valid size
                if (View.Width + deltaX > 20.0f &&
                    View.Height + deltaY > 20.0f
                    )
                {
                    View.Width += deltaX;
                    View.Height += deltaY;

                    m_handle.Margin = new Thickness(m_handle.Margin.Left + deltaX, m_handle.Margin.Top + deltaY, 0, 0);

                    m_lastPoint = curPoint;

                    // update the shadow:
                    this.ManageShadow(true, View, ref m_shadowOptions);
                }
            }
            else
            {
                m_lastPoint = curPoint;
            }

            shadowInitialSizeEvaluated = true;

		}
	}
}
