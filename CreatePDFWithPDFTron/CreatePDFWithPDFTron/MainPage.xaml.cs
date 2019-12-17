using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LiquidTextUWP.PDFExport;
using pdftron;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CreatePDFWithPDFTron
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private static readonly List<string> s_fileNames = new List<string>
		{
			"ms-appx:///greyMite.pdf",
			"ms-appx:///SnapBrickPatent.pdf"
		};

		private static readonly List<string> s_rotatedFiles = new List<string>
		{
			"ms-appx:///rotatedPDF.pdf"
		};

		public MainPage()
		{
			InitializeComponent();

			PDFNet.Initialize("");
		}

		private void Create_OnClick(object sender, RoutedEventArgs e)
		{
			Action(null, creator => creator.Generate(), "Generation");
		}

		private void Merge_OnClick(object sender, RoutedEventArgs e)
		{
			Action(null, creator => creator.ImportFiles(s_fileNames), "Import");
		}

		private void Rotate_OnClick(object sender, RoutedEventArgs e)
		{
			Action(creator => creator.ImportFiles(s_rotatedFiles), creator => Task.FromResult(creator.RotatePage(2, 90)) ,"Rotate");
		}

		private async void Action(Func<PDFCreator, Task<bool>> population, Func<PDFCreator, Task<bool>> action, string actionName)
		{
			StorageFile outputFile = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("test.pdf",
				CreationCollisionOption.GenerateUniqueName);
			using (PDFCreator creator = new PDFCreator(outputFile))
			{
				if (population != null)
				{
					if (!await population(creator))
					{
						Results.Text = "Population Failed";
						return;
					}
				}

				if (await action(creator))
				{
					if (await creator.SaveToFile())
						Results.Text = actionName + " completed in " + outputFile.Path;
					else
						Results.Text = actionName + " completed but save failed in " + outputFile.Path;
				}
				else
					Results.Text = actionName + " Failed";
			}
		}
	}
}
