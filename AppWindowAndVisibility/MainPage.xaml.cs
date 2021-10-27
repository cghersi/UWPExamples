using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AppWindowAndVisibility
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private AppWindow m_appWindow;

		private Canvas m_wrapperCanvas;
		private Grid m_searchGrid;
		private TextBox m_searchBox;
		private const double SEARCH_BOX_HEIGHT = 35;

		public MainPage()
		{
			this.InitializeComponent();
			Init();
		}

		private void Init()
		{
			// wrapper canvas
			m_wrapperCanvas = new Canvas()
			{
				Width = 800,
				Height = 600,
				Background = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200))
			};

			// Change visibility button
			Button changeVisibilityBt = new Button()
			{
				Content = "Show/hide search bar",
			};
			changeVisibilityBt.Click += (sender, args) =>
			{
				SwitchSearchBarVisibility();
			};
			m_wrapperCanvas.Children.Add(changeVisibilityBt);


			// The grid
			m_searchGrid = new Grid
			{
				Visibility = Visibility.Collapsed,
				Background = new SolidColorBrush(Color.FromArgb(255, 128, 128, 128)),
				Padding = new Thickness(10),
				Margin = new Thickness(0, 60, 0, 0)
			};
			m_searchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
			m_searchGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });



			// Search box
			m_searchBox = new TextBox
			{
				PlaceholderText = "Keyword Search All Documents",
				Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
				CornerRadius = new CornerRadius(10),
				Height = SEARCH_BOX_HEIGHT
			};
			Grid.SetRow(m_searchBox, 0);
			Grid.SetColumn(m_searchBox, 0);
			m_searchGrid.Children.Add(m_searchBox);
			m_wrapperCanvas.Children.Add(m_searchGrid);
		}


		public async Task<bool> ShowAsync()
		{
			if (m_appWindow == null)
			{

				m_appWindow = await AppWindow.TryCreateAsync();

				if (m_appWindow != null)
				{
					m_appWindow.Title = "MyTitle";
					try
					{
						ElementCompositionPreview.SetAppWindowContent(m_appWindow, m_wrapperCanvas);
					}
					catch (Exception e)
					{
						// HERE WE GO: IF SEARCH BAR VISIBILITY HAS BEEN CHANGED AN EXCEPTION IS RAISED HERE!
					}
					finally
					{
						m_appWindow.Closed += AppWindow_OnClosed;
					}
				}
			}


			if (m_appWindow != null)
				await m_appWindow.TryShowAsync();

			return true;
		}

		public void SwitchSearchBarVisibility()
		{
			// Change search bar visibility
			m_searchGrid.Visibility = m_searchGrid.Visibility != Visibility.Visible ? Visibility.Visible : Visibility.Collapsed;
			m_searchGrid.UpdateLayout();
			m_wrapperCanvas.UpdateLayout();
		}

		private void AppWindow_OnClosed(AppWindow sender, AppWindowClosedEventArgs args)
		{
			m_appWindow.Closed -= AppWindow_OnClosed;
			m_appWindow = null;
		}

		private async void OpenWindowBt_OnClick(object sender, RoutedEventArgs e)
		{
			await ShowAsync();
		}
	}
}