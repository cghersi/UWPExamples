using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ExtendIOSConcepts
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			InitializeComponent();

			MyCustomView view1 = new MyCustomView();
			Workspace.Children.Add(view1.View);
			view1.SetMargin(100, 200);
		}
	}
}
