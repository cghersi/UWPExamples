using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SizeOfTextBox
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public static readonly FontFamily FONT_FAMILY = new FontFamily("Arial"); //"new FontFamily("Assets/Fonts/paltn.ttf#Palatino-Roman");
		public const int FONT_SIZE = 10;
		public const int LINE_HEAD_INDENT = 3;
		public const float LINE_SPACING = 1.08f;
		private static readonly Color FONT_FOREGROUND_COLOR = Color.FromArgb(255, 59, 52, 26);
		public static readonly SolidColorBrush FONT_FOREGROUND = new SolidColorBrush(FONT_FOREGROUND_COLOR);
		public const int MIN_WIDTH = 100;
		public const int MIN_HEIGHT = 50;

		private int offsetY = 100;

		private RichEditBox b;

		public MainPage()
		{
			InitializeComponent();
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			b = new RichEditBox
			{
				FontFamily = FONT_FAMILY,
				FontSize = FONT_SIZE,
				Padding = new Thickness(0),
				TextWrapping = TextWrapping.Wrap,
				Foreground = FONT_FOREGROUND,
				BorderThickness = new Thickness(0),
				AcceptsReturn = true,
				AllowFocusWhenDisabled = false
			};
			ScrollViewer.SetVerticalScrollBarVisibility(b, ScrollBarVisibility.Hidden);
			ScrollViewer.SetHorizontalScrollBarVisibility(b, ScrollBarVisibility.Hidden);

			b.Width = MIN_WIDTH;
			b.Height = MIN_HEIGHT;
			b.Margin = new Thickness(100, offsetY, 0, 0);
			offsetY += 100;
			MainContainer.Children.Add(b);

			InitHeight.Text = "Init H:" + GetElemHeight(b);
		}

		private void ComputeHeight_OnClick(object sender, RoutedEventArgs e)
		{
			FinalHeight.Text = "Final H:" + GetElemHeight(b);
		}

		public static double GetElemHeight(FrameworkElement elem, double? actualWidth = null)
		{
			if (elem == null)
				return 0;

			// take note of the existing height, if any, since we have to re-establish it later:
			double currentH = elem.Height;
			if (!double.IsNaN(currentH))
				elem.Height = double.NaN;
			double totalW = (actualWidth ?? elem.Width) + elem.Margin.Left + elem.Margin.Right;

			// Measure() only works as expected in this context if the Height is NaN:
			elem.Measure(new Size(totalW, Double.PositiveInfinity));
			Size size = elem.DesiredSize;
			elem.Height = currentH; //re-establish the correct height
			return size.Height - elem.Margin.Top - elem.Margin.Bottom;
		}
	}
}
