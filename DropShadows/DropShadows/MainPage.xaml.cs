using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DropShadows
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page, IShadowPresenter
	{

		private DropShadowMetadata m_shadowOptions = new DropShadowMetadata(null);
		private Path m_bezierPath = null; //TODO in case you want to try with custom shapes

		public MainPage()
		{
			this.InitializeComponent();

			// set up the shadow for the element:
			m_shadowOptions.Shape = m_bezierPath;
			this.ManageShadow(true, View, ref m_shadowOptions);
		}

		public Path ShapeForShadow => m_bezierPath;
		public Panel View => ShadowPres1;
	}
}
