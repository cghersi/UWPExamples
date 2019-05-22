using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExtendIOSConcepts
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly MyCustomView m_view1;

		public MainPage()
		{
			InitializeComponent();

			m_view1 = new MyCustomView();
			Workspace.Children.Add(m_view1.View);
			m_view1.SetMargin(100, 200);
			m_view1.SetSize(100, 45);
		}

		private void MovePanel_OnClick(object sender, RoutedEventArgs e)
		{
			m_view1.SetMargin(300, 500);
		}

		private void ExpandPanel_OnClick(object sender, RoutedEventArgs e)
		{
			m_view1.SetSize(600, 300);
		}

		private void ComplexMove_OnClick(object sender, RoutedEventArgs e)
		{
			m_view1.SetMargin(100, 200);
			m_view1.SetSize(100, 100);
			m_view1.View.InvalidateArrange();
			m_view1.SetMargin(100, 400);
			m_view1.SetSize(200, 500);
			m_view1.View.InvalidateMeasure();
			m_view1.SetMargin(300, 500);
			m_view1.SetSize(250, 300);
			m_view1.View.InvalidateMeasure();
			m_view1.View.InvalidateMeasure();
			m_view1.View.InvalidateMeasure();
			m_view1.SetSize(600, 300);
			m_view1.View.InvalidateMeasure();
			m_view1.SetSize(60, 350);
			m_view1.SetSize(65, 400);
		}
	}
}
