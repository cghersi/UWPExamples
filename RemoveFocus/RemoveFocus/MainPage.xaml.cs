using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RemoveFocus
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		public static Button s_focusBtn;

		private static int s_counter = 0;

		public MainPage()
		{
			InitializeComponent();
			s_focusBtn = FocusBtn;

			Canvas editContainer = new Canvas()
			{
				Width = 200,
				Height = 300,
				Background = new SolidColorBrush(Colors.Aqua),
				Margin = new Thickness(300)
			};
			MainPanel.Children.Add(editContainer);

			RichEditBox editBox = new RichEditBox()
			{
				BorderThickness = new Thickness(3),
				BorderBrush = new SolidColorBrush(Colors.Red)
			};

			editBox.LosingFocus += EditBox_LostFocus;
			//editBox.LostFocus += EditBox_LostFocus;

			editContainer.Children.Add(editBox);

			UpdateLayout();

			MainPanel.PointerReleased += MainPanel_PointerReleased;
		}

		private void EditBox_LostFocus(object sender, RoutedEventArgs e)
		{
			Debug.WriteLine("LostFocus {0}", s_counter++);
		}

		private void MainPanel_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
		{
			//FocusUtils.RemoveFocus();
		}
	}

	public static class FocusUtils
	{
		/// <summary>
		/// Removed the focus from the current focused element by giving the focus to an invisible
		/// button.
		/// </summary>
		public static void RemoveFocus()
		{
			MainPage.s_focusBtn.Focus(FocusState.Programmatic);
		}
	}
}
