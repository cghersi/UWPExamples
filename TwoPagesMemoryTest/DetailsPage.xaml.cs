using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TwoPagesMemoryTest
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class DetailsPage : Page
	{
		private User m_user;
		public DetailsPage()
		{
			this.InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);
			if (e.Parameter is User u)
			{
				m_user = u;
				RenderDetails();
			}
		}

		private void RenderDetails()
		{
			TbId.Text = m_user.Id.ToString();
			TbSurname.Text = m_user.Surname;
			TbName.Text = m_user.Name;
			TbNick.Text = m_user.Nick;
			TbEmail.Text = m_user.Email;
		}

		private void GoBack_OnClick(object sender, RoutedEventArgs e)
		{
			GoBack();
		}

		public static void GoBack()
		{
			Frame rootFrame = Window.Current?.Content as Frame;
			NavigationTransitionInfo navInfo = new SlideNavigationTransitionInfo
			{
				Effect = SlideNavigationTransitionEffect.FromBottom
			};
			try
			{
				rootFrame?.GoBack(navInfo);
			}
			catch (Exception e)
			{

			}
		}
	}
}
