using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

// ReSharper disable once CheckNamespace
namespace LiquidTextUWP.PDFExport
{
	public static class Utils
	{
		public static bool IsNullOrEmpty<T>(this ICollection<T> list)
		{
			return (list == null) || (list.Count <= 0);
		}

		public static async Task<StorageFile> GetFile(this string filename, StorageFolder inDir = null)
		{
			try
			{
				if (filename.StartsWith("ms-appx"))
					return await StorageFile.GetFileFromApplicationUriAsync(new Uri(filename, UriKind.RelativeOrAbsolute));
				else if ((inDir != null) && filename.StartsWith(inDir.Path))
					return await inDir.GetFileAsync(filename.Replace(inDir.Path + "\\", ""));
				else if ((inDir != null) && !filename.Contains("\\"))
					return await inDir.GetFileAsync(filename);
				else
					return await StorageFile.GetFileFromPathAsync(filename);
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Cannot find file '{0}'", filename);
				return null;
			}
			catch (Exception e)
			{
				Console.WriteLine("Cannot read file {0} due to {1}", filename, e.Message);
				return null;
			}
		}
	}

	public class Logger
	{
		public Logger(Type type)
		{
		}

		public Logger(string name)
		{
		}

		// ReSharper disable once UnusedMember.Global
		public void DebugFormat(string format, params object[] args)
		{
			Debug.WriteLine("DEBUG: " + DateTime.Now + " - " + format, args);
		}

		public void InfoFormat(string format, params object[] args)
		{
			Debug.WriteLine("INFO: " + DateTime.Now + " - " + format, args);
		}

		public void WarnFormat(string format, params object[] args)
		{
			Debug.WriteLine("WARN: " + DateTime.Now + " - " + format, args);
		}

		public void ErrorFormat(string format, params object[] args)
		{
			Debug.WriteLine("ERROR: " + DateTime.Now + " - " + format, args);
		}
	}
}
