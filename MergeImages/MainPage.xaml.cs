using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MergeImages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private Canvas canvas1;
		private Canvas canvas2;

		public MainPage()
		{
			InitializeComponent();

			canvas1 = new Canvas()
			{
				Width = 200,
				Height = 300,
				Background = new SolidColorBrush(Colors.Yellow),
				Margin = new Thickness(0)
			};
			MainContainer.Children.Add(canvas1);

			Rectangle r1 = new Rectangle()
			{
				Width = 50,
				Height = 30,
				Margin = new Thickness(30),
				Fill = new SolidColorBrush(Colors.Blue)
			};
			canvas1.Children.Add(r1);

			TextBlock tb1 = new TextBlock()
			{
				Margin = new Thickness(20, 40, 0, 0),
				Text = "First text for test"
			};
			canvas1.Children.Add(tb1);


			canvas2 = new Canvas()
			{
				Width = 400,
				Height = 200,
				Background = new SolidColorBrush(Colors.Red),
				Margin = new Thickness(100, 200, 0 ,0)
			};
			MainContainer.Children.Add(canvas2);

			Rectangle r2 = new Rectangle()
			{
				Width = 50,
				Height = 30,
				Margin = new Thickness(30),
				Fill = new SolidColorBrush(Colors.Green)
			};
			canvas2.Children.Add(r2);

			TextBlock tb2 = new TextBlock()
			{
				Margin = new Thickness(20, 40, 0, 0),
				Text = "Second text for test"
			};
			canvas2.Children.Add(tb2);

		}

		private async void MergeClick(object sender, RoutedEventArgs e)
		{
			Rect finalRect = GetFrame(canvas1);
			Rect rect2 = GetFrame(canvas2);
			finalRect.Union(rect2);

			WriteableBitmap writeableBmp1 = await GetBitmap(canvas1);
			WriteableBitmap writeableBmp2 = await GetBitmap(canvas2);

			WriteableBitmap result = new WriteableBitmap((int)finalRect.Width, (int)finalRect.Height);
			result.Clear(Colors.Gray);
			result.Blit(GetFrame(canvas1), writeableBmp1, GetBounds(canvas1));
			result.Blit(GetFrame(canvas2), writeableBmp2, GetBounds(canvas2));

			Result.Source = result;

			//// Clear the WriteableBitmap with white color
			//writeableBmp.Clear(Colors.Yellow);

			//writeableBmp.

			//writeableBmp.Blit()
		}

		private static Rect GetFrame(FrameworkElement element)
		{
			return new Rect(element.Margin.Left, element.Margin.Top, element.Width, element.Height);
		}

		private static Rect GetBounds(FrameworkElement element)
		{
			return new Rect(0, 0, element.Width, element.Height);
		}

		private static async Task<WriteableBitmap> GetBitmap(FrameworkElement element)
		{
			byte[] img1 = await TakeScreenshot(element);
			if (img1 == null)
				return null;

			WriteableBitmap writeableBmp1 = new WriteableBitmap((int)element.Width, (int)element.Height);
			using (Stream stream = writeableBmp1.PixelBuffer.AsStream())
			{
				await stream.WriteAsync(img1, 0, img1.Length);
			}

			return writeableBmp1;
		}

		private static async Task<byte[]> TakeScreenshot(FrameworkElement element)
		{
			// create and capture Window:
			int w = (int)element.ActualWidth;
			int h = (int)element.ActualHeight;
			if ((w <= 0) || (h <= 0))
				return null;
			RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap();

			try
			{
				await renderTargetBitmap.RenderAsync(element);
				IBuffer buffer = await renderTargetBitmap.GetPixelsAsync();
				return buffer.ToArray();
			}
			catch (Exception e)
			{
				return null;
			}
		}
	}
}
