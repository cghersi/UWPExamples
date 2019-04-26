using System;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ImageBrushes
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		// original size is 839x1081:
		private readonly Uri m_imgUri = new Uri("ms-appx:///Assets/Anger.png", UriKind.RelativeOrAbsolute);
		private readonly ImageWithMask m_img;
		private static readonly StorageFolder FOLDER = ApplicationData.Current.LocalFolder;

		private int m_moveDeltaX = 100;

		public MainPage()
		{
			InitializeComponent();

			// set a custom size:
			Size imgSize = new Size(80, 130);

			// create the image and attache to container:
			m_img = new ImageWithMask(ImageContainer, imgSize);
			m_img.SetImage(m_imgUri);

			PointerManager pointerManager = new PointerManager(m_img);
			m_img.MaskPath.PointerPressed += pointerManager.OnPointerDown;
			m_img.MaskPath.PointerMoved += pointerManager.OnPointerMoved;
			m_img.MaskPath.PointerReleased += pointerManager.OnPointerUp;
		}

		private async void SetImage_OnClick(object sender, RoutedEventArgs e)
		{
			// replicates the image by saving onto file system and reading the byte[] content:
			byte[] content = await ToByteArrayImage(m_imgUri);
			string imgName = await SaveImage(content);
			Uri storedImgUri = GetUri(imgName);
			m_img.SetImage(storedImgUri);
			Move(m_img.MaskPath, m_moveDeltaX, 0);
			m_moveDeltaX += 100;
		}

		private static void Move(FrameworkElement panel, double deltaX, double deltaY)
		{
			if (panel == null)
				return;
			Thickness margin = panel.Margin;
			margin.Left += deltaX;
			margin.Top += deltaY;
			panel.Margin = margin;
		}

		private static async Task<byte[]> ToByteArrayImage(Uri imageUri)
		{
			RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(imageUri);
			IRandomAccessStreamWithContentType streamWithContent = await streamRef.OpenReadAsync();
			byte[] buffer = new byte[streamWithContent.Size];
			await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);
			return buffer;
		}

		private static Uri GetUri(string filename)
		{
			string fullPath = FOLDER.Path + '\\' + filename;
			return new Uri(fullPath, UriKind.RelativeOrAbsolute);
		}

		private static async Task<string> SaveImage(byte[] imgContent)
		{
			if (imgContent == null)
				return string.Empty;
			string name = string.Format("img{0}.png", DateTime.Now.Ticks);
			bool imgWritten = await WriteToFile(imgContent, name);
			return imgWritten ? name : string.Empty;
		}

		private static async Task<bool> WriteToFile(byte[] content, string filename)
		{
			if ((content == null) || string.IsNullOrEmpty(filename))
				return false;

			// 1) try to retrieve / create the file:
			StorageFile file;
			try
			{
				file = await FOLDER.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot create file {0} due to {1}", filename, e.Message);
				return false;
			}

			if (file == null)
				return false;

			// 2) write onto the file:
			try
			{
				await FileIO.WriteBytesAsync(file, content);
				return true;
			}
			catch (Exception e)
			{
				Debug.WriteLine("Cannot write on file {0} due to {1}", filename, e.Message);
				return false;
			}
		}
	}
}
