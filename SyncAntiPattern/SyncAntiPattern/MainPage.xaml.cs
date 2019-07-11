using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SyncAntiPattern
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

		private void CreateFolder_OnClick(object sender, RoutedEventArgs e)
		{
			ResultTxt.Text = "";

			// let's create a folder: this works all the times!
			string dirName = "MyFolder" + DateTime.Now.Ticks;
			Task task = Task.Run(async () => { await dirName.CreateFolderAsync(); });
			task.Wait();

			ResultTxt.Text = "Success: " + dirName;
		}

		private void CreateFolderWithSync_OnClick(object sender, RoutedEventArgs e)
		{
			ResultWithSyncTxt.Text = "";

			// let's create a folder calling a utility method: this hangs indefinitely!
			string dirName = "MyFolderWithSync" + DateTime.Now.Ticks;
			dirName.CreateFolderAsync().SyncVoid();

			ResultWithSyncTxt.Text = "Success: " + dirName;
		}

		private void GetFolder_OnClick(object sender, RoutedEventArgs e)
		{
			ResultGetTxt.Text = "";

			// let's create a folder: 
			string dirName = "MyFolder" + DateTime.Now.Ticks;
			Task task = Task.Run(async () => { await dirName.CreateFolderAsync(); });
			task.Wait();

			// now read that folder: this works all the times! 
			string res = null;
			Task taskGet = Task.Run(async () => { res = await dirName.GetFolderAsync(); });
			taskGet.Wait();

			ResultGetTxt.Text = "Success: " + res;
		}

		private void GetFolderWithSync_OnClick(object sender, RoutedEventArgs e)
		{
			ResultWithSyncTxt.Text = "";

			// let's create a folder: 
			string dirName = "MyFolder" + DateTime.Now.Ticks;
			Task task = Task.Run(async () => { await dirName.CreateFolderAsync(); });
			task.Wait();

			// now read that folder calling a utility method: this hangs indefinitely!
			string res = dirName.GetFolderAsync().Sync();

			ResultWithSyncTxt.Text = "Success: " + res;
		}
	}
}
