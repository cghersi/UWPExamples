using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Timer = System.Threading.Timer;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwoPagesMemoryTest
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private List<User> m_items;
		private DispatcherTimer m_memoryTimer;
		public MainPage()
		{
			this.InitializeComponent();
			InitListView();
			StartMemoryTimer();
		}


		private void InitListView()
		{
			m_items = new List<User>();
			for (int i = 0; i < 10; i++)
			{
				User u = new User()
				{
					Id = i,
					Surname = "Surname_" + i,
					Name = "Name_" + i,
					Email = "Email" + i + "@liquidtext-net",
					Nick = "Nickname_" + i
				};
				m_items.Add(u);
			}
			MainListView.ItemsSource = m_items;
			MainListView.ItemClick += MainListView_OnItemClick;
		}

		private void MainListView_OnItemClick(object sender, ItemClickEventArgs e)
		{
			if (e.ClickedItem is User u)
			{
				ShowDetails(u);
			}
		}

		private void ShowDetails(object parameter = null)
		{
			Frame rootFrame = Window.Current?.Content as Frame;
			NavigationTransitionInfo navInfo = new SlideNavigationTransitionInfo
			{
				Effect = SlideNavigationTransitionEffect.FromBottom
			};
			rootFrame?.Navigate(typeof(DetailsPage), parameter, navInfo);
		}

		private void Loop_OnClick(object sender, RoutedEventArgs e)
		{
			Task.Run(ShowDetailsLoop);
		}

		private void ShowDetailsLoop()
		{
			foreach (User u in m_items)
			{
				Dispatcher?.RunAsync(CoreDispatcherPriority.High, () => ShowDetails(u));
				Thread.Sleep(TimeSpan.FromSeconds(1));
				Dispatcher?.RunAsync(CoreDispatcherPriority.High, DetailsPage.GoBack);
			}
		}

		private void StartMemoryTimer()
		{
			// Create a timer with a two second interval.
			m_memoryTimer = new DispatcherTimer();
			m_memoryTimer.Tick += MemoryTimer_OnTick;
			m_memoryTimer.Interval = new TimeSpan(0, 0, 1);
			m_memoryTimer.Start();
		}

		private void MemoryTimer_OnTick(object sender, object e)
		{
			string memoryData = string.Empty;
			memoryData += $"commit:\t\t{MemoryManager.AppMemoryUsage}\n";
			ProcessMemoryReport r1 = MemoryManager.GetProcessMemoryReport();
			memoryData += $"private:\t{r1.PrivateWorkingSetUsage}\n";
			memoryData += $"total:\t\t{r1.TotalWorkingSetUsage}\n";

			ProcessDiagnosticInfo info = ProcessDiagnosticInfo.GetForCurrentProcess();
			ProcessMemoryUsageReport r2 = info.MemoryUsage.GetReport();
			memoryData += $"peak:\t\t{r2.PeakWorkingSetSizeInBytes}\n";

			TbMemory.Text = memoryData;
		}
	}

	public struct User
	{
		public int Id { get; set; }
		public string Surname { get; set; }
		public string Name { get; set; }
		public string Nick { get; set; }
		public string Email { get; set; }

		public override string ToString()
		{
			return $"{Surname} {Name}";
		}
	}

}
