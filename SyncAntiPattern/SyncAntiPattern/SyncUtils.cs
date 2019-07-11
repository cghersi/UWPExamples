using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SyncAntiPattern
{
	public static class SyncUtils
	{
		public static void SyncVoid(this Task task)
		{
			if (task == null)
				return;

			Task t = Task.Run(async () =>
			{
				try
				{
					await task;
				}
				catch (Exception e)
				{
					Debug.WriteLine("Exception while running task {0} due to {1}", task.Id, e.Message);
				}
			});
			try
			{
				t.Wait();
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception while waiting for the completion of task {0} due to {1}", task.Id, e.Message);
			}
		}

		public static T Sync<T>(this Task<T> task)
		{
			if (task == null)
				return default;

			T res = default;
			Task t = Task.Run(async () =>
			{
				try
				{
					res = await task;
				}
				catch (Exception e)
				{
					Debug.WriteLine("Exception while running task {0} due to {1}", task.Id, e.Message);
					res = default;
				}
			});
			try
			{
				t.Wait();
			}
			catch (Exception e)
			{
				Debug.WriteLine("Exception while waiting for the completion of task {0} due to {1}", task.Id, e.Message);
				return default;
			}

			return res;
		}
	}
}
