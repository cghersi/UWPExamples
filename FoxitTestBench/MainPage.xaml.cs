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
		private static readonly string SIMPLE_PDF_NAME = "ZZZ SimpleDoc.pdf";
		private static readonly string SIMPLE_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + SIMPLE_PDF_NAME;

		private static readonly string LOCKED_PDF_NAME = "ZZZ LockedDoc.pdf";
		private static readonly string LOCKED_PDF = ApplicationData.Current.LocalFolder.Path + "\\" + LOCKED_PDF_NAME;

		public MainPage()
		{
			this.InitializeComponent();

			ErrorCode error_code = Library.Initialize("okczsb6vTBZN2TNihqyqpwTvmqkgmJVTy+PRp9kcjCva3WBJ4gWcig==", "ezJvjl3GrGpz9JsXIVWofHd+ZehluFa6bsRqkkMKLapzBohxF3ZbZ2nKMbbZIA5S6NmZUvmZ/XALkjpytdB+RYTL4sVy0K1fLq/3/748yvi6ZNmXPXDEaWKcl0iTL9gI2Zj4amUFJHn744x42E4tett8Wi4uX5DsTRUjTRukQxpVIZOklPu/7E1fweEjCp7I83i24Y6z6cwFQMjnpElh+ARHvj93DYhEFolpW/TnX7ZaPsn0Xbgyo4kVLJJQXxEp7Qw8gMtVWudUc/ksNyFa9AGoscpFt/Ms2r1uId+9QsBj9dMaH4kckP2EF4jDC/+6nZFRy64TarhDfOAzcb3Q7ulgI0HiYQ6w4ITxZQM/yLnviAvtHkmb3vYJ6VfzSTz/pPT5vePdxW3O4KseqqwECVlWlN0EY2+I/kAlerUiNlvvykmQ7tFzzZccFg3tK6N1piFziBne9dDYoJUyy43N06oiQUkJtidZnkCWlgrdjVApGFhwNfJuqZP8mv4CyNrZ+ZIokok3PGeVTsaegyWNDshL0DmB0CorP2iTW/Li2vx26k0UKM8Om8CH/VhyusrHDhWqM69Dc1RLa1s92QdWXPeIDd92AY+UbPS3QhjWzMfCCgYa+kFZRPjhTeYX/6ZKdFjItYa+6DFA7B5+2fW4PZA1mguVR9MRhjZghZ3SvhF1u62TKV4mQFy6B13dSnvamjRC/gw1VXY/nXIEfgntt9U7aL4AxXo8ekmhg0F4rI0cek6s7n595FR2/+cBANIhqshzSRHdEhADERnQCpA1SnId+GieFLGWRex+/tKySC4m2aMqV/m7lB/oOWgSZXqa1o104/zbNxZjTWco4xjztArJt9aQmeQ8BABCBfqGOk/JVyXq8i++G2wzUZJf+L7Y9AGCs/H5QWeTxTyJ8izDZn21WSRiG6Meyku/WS/caQ1EilYkrpB+xAIeyDTz1gUqYPdI+QAjXZRXuDtrcaG4M2VrbrT9IYUf0k5o4vqvyENmXWTCewFpCM0aWA+y4wAIWhyeG9Jjrr8jAxfgBj8yPLO4W9Myt8PAQqAOzMVjkLTLyYk6UTjncmPVifbxrinTk2slRf8ELqtpeQLx8HDROO9P5ueTQlhTlBpEUAOr4ujHj8Xjb5H5Ea7YKtw9eHQNademwVOnHzVx8sOWWVHn6XpvqjXYMoCpsytcQbNXFILO8Nym5W1YmBX3QS3azaqkkWZMM8TNgrNHe7TQqEKgGC6JxVQsPSnfALjzIeSRi07gaQbkEAvxzI00PV8PxzSVsKsuKeNQMA==");
			if (error_code != ErrorCode.e_ErrSuccess)
				return;
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
			await CopyFileIfNeeded(SIMPLE_PDF_NAME);
			await CopyFileIfNeeded(LOCKED_PDF_NAME);
		}

		private void Test(TestBench bench)
		{
			//Results.Text += bench.LoadDocAndPrintPages(SIMPLE_PDF);
			Results.Text += bench.LoadDocAndPrintPages(LOCKED_PDF);
		}
	}
}
