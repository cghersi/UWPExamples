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
		private static readonly string EASY_PDF_NAME = "ZZZ easyPdf.pdf";
		private static readonly string EASY_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + EASY_PDF_NAME;

		private static readonly string LONG_TIME_TO_SELECT_PDF_NAME = "ZZZ ValleyBrookPost.pdf";
		private static readonly string LONG_TIME_TO_SELECT_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + LONG_TIME_TO_SELECT_PDF_NAME;

		private static readonly string WITH_COMMENTS_PDF_NAME = "ZZZ PDF-Comments-Test.pdf";
		private static readonly string WITH_COMMENTS_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + WITH_COMMENTS_PDF_NAME;

		private static readonly string WITH_INTERNAL_LINK_PDF_NAME = "ZZZ pdfWithInternalLink.pdf";
		private static readonly string WITH_INTERNAL_LINK_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + WITH_INTERNAL_LINK_PDF_NAME;

		private static readonly string WITH_LINK_PDF_NAME = "ZZZ PDFWithLink.pdf";
		private static readonly string WITH_LINK_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + WITH_LINK_PDF_NAME;

		private static readonly string SIMPLE_PDF_NAME = "ZZZ SimpleDoc.pdf";
		private static readonly string SIMPLE_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + SIMPLE_PDF_NAME;

		private static readonly string LOCKED_PDF_NAME = "ZZZ LockedDoc.pdf";
		private static readonly string LOCKED_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + LOCKED_PDF_NAME;

		public MainPage()
		{
			this.InitializeComponent();

			ErrorCode ecCode = Library.Initialize("d3BGIHODAZqhtJqM49Ewv8LfTj5z29EusrHZe6o8ZJsuSYMv6GRA+A==", "ezJvj0kmtBh3VNviuesJQSIdTAq2tIXH52fZJYImDXHXheEP9ItFUNqlI4KToRW4etV0U2mXW8L7EQ5/pYwISdx3XvqPwfRHT8Xstds4ndTN1BCUX0jgfQiTYWn4p15GPc+StIbZSGwfCjp83KeIB3NGTizYSkI8E4K1Z+MoH2xKR5DKZowxHHSAVQRT5to8WZJFP0d1cgpHxGb42vswGODC1EgEiravWEXIpD4Z4tcK175sxMtiJ1fLDJZ2znv4/PjC6mdwiI0DUIsFvLSUWnF4uj0h5WfMDsEoAA2pykrYMkaryjmjLfEBblyYS2pj8IdHKXN5b0B4fOV+kXx4Vtw08IyMX6SgFeD9SLDaHYDZjcwpf3feaBdSOm7s3iUWCnbsO3uAYkgxlkFBu3tcQpLbZUvRB8O+9oVuJmQ+FM0InnchfFlpJ/lHUX6y2Oz/00HYEFY4o9IL5pWVqrvdrmvAwC6w9y+mYXRjBh3ii9j1mNDyH2H5eXctbNrkEN52d5DfOV28kueTyjVP100hjv/LOvrzt5O+ApD2NIf7xFr8xxQPYfcQKyt2BnkIEFlKSTl6FInDR3go/48ZPIGMTYza5BHlNLE/4h/DK5dxleQQnJ9Wwl+f2tbK/gSEstmCfvMJf4QzLI2UxIc/ivS5HSzGvcu4NZZnWkuLVxSGJFX/7SgfsFAwpXx87w+yiwoX7TSevU3E1VTqkDTpF2Jr/jX8W5nQMvlLp5DZWkPBrB3GFjIwIaRgR0hpT5Qfihf/mpIr+3j3tAkdemGXOZ21zbL9O8Qm1G4nDxou+kzDpWTvv3mDltPOwBdQjgDOYb4ZfwD845SFcASqpWiF2Jyr+Lr4G7Zf8hwZuVsAV+XNRSO0dvPdsTAKRxp2EZ/tUUKwyOMQh5EpfP6lbJl8C+LY7/PN1rX5f4S2Oqi0d+jYBWywebseLCh19aSgat3EGbsNrwmuQhrtLj27FaMxoIUqn+gfiNFYPd3/h8oJ47Cp4Enbm9IkR9008I24+gjbhn+88j2plgCno4xmbb4imvbFiuJkU9c5BGfSvcm9wIyNXo58bYFvbqSGGK2JS6ZkQBCXSuhTdpSsQyhDfbrXw32W+4PkPCYG9XKPx9dqbgL58Mxw60AfUP5pEGYoMOoIUi5MQh7jmtWg2d4W+a3AzHfeZSHm0oLWfZtZ1AWExosPEfWoyhR4JvVedC1rcpm4za9KgstDeA==");
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
			await CopyFileIfNeeded(EASY_PDF_NAME);
			await CopyFileIfNeeded(LONG_TIME_TO_SELECT_PDF_NAME);
			await CopyFileIfNeeded(WITH_COMMENTS_PDF_NAME);
			await CopyFileIfNeeded(WITH_LINK_PDF_NAME);
			await CopyFileIfNeeded(WITH_INTERNAL_LINK_PDF_NAME);
			await CopyFileIfNeeded(SIMPLE_PDF_NAME);
			await CopyFileIfNeeded(LOCKED_PDF_NAME);
		}

		private void Test(TestBench bench)
		{
			Results.Text += bench.LoadDocAndPrintPages(EASY_PDF);
			Results.Text += bench.LoadDocAndPrintPages(LONG_TIME_TO_SELECT_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(WITH_COMMENTS_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(WITH_INTERNAL_LINK_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(WITH_LINK_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(SIMPLE_PDF);
			//Results.Text += bench.LoadDocAndPrintPages(LOCKED_PDF);
		}
	}
}
