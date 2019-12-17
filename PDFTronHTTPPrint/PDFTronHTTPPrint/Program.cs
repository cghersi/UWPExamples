using System;
using pdftron;
using pdftron.Common;
using pdftron.PDF;
using pdftron.SDF;

namespace PDFTronHTTPPrint
{
	public class Program
	{
		private static pdftron.PDFNetLoader pdfNetLoader = pdftron.PDFNetLoader.Instance();

		public static void Main(string[] args)
		{
			string output_path = "../../../../html2pdf_example";
			string host = "https://liquidtext.net"; 
			//string host = "https://mail.google.com/mail/u/0/#inbox";
			//string host = "http://www.gutenberg.org/";
			string page0 = ""; //"wiki/Main_Page";
			string page1 = "catalog/";
			string page2 = "browse/recent/last1";
			string page3 = "wiki/Gutenberg:The_Sheet_Music_Project";

			// The first step in every application using PDFNet is to initialize the 
			// library and set the path to common PDF resources. The library is usually 
			// initialized only once, but calling Initialize() multiple times is also fine.
			PDFNet.Initialize();

			// For HTML2PDF we need to locate the html2pdf module. If placed with the 
			// PDFNet library, or in the current working directory, it will be loaded
			// automatically. Otherwise, it must be set manually using HTML2PDF.SetModulePath.
			//HTML2PDF.SetModulePath("../../../../../Lib");

			try
			{
				// open the existing PDF, and initialize the security handler
				using (PDFDoc doc = new PDFDoc())
				{
					doc.InitSecurityHandler();

					// create the HTML2PDF converter object and modify the output of the PDF pages
					HTML2PDF converter = new HTML2PDF();
					converter.SetImageQuality(2000);
					converter.SetPaperSize(PrinterMode.PaperSize.e_a4);
					//converter.SetCookieJar();

					// insert the web page to convert
					HTML2PDF.WebPageSettings settings = new HTML2PDF.WebPageSettings();
					settings.SetAllowJavaScript(true);
					settings.SetAllowPlugins(true);
					settings.SetJavaScriptDelay(3000);
					settings.SetLoadImages(true);

					converter.InsertFromURL(host + page0, settings);

					// convert the web page, appending generated PDF pages to doc
					if (converter.Convert(doc))
						doc.Save(output_path + "_02.pdf", SDFDoc.SaveOptions.e_linearized);
					else
						Console.WriteLine("Conversion failed. HTTP Code: {0}\n{1}", converter.GetHTTPErrorCode(), converter.GetLog());
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
			}

			try
			{
				using (PDFDoc doc = new PDFDoc())
				{
					// now convert a web page, sending generated PDF pages to doc
					if (HTML2PDF.Convert(doc, host + page0))
						doc.Save(output_path + "_01.pdf", SDFDoc.SaveOptions.e_linearized);
					else
						Console.WriteLine("Conversion failed.");
				}
			}
			catch (PDFNetException e)
			{
				Console.WriteLine(e.Message);
			}
		}
	}
}
