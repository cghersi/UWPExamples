using Microsoft.Graphics.Canvas;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Graphics.Canvas.UI.Composition;

namespace MultiplyAlphaBlending
{
	public static class Utils
	{
		public static readonly Compositor Compositor = Window.Current.Compositor;
		public static readonly CanvasDevice CanvasDev = CanvasDevice.GetSharedDevice();
		public static readonly CompositionGraphicsDevice GraphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(Compositor, CanvasDev);

		public static async Task<WriteableBitmap> ReadImage(StorageFile imageFile)
		{
			WriteableBitmap writeableBitmap;
			using (IRandomAccessStream imageStream = await imageFile.OpenReadAsync())
			{
				BitmapDecoder bitmapDecoder = await BitmapDecoder.CreateAsync(imageStream);

				BitmapTransform dummyTransform = new BitmapTransform();
				PixelDataProvider pixelDataProvider =
					await bitmapDecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
						BitmapAlphaMode.Premultiplied, dummyTransform,
						ExifOrientationMode.RespectExifOrientation,
						ColorManagementMode.ColorManageToSRgb);
				byte[] pixelData = pixelDataProvider.DetachPixelData();

				writeableBitmap = new WriteableBitmap(
					(int)bitmapDecoder.OrientedPixelWidth,
					(int)bitmapDecoder.OrientedPixelHeight);
				using (Stream pixelStream = writeableBitmap.PixelBuffer.AsStream())
				{
					await pixelStream.WriteAsync(pixelData, 0, pixelData.Length);
				}
			}

			return writeableBitmap;
		}
	}
}
