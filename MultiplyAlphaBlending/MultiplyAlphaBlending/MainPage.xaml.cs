using System.Numerics;
using Windows.Foundation;
using Windows.Graphics.DirectX;
using Windows.Graphics.Imaging;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MultiplyAlphaBlending
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		private static readonly Compositor s_compositor = Window.Current.Compositor;

		private const int DIM = 600;

		public MainPage()
		{
			InitializeComponent();
			CreateScene();
		}

		private async void CreateScene()
		{
			// 1) fetch the background image:
			StorageFile imageFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/background.jpg", UriKind.RelativeOrAbsolute));
			WriteableBitmap image = await Utils.ReadImage(imageFile);
			SoftwareBitmap swBmp = SoftwareBitmap.CreateCopyFromBuffer(image.PixelBuffer, BitmapPixelFormat.Bgra8, image.PixelWidth, image.PixelHeight, BitmapAlphaMode.Premultiplied);

			// 2) add the image as background:
			ContentPageView background = new ContentPageView()
			{
				Width = DIM,
				Height = DIM
			};
			ContainerVisual container = s_compositor.CreateContainerVisual();
			container.Size = new Vector2(DIM, DIM);
			SpriteVisual imageVisual = s_compositor.CreateSpriteVisual();
			imageVisual.Size = new Vector2(DIM, DIM);
			CompositionSurfaceBrush surfBrush = s_compositor.CreateSurfaceBrush();
			CompositionDrawingSurface surface = Utils.GraphicsDevice.CreateDrawingSurface(imageVisual.Size.ToSize(),
				DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
			using (CanvasBitmap canvas = CanvasBitmap.CreateFromSoftwareBitmap(Utils.CanvasDev, swBmp))
			{
				// Draw the image to the surface
				using (CanvasDrawingSession session = CanvasComposition.CreateDrawingSession(surface))
				{
					session.Clear(Colors.Transparent);
					session.DrawImage(canvas, new Rect(0, 0, DIM, DIM));
				}
			}

			surfBrush.Surface = surface;
			imageVisual.Brush = surfBrush;
			container.Children.InsertAtTop(imageVisual);
			ElementCompositionPreview.SetElementChildVisual(background, container);

			Container.Children.Add(background);

			// 3) add the overlays:
			RelativePanel overlayPanel = new RelativePanel()
			{
				Width = DIM,
				Height = DIM
			};

			ContainerVisual container2 = s_compositor.CreateContainerVisual();
			container2.Size = new Vector2(DIM, DIM);

			SpriteVisual overlay1 = s_compositor.CreateSpriteVisual();
			overlay1.Size = new Vector2(90, 40);
			overlay1.Offset = new Vector3(120, 60, 0);
			overlay1.Brush = s_compositor.CreateColorBrush(Color.FromArgb(128, 255, 255, 0));
			container2.Children.InsertAtTop(overlay1);

			SpriteVisual overlay2 = s_compositor.CreateSpriteVisual();
			overlay2.Size = new Vector2(40, 35);
			overlay2.Offset = new Vector3(80, 560, 0);
			overlay2.Brush = s_compositor.CreateColorBrush(Color.FromArgb(128, 255, 255, 0));
			container2.Children.InsertAtTop(overlay2);

			ElementCompositionPreview.SetElementChildVisual(overlayPanel, container2);
			Container.Children.Add(overlayPanel);
		}
	}

	public class ContentPageView : FrameworkElement
	{
	}
}
