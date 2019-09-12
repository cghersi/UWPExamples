using System.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CustomTitleBar
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();

			// Hide default title bar.
			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
			coreTitleBar.ExtendViewIntoTitleBar = true;
			UpdateTitleBarLayout(coreTitleBar);

			// Set XAML element as a draggable region.
			Window.Current.SetTitleBar(AppTitleBar);
			AppTitleBar.PointerPressed += AppTitleBar_PointerPressed;
			AppTitleBar.PointerMoved += AppTitleBar_PointerMoved;
			AppTitleBar.PointerReleased += AppTitleBar_PointerReleased;

			// Register a handler for when the size of the overlaid caption control changes.
			// For example, when the app moves to a screen with a different DPI.
			coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

			// Register a handler for when the title bar visibility changes.
			// For example, when the title bar is invoked in full screen mode.
			coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
		}

		private void AppTitleBar_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			Debug.WriteLine("Released at " + e.GetCurrentPoint(null).Position);
		}

		private void AppTitleBar_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			Debug.WriteLine("Moved at " + e.GetCurrentPoint(null).Position);
		}

		private void AppTitleBar_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			Debug.WriteLine("Picked at " + e.GetCurrentPoint(null).Position);
		}

		private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
		{
			UpdateTitleBarLayout(sender);
		}

		private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
		{
			// Get the size of the caption controls area and back button 
			// (returned in logical pixels), and move your content around as necessary.
			LeftPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayLeftInset);
			RightPaddingColumn.Width = new GridLength(coreTitleBar.SystemOverlayRightInset);
			//TitleBarButton.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);

			// Update title bar control size as needed to account for system size changes.
			AppTitleBar.Height = coreTitleBar.Height;
		}

		private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
		{
			if (sender.IsVisible)
			{
				AppTitleBar.Visibility = Visibility.Visible;
			}
			else
			{
				AppTitleBar.Visibility = Visibility.Collapsed;
			}
		}
		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("Click!");
		}
	}
}
