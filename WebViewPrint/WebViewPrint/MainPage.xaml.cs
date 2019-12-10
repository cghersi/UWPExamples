using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WebViewPrint
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private PrintHelper printHelper;

		public MainPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Initialize common helper class and register for printing
			printHelper = new PrintHelper(this);
			printHelper.RegisterForPrinting();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			printHelper?.UnregisterForPrinting();
		}

		private async void Web_LoadCompleted(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
		{
			// Initialize print content for this scenario
			Canvas printingContainer = new Canvas()
			{
				Width = 5000,
				Height = 20000,
				Background = await GetWebViewBrush(WebPages)
			};

			printHelper.PreparePrintContent(printingContainer);

			if (Windows.Graphics.Printing.PrintManager.IsSupported())
			{
				try
				{

					// Show print UI
					await Windows.Graphics.Printing.PrintManager.ShowPrintUIAsync();

				}
				catch
				{
					// Printing cannot proceed at this time
					ContentDialog noPrintingDialog = new ContentDialog()
					{
						Title = "Printing error",
						Content = "\nSorry, printing can' t proceed at this time.",
						PrimaryButtonText = "OK"
					};
					await noPrintingDialog.ShowAsync();
				}
			}
			else
			{
				// Printing is not supported on this device
				ContentDialog noPrintingDialog = new ContentDialog()
				{
					Title = "Printing not supported",
					Content = "\nSorry, printing is not supported on this device.",
					PrimaryButtonText = "OK"
				};
				await noPrintingDialog.ShowAsync();
			}
		}

		private async Task<WebViewBrush> GetWebViewBrush(Windows.UI.Xaml.Controls.WebView webView)
		{
			// resize width to content
			var _OriginalWidth = webView.Width;
			var _WidthString = await webView.InvokeScriptAsync("eval",
				new[] { "document.body.scrollWidth.toString()" });
			int _ContentWidth;

			if (!int.TryParse(_WidthString, out _ContentWidth))
				throw new Exception(string.Format("failure/width:{0}", _WidthString));

			webView.Width = _ContentWidth;

			// resize height to content
			var _OriginalHeight = webView.Height;

			var _HeightString = await webView.InvokeScriptAsync("eval",
				new[] { "document.body.scrollHeight.toString()" });
			int _ContentHeight;

			if (!int.TryParse(_HeightString, out _ContentHeight))
				throw new Exception(string.Format("failure/height:{0}", _HeightString));

			webView.Height = _ContentHeight;

			// create brush
			var _OriginalVisibilty = webView.Visibility;

			webView.Visibility = Windows.UI.Xaml.Visibility.Visible;

			var _Brush = new WebViewBrush
			{
				Stretch = Stretch.Uniform,
				SourceName = webView.Name
			};


			_Brush.Redraw();


			// webView.Width = _OriginalWidth;
			//webView.Height = _OriginalHeight;
			webView.Visibility = _OriginalVisibilty;
			return _Brush;
		}
	}
}
