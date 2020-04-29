using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExportPDFWithSyncFusion
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private static readonly string PDF1_NAME = "180degreeRotatedPages-no90s.pdf";
		private static readonly string PDF1 = ApplicationData.Current.LocalFolder.Path + "\\" + PDF1_NAME;

		private static readonly string PDF2_NAME = "180degreeRotatedPages.pdf";
		private static readonly string PDF2 = ApplicationData.Current.LocalFolder.Path + "\\" + PDF2_NAME;

		public MainPage()
		{
			this.InitializeComponent();
		}

		private static async Task CopyFileIfNeeded(string fileName)
		{
			if (File.Exists(ApplicationData.Current.LocalFolder.Path + "\\" + fileName))
				return;
			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/" + fileName, UriKind.RelativeOrAbsolute));
			if (file != null)
				await file.CopyAsync(ApplicationData.Current.LocalFolder);
		}

		private void Generate_Click(object sender, RoutedEventArgs e)
		{
			Results.Text = "";
			Test(PDF1);
			Test(PDF2);
		}

		private async void CopyFile_Click(object sender, RoutedEventArgs e)
		{
			await CopyFileIfNeeded(PDF1_NAME);
			await CopyFileIfNeeded(PDF2_NAME);
		}

		private void Test(string pdfPath)
		{
			LTPDFDrawingContext context = LTPDFDrawingContext.createFromPDFPaths(new List<string> { pdfPath });

			string savePath = ApplicationData.Current.LocalFolder.Path + "\\" + Guid.NewGuid() + ".pdf";
			context.saveAs(savePath);

			Results.Text += "Saved file " + pdfPath.Substring(pdfPath.Length - 25) + " into      " + savePath + Environment.NewLine + Environment.NewLine;
		}
	}
}
