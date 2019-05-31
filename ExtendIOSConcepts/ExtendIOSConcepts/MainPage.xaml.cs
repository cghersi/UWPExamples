using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExtendIOSConcepts
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private readonly MyCustomView m_view1;
		private readonly PanningView m_panningView;

		public MainPage()
		{
			InitializeComponent();

			m_panningView = new PanningView();
			Workspace.Children.Add(m_panningView.View);
			m_panningView.SetMargin(1300, 1300);
			m_panningView.SetSize(1700, 1600);

			m_view1 = new MyCustomView();
			Workspace.Children.Add(m_view1.View);
			//m_panningView.ParentForChildren.Children.Add(m_view1.View);
			m_view1.SetMargin(100, 200);
			m_view1.SetSize(100, 45);

			//for (int i = 0; i < 10; i++)
			//{
			//	for (int j = 0; j < 10; j++)
			//	{
			//		MyCustomView view = new MyCustomView();
			//		m_panningView.ParentForChildren.Children.Add(view.View);
			//		view.SetMargin(100 * i, 100 * j);
			//		view.SetSize(100, 45);
			//	}
			//}
		}

		private void MovePanel_OnClick(object sender, RoutedEventArgs e)
		{
			m_view1.SetMargin(300, 500);
			m_view1.RequestNewLayout();
			m_view1.RequestNewLayout();
		}

		private void ExpandPanel_OnClick(object sender, RoutedEventArgs e)
		{
			m_view1.SetSize(600, 300);
			m_view1.RequestNewLayout();
			m_view1.RequestNewLayout();
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
