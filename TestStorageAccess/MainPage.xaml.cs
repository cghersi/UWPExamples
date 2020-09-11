using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TestStorageAccess
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		private const string SAVE_TOKEN_FILE_NAME = "SavedToken.txt";

		public MainPage()
		{
			InitializeComponent();
		}

		private async void PickFile_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				// Open the file picker and let the user select a file:
				FileOpenPicker openPicker = new FileOpenPicker();
				openPicker.FileTypeFilter.Add(".txt");

				StorageFile file = await openPicker.PickSingleFileAsync();

				// Get the access token of this file:
				string token = StorageApplicationPermissions.FutureAccessList.Add(file);

				// Save this token on a file, so that when we re-open the app we can retrieve it:
				StorageFile saveTokenFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(SAVE_TOKEN_FILE_NAME,
					CreationCollisionOption.OpenIfExists);

				// Clean the file if it was already present:
				await FileIO.WriteTextAsync(saveTokenFile, string.Empty);

				// Write the token onto the file:
				await FileIO.WriteTextAsync(saveTokenFile, token);

				Results.Text = "Token saved.";
			}
			catch (Exception ex)
			{
				Results.Text = "Cannot save token due to " + ex.Message;
			}
		}

		private async void WriteFile_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				// Get the file where we saved the token:
				StorageFile saveTokenFile = await ApplicationData.Current.LocalFolder.GetFileAsync("SavedToken.txt");

				// Retrieve the access token from the saved file:
				string token = await FileIO.ReadTextAsync(saveTokenFile);

				// Get the file using the token:
				StorageFile file = await StorageApplicationPermissions.FutureAccessList.GetFileAsync(token);

				// Write onto the file:
				//await FileIO.WriteTextAsync(file, string.Empty);
				byte[] content = { 0x41, 0x52 };
				IBuffer buf = content.AsBuffer();

				using (IRandomAccessStream fs = await file.OpenAsync(FileAccessMode.ReadWrite))
				{
					await fs.WriteAsync(buf);
				}

				ResultsWrite.Text = "File written.";
			}
			catch (Exception ex)
			{
				ResultsWrite.Text = "Cannot write on file due to " + ex.Message;
			}
		}
	}
}
