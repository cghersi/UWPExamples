using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MoveShapeFilledByImageBrush
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly Canvas m_elementToMove = new Canvas()
		{
			Width = 100,
			Height = 100,
			Background = new SolidColorBrush(Colors.LightBlue)
		};

		public MainPage()
		{
			InitializeComponent();

			// add children to the main container:
			MainView.Children.Add(m_elementToMove);

			List<Point> maskInkStroke = new List<Point>()
			{
				new Point(0.5, 0), // start point
				new Point(0.75, 0.25), // control point 1
				new Point(1, 0.5), // point 1
				new Point(0.75, 0.75), // control point 2
				new Point(0.5, 1), // point 2
				new Point(0.25, 0.75), // control point 3
				new Point(0, 0.5), // point 3
				new Point(0.25, 0.25), // control point 4
				new Point(0.5, 0), // point 4 (= start point)
			};
			ImageWithMask imageWithMask = new ImageWithMask(m_elementToMove, 
				new Uri("ms-appx:///Assets/textselect.png", UriKind.RelativeOrAbsolute),
				maskInkStroke, new Size(100, 100));

			PointerManager pointerManager = new PointerManager(imageWithMask);
			m_elementToMove.PointerPressed += pointerManager.OnPointerDown;
			m_elementToMove.PointerMoved += pointerManager.OnPointerMoved;
			m_elementToMove.PointerReleased += pointerManager.OnPointerUp;
		}
	}
}
