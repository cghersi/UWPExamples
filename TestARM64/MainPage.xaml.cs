using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DevExpress.Logify.UWP;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestARM64
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void GoodClick(object sender, RoutedEventArgs e)
		{
			Results.Text += " OK ";
		}

		private void BadClick(object sender, RoutedEventArgs e)
		{
			int a = 0;
			int b = 8;
			//int c = b / a; // should throw exception

			for (int i = 0; i < 1000; i++)
			{
				//i.e.
				//you have a local scope Exception object
				//send it to Logify like:
				LogifyAlert.Instance.Send(new System.Exception());
			}



			Results.Text += " OK also Bad ";
		}
	}
}
