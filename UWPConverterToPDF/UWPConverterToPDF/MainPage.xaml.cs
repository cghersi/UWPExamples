using System;
using Windows.Storage;
using Windows.UI.Xaml;

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
			StorageFile input;
			try
			{
				input = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///test.docx", UriKind.RelativeOrAbsolute));
			}
			catch (Exception ex)
			{
				Results.Text = "Cannot create file due to " + ex.Message;
				if (ex.InnerException != null)
					Results.Text += " || " + ex.InnerException.Message;
				return;
			}

			string outputPath;
			try
			{
				outputPath = await input.ToPDF();
			}
			catch (Exception ex)
			{
				Results.Text = "Cannot generate file due to " + ex.Message;
				if (ex.InnerException != null)
					Results.Text += " || " + ex.InnerException.Message;
				return;
			}


			Results.Text = "File Generated in: " + outputPath;
		}
	}
}
