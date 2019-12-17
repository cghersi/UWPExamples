using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPConverterToPDF
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

		private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			StorageFile input = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///test.docx", UriKind.RelativeOrAbsolute));
			string outputPath = await input.ToPDF();
			Results.Text = "File Generated in: " + outputPath;
		}
	}
}
