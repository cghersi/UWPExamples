using AForge.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AForgeTest
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page
  {
    public MainPage()
    {
      this.InitializeComponent();
    }

    private async void btnTest_Click(object sender, RoutedEventArgs e)
    {
      await ComputeSimilarity("docA_pag0.jpg", "docB_pag0.jpg");
    }

    private static async Task<float> ComputeSimilarity(string file1, string file2)
    {
      var f1 = await ApplicationData.Current.LocalFolder.GetFileAsync(file1);
      var f2 = await ApplicationData.Current.LocalFolder.GetFileAsync(file2);

      ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0);

      System.Drawing.Bitmap image = (Bitmap)System.Drawing.Image.FromStream((await f1.OpenReadAsync()).AsStreamForRead());
      System.Drawing.Bitmap template = (Bitmap)System.Drawing.Image.FromStream((await f2.OpenReadAsync()).AsStreamForRead());

      System.Drawing.Imaging.BitmapData val = image.LockBits(new Rectangle(0, 0, ((Image)image).get_Width(), ((Image)image).get_Height()), (ImageLockMode)1, ((Image)image).get_PixelFormat());
      System.Drawing.Imaging.BitmapData val2 = template.LockBits(new Rectangle(0, 0, ((Image)template).get_Width(), ((Image)template).get_Height()), (ImageLockMode)1, ((Image)template).get_PixelFormat());

      // return 0;

      // compare two images
      TemplateMatch[] matchings = tm.ProcessImage(image, template);
      //// check similarity level
      //return matchings[0].Similarity;
    }

    private static System.Drawing.Imaging.BitmapData LockBits(int w, int h, System.Drawing.Imaging.ImageLockMode readOnly, System.Drawing.Imaging.PixelFormat pixelFormat)
    {
      switch (pixelFormat)
      {
        case System.Drawing.Imaging.PixelFormat.Undefined:
        case System.Drawing.Imaging.PixelFormat.Indexed:
        case System.Drawing.Imaging.PixelFormat.Alpha:
        case System.Drawing.Imaging.PixelFormat.PAlpha:
          throw new ArgumentException("LockBits method only applicable to pixel formats with prefix Format", "pixelFormat");
        default:
          if (!pixelFormat.Equals(_pixelFormat))
          {
            throw new ArgumentException($"Bitmap.PixelFormat = {_pixelFormat}", "pixelFormat");
          }

          return new System.Drawing.Imaging.BitmapData(w, h, _stride, _pixelFormat, _scan0);
      }
    }
  }
}
