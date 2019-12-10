using System.Diagnostics;
using Windows.Storage;

namespace Win32Printer
{
	class Program
	{
		static void Main(string[] args)
		{
			Process pdfProcess = new System.Diagnostics.Process {StartInfo = {FileName = @"FoxitReader.exe"}};
			object fileToPrint = ApplicationData.Current.LocalSettings.Values["FileToPrint"];
			pdfProcess.StartInfo.Arguments = string.Format("-t \"{0}\" \"{1}\"", fileToPrint, "Microsoft Print to PDF");
			pdfProcess.Start();
		}
	}
}
