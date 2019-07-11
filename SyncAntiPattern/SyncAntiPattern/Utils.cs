using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace SyncAntiPattern
{
	public static class Utils
	{
		private static readonly StorageFolder FOLDER = ApplicationData.Current.LocalFolder;

		public static async Task<string> CreateFolderAsync(this string dirName)
		{
			try
			{
				StorageFolder f = await FOLDER.CreateFolderAsync(dirName);
				return f?.Path;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static async Task<string> GetFolderAsync(this string dirName)
		{
			try
			{
				StorageFolder f = await FOLDER.GetFolderAsync(dirName);
				return f?.Path;
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}
	}
}
