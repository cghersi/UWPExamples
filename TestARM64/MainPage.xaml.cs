﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestARM64
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void GoodClick(object sender, RoutedEventArgs e)
		{
			Results.Text += " OK ";
		}

		private void BadClick(object sender, RoutedEventArgs e)
		{
			int a = 0;
			int b = 8;
			int c = b / a; // should throw exception

			Results.Text += " OK also Bad ";
		}
	}
}
