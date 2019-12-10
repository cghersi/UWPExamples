using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using pdftron;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CreatePDFWithPDFTron
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();

			PDFNet.Initialize("");
		}

		private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			StorageFile outputFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("test.pdf",
				CreationCollisionOption.GenerateUniqueName);
			PDFCreator creator = new PDFCreator(outputFile);
			if (await creator.Generate())
				Results.Text = "Generation completed in " + outputFile.Path;
			else
				Results.Text = "Generation Failed";
		}
	}
}
