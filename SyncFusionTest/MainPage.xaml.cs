using System;
using System.Collections.Generic;
using System.IO;
using Windows.UI.Xaml.Controls;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.Drawing;
using FormatType = Syncfusion.DocIO.FormatType;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Image = Syncfusion.Drawing.Image;
using PdfDocument = Syncfusion.Pdf.PdfDocument;
using PdfPage = Syncfusion.Pdf.PdfPage;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SyncFusionTest
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}


		private static async Task ConvertDocToPdf(StorageFile fileToConvert)
		{
			byte[] fileBytes = await ToByteArray(fileToConvert);
			Stream fileStream = new MemoryStream(fileBytes);

			//Loads an existing Word document
			WordDocument wordDocument = new WordDocument(fileStream, FormatType.Automatic)
			{
				//Initializes the ChartToImageConverter for converting charts during Word to pdf conversion
				//ChartToImageConverter = new ChartToImageConverter()
			};

			//Creates an instance of DocToPDFConverter - responsible for Word to PDF conversion
			DocIORenderer docIoRenderer = new DocIORenderer();
			//Sets true to embed TrueType fonts
			docIoRenderer.Settings.EmbedFonts = true;

			//Converts Word document into PDF document
			PdfDocument pdfDocument = docIoRenderer.ConvertToPDF(wordDocument);
			pdfDocument.FileStructure = new PdfFileStructure()
			{
				Version = PdfVersion.Version1_7,
			};

			//Save the document into stream.
			MemoryStream outputStream = new MemoryStream();
			pdfDocument.Save(outputStream);

			//Closes the instance of PDF document object
			pdfDocument.Close();

			Save(outputStream, fileToConvert.DisplayName + ".pdf");
		}

		private static async Task ConvertImageToPdf(StorageFile fileToConvert)
		{
			byte[] fileBytes = await ToByteArray(fileToConvert);

			//Create a new PDF document
			PdfDocument pdfDocument = new PdfDocument();

			//Add a page to the document
			PdfPage page = pdfDocument.Pages.Add();

			//Get page size to draw image which fits the page
			SizeF pageSize = page.GetClientSize();

			//Create PDF graphics for the page
			PdfGraphics graphics = page.Graphics;
			//graphics.ColorSpace = PdfColorSpace.RGB;

			//Load the image from the disk
			Stream imageFileStream = new MemoryStream(fileBytes);
			PdfBitmap image = new PdfBitmap(imageFileStream);
			float imageWidth;
			float imageHeight;
			float ratio = image.Width / (float)image.Height;
			if (image.Width > image.Height)
			{
				// Landscape
				imageWidth = Math.Min((float)image.Width, pageSize.Width);
				imageHeight = imageWidth / ratio;
			}
			else
			{
				// Portrait
				imageHeight = Math.Min((float)image.Height, pageSize.Height);
				imageWidth = imageHeight * ratio;
			}

			//Draw the image
			graphics.DrawImage(image, new RectangleF(0, 0, imageWidth, imageHeight));

			//Save the document into stream.
			MemoryStream outputStream = new MemoryStream();
			pdfDocument.Save(outputStream);


			//Closes the instance of PDF document object
			pdfDocument.Close();
			Save(outputStream, fileToConvert.DisplayName + ".pdf");

		}

		public static async Task<byte[]> ToByteArray(StorageFile file)
		{
			if (file != null)
			{
				var randomAccessStream = await file.OpenReadAsync();
				Stream filestream = randomAccessStream.AsStreamForRead();
				using (var memoryStream = new MemoryStream())
				{
					filestream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}

			return new byte[0];
		}

		//Saves the PDF document
		public static async void Save(MemoryStream streams, string filename)
		{
			streams.Position = 0;
			StorageFile stFile;
			if (!(Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")))
			{
				FileSavePicker savePicker = new FileSavePicker();
				savePicker.DefaultFileExtension = ".pdf";
				savePicker.SuggestedFileName = filename;
				savePicker.FileTypeChoices.Add("Word Documents", new List<string>() { ".pdf" });
				stFile = await savePicker.PickSaveFileAsync();
			}
			else
			{
				StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
				stFile = await local.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
			}
			if (stFile != null)
			{
				using (IRandomAccessStream zipStream = await stFile.OpenAsync(FileAccessMode.ReadWrite))
				{
					//Write compressed data from memory to file
					using (Stream outstream = zipStream.AsStreamForWrite())
					{
						byte[] buffer = streams.ToArray();
						outstream.Write(buffer, 0, buffer.Length);
						outstream.Flush();
					}
				}
			}
			//Launch the saved Word file
			await Windows.System.Launcher.LaunchFileAsync(stFile);
		}

		private async void ConvertDocX_OnClick(object sender, RoutedEventArgs e)
		{
			string docxFilename = "ms-appx:///Assets/helloworld.docx";

			StorageFile docxFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(docxFilename, UriKind.RelativeOrAbsolute));

			await ConvertDocToPdf(docxFile);

		}

		private async void ConvertImage_OnClick(object sender, RoutedEventArgs e)
		{
			string imageName = "ms-appx:///Assets/labrador.jpg";

			StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri(imageName, UriKind.RelativeOrAbsolute));

			await ConvertImageToPdf(imageFile);
		}

	}
}