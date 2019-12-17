using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Syncfusion.DocIORenderer;
using Syncfusion.Pdf;

namespace UWPConverterToPDF
{
	public static class ConverterToPDF
	{
		public static async Task<string> ToPDF(this StorageFile fileToConvert)
		{

			PdfDocument pdfDoc;
			using (DocIORenderer render = new DocIORenderer())
			{
				using (Stream stream = await fileToConvert.OpenStreamForReadAsync())
				{
					pdfDoc = render.ConvertToPDF(stream);
				}
			}

			StorageFile outputFile = await
				ApplicationData.Current.LocalFolder.CreateFileAsync("test.pdf", CreationCollisionOption.GenerateUniqueName);

			using (Stream outputStream = await outputFile.OpenStreamForWriteAsync())
			{
				pdfDoc.Save(outputStream);
			}

			return outputFile.Path;
		}
	}
}
