using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace SyncAntiPattern
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		public MainPage()
		{
			InitializeComponent();
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

			// let's create a folder calling a utility method:
			// note the trick is to pass a Func, rather than the Task, because in this way the 
			// task is invoked in the utility method, which doesn't results into deadlock!
			string dirName = "MyFolderWithSync" + DateTime.Now.Ticks;
			SyncUtils.ExecAndWait(() => dirName.CreateFolderAsync());
			//dirName.CreateFolderAsync().SyncVoid();  this instead would hangs indefinitely!

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
			ResultWithSyncGetTxt.Text = "";

			// let's create a folder: 
			string dirName = "MyFolder" + DateTime.Now.Ticks;
			Task task = Task.Run(async () => { await dirName.CreateFolderAsync(); });
			task.Wait();

			// now read that folder calling a utility method: 
			// note the trick is to pass a Func, rather than the Task, because in this way the 
			// task is invoked in the utility method, which doesn't results into deadlock!
			string res = SyncUtils.Sync(() => dirName.GetFolderAsync());
			//string res = dirName.GetFolderAsync().Sync();  this instead would hangs indefinitely!

			ResultWithSyncGetTxt.Text = "Success: " + res;
		}
	}
}
