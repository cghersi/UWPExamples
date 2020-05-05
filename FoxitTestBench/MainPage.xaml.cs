using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using foxit.common;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FoxitTestBench
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private static readonly string WITH_LINK_PDF_NAME = "ZZZ PDFWithLink.pdf";
		private static readonly string WITH_LINK_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + WITH_LINK_PDF_NAME;

		private static readonly string SIMPLE_PDF_NAME = "ZZZ SimpleDoc.pdf";
		private static readonly string SIMPLE_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + SIMPLE_PDF_NAME;

		private static readonly string LOCKED_PDF_NAME = "ZZZ LockedDoc.pdf";
		private static readonly string LOCKED_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + LOCKED_PDF_NAME;

		public MainPage()
		{
			this.InitializeComponent();

			ErrorCode ecCode = Library.Initialize("tbBasm2vcpttnM50+QcDo3lfx1dp83Vfc6pSXiumakBKvGxX6L8vHQ==", "ezJvj1/GvG539PvD3Z8X5uCMFXcWtIXHNyp0EUlUx+nSghquUN9LSQodcT71KeEfkxmkylLQy+gynfNUrM6+vZqPg8ShQLx8Xw5DGq/0TpOLhZ6fVWkAgqciqlhMIjIdaVBAuRphGRN4ghE2zUaLMeE9GvjeBMm2oIUlYxqTrGZ8RM3+oEQurX7O4D6lA2OzqHGByIJutoLAeI+yUL+bsMFx8bxh54Bgu3fzI0mpxMVA1az74U4t6kNcbz1slyg66YXDcyK2iuHu56tev8I5ce1D57bdQHdmX2t8MK4fiLDfAEIO7sVXU/13CMPNS01iHEtpfZzzkk19KfvZwpNylua9V5mij0F5UkUq/Bz9Nk5eX7z+TiHJQa+smggEl0GLkiDzm4lnQfTqOan5hrJsdtRxUqUE+pK+7qUwOHiLXUnyp1ShKNzuLV/63KLPMPLWCz3YGcibBkfFEfg7zFMs1w2eHoyLVd+UOeJXoQ5Af31rP1vF32mTR4EROA5MHZ5l2AUcxY1CLcfaFsNXL8ATDbv+vtALsvgj8xgoccX+ybKoILc1h5wZO/BE/eeOZS4uyedLETWM3qM1xFW0gTNKfpjAxJM3ypZyA/wAjhCEkH8z3a79mrWYxRIH0itcAtYlQCOITW9Ysdjlup46t+0Bf5OEV1mTvec/7ckMpq1Lo+cHDPEiepmtdvksHUbdU/Vbvs8D1U2eOLI4njvG6Ix0MPy4Up/fk+tkaGSLQOP32ONkfWMIih8+P7t4i7Ir0TVumaX37wjmV6TCdFVgxtvHCoKjXZJeBc/QwPInmT5yECJ3VMeD+gBgmUuJzluzFRJuFKDKUChFycmLKTrbJW3H1v8baurpal1ULbPwMXEMF5rqrJA2tCJ7ed2WMn3W+fqeJNfe64lKqbQFxoCl7C7BtOac6ehiwfvWIwAjom5lcuOndREF8KZRXeOqbawXmJRMViD58ajEupO3FYQaX71O2SCrThIjw+Vfd44+zcE8R0pWGRTyH4D1EqJcEr5xDT5zT8jbnzB/IroeDTZ/Udq2QTZNVuIlH+/5CIDNPyl7odeYo+93LJyxHI6pbdRHWdRQsi3X0R44qZEvYQfonF0aTR4koeyddF/3kyZhJSTykcBy9YJ7N/NA8h2B5WP+I7Plp98XRfM42V8sxhmhv7ME+SrPobVE16E1mpUWKekNEAfbzvZQcReJ3w==");
			if (ecCode != ErrorCode.e_ErrSuccess)
				throw new Exception("Invalid license");
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
			Test(new TestBench());
		}

		private async void CopyFile_Click(object sender, RoutedEventArgs e)
		{
			await CopyFileIfNeeded(WITH_LINK_PDF_NAME);
			await CopyFileIfNeeded(SIMPLE_PDF_NAME);
			await CopyFileIfNeeded(LOCKED_PDF_NAME);
		}

		private void Test(TestBench bench)
		{
			Results.Text += bench.LoadDocAndPrintPages(WITH_LINK_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(SIMPLE_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(LOCKED_PDF);
		}
	}
}
