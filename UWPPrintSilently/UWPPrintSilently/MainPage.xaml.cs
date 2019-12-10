using System;
using Windows.Storage;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPPrintSilently
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private async void Print_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				StorageFile pdfToPrint =
					await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///LiquidText.pdf",
						UriKind.RelativeOrAbsolute));
				StorageFile tempCopy = await pdfToPrint.CopyAsync(ApplicationData.Current.LocalFolder, "Temp.pdf",
					NameCollisionOption.ReplaceExisting);
				ApplicationData.Current.LocalSettings.Values["FileToPrint"] = tempCopy.Path;
				if (Windows.Foundation.Metadata.ApiInformation.IsApiContractPresent(
					"Windows.ApplicationModel.FullTrustAppContract", 1, 0))
				{
					await Windows.ApplicationModel.FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
					Results.Text = "Printer invoked successfully!";
				}
				else
				{
					Results.Text = "FullTrustAppContract not present";
				}
			}
			catch (Exception ex)
			{
				Results.Text = "Error: " + ex.Message + ";  " + ex.StackTrace;
			}
		}
	}
}
