using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace EventsOnOverlyingElements
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private const int WIDTH = 100;
		private const int HEIGHT = 50;


		public MainPage()
		{
			this.InitializeComponent();

			ParentContainer.Width = WIDTH;
			ParentContainer.Height = HEIGHT;

			// Add the elements hosted by the ParentContainer:

			// 1. a container for text:
			RelativePanel textView = new RelativePanel()
			{
				Margin = new Thickness(10, 10, 5, 5),
				Width = WIDTH - 10,
				Height = HEIGHT - 10,
				Background = new SolidColorBrush(Colors.White)
			};
			TextBlock label = new TextBlock()
			{
				Text = "Content of the label"
			};
			textView.Children.Add(label);
			ParentContainer.Children.Add(textView);

			// 2. a container for other content:
			RelativePanel otherSurface = new RelativePanel()
			{

			};
		}
	}
}
