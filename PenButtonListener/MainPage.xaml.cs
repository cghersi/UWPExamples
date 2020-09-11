using System;
using System.Diagnostics;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Input;

namespace PenButtonListener
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private int m_buttonPressedTimes = 0;

		public MainPage()
		{
			InitializeComponent();

			if (!ApiInformation.IsTypePresent("Windows.Devices.Input.PenButtonListener"))
				return;
			try
			{
				Windows.Devices.Input.PenButtonListener.GetDefault().TailButtonClicked += OnTailButtonClicked;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot support PenButtonListener due to {0}", e.Message);
			}
		}

		private void OnTailButtonClicked(Windows.Devices.Input.PenButtonListener sender, PenTailButtonClickedEventArgs args)
		{
			Results.Text = string.Format("Tail Button Pressed {0} times", ++m_buttonPressedTimes);
		}
	}
}
