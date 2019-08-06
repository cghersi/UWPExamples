using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultipleTouches
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly GestureRecognizerEngine m_engine = new GestureRecognizerEngine();

		public MainPage()
		{
			this.InitializeComponent();

			for (byte i = 0; i < 10; i++)
			{
				byte colorCode = (byte)(i * 20);
				Rectangle page = new Rectangle()
				{
					Width = 380,
					Height = 290,
					Margin = new Thickness(10, 10 + (i * 300), 0, 0),
					Fill = new SolidColorBrush(Color.FromArgb(255, colorCode, colorCode, colorCode)),
					Name = "Page " + i
				};
				ScrollerContent.Children.Add(page);
			}

			Results.Text = "Test results to be received.";

			m_engine.MillisecondsForDoubleClick = 200;
			m_engine.AttachEvents(Scroller);
			m_engine.ResetState();
		}

		private void FetchLogClick(object sender, RoutedEventArgs e)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string log in GestureRecognizerEngine.Logs)
			{
				sb.AppendLine(log);
			}
			Results.Text = sb.ToString();
			GestureRecognizerEngine.Logs.Clear();
		}

		private void ResetClick(object sender, RoutedEventArgs e)
		{
			m_engine.ResetState();
		}
	}
}
