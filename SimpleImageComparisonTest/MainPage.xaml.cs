using SimpleImageComparisonClassLibrary.ExtensionMethods;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SimpleImageComparisonTest
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
      await Test();
    }

    private static async Task Test()
    {
      Console.WriteLine("Hello World!");

      StringBuilder sb = new StringBuilder();
      Stopwatch sw = Stopwatch.StartNew();
      try
      {
        for (int pagA = 0; pagA < 9; pagA++)
        {
          float maxSimilarity = 0;
          int indexOfMaxSimilarity = -1;
          float minDiff = 100;
          int indexOfMinDiff = -1;
          for (int pagB = 0; pagB < 12; pagB++)
          {
            sb.AppendLine();
            string file1 = string.Format("docA_pag{0}.jpg", pagA);
            string file2 = string.Format("docB_pag{0}.jpg", pagB);
            //float sim = ComputeSimilarity(@"C:\Share\TestVision\" + file1, @"C:\Share\TestVision\" + file2);
            float diff = await GetDiff(file1, file2);
            //float diff = SimpleImageComparisonClassLibrary.ImageTool.GetPercentageDifference(@"C:\Share\TestVision\" + file1, @"C:\Share\TestVision\" + file2);
            sb.AppendFormat("doc A pag {0} vs doc B pag {1} = {2}", pagA, pagB, diff);
            //if (maxSimilarity < sim)
            //{
            //  maxSimilarity = sim;
            //  indexOfMaxSimilarity = pagB;
            //}

            if (minDiff > diff)
            {
              minDiff = diff;
              indexOfMinDiff = pagB;
            }
          }

          sb.AppendLine();
          sb.AppendLine();

          sb.AppendFormat("doc A pag {0} => max similarity {2} with doc B pag {1} = {2}", pagA, indexOfMaxSimilarity, maxSimilarity);
          sb.AppendLine();
          sb.AppendFormat("doc A pag {0} => min diff {2} with doc B pag {1} = {2}", pagA, indexOfMinDiff, minDiff);

          sb.AppendLine();
          sb.AppendLine();
        }
        sw.Stop();

        
        File.WriteAllText(ApplicationData.Current.LocalFolder.Path + "\\res.txt", sb.ToString());
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }

      Console.WriteLine("finished in {0}ms", sw.ElapsedMilliseconds);
    }

    private static async Task<float> GetDiff(string file1, string file2)
    {
      try
      {
        WriteableBitmap img1 = await OpenWriteableBitmapFile(file1);
        WriteableBitmap img2 = await OpenWriteableBitmapFile(file2);
        float diff = await ImageMethods.GetPercentageDifference(img1, img2);
        return diff;
      }
      catch (Exception)
      {
        return -1;
      }
    }

    private static async Task<WriteableBitmap> OpenWriteableBitmapFile(string path)
    {
      StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(path);
      using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
      {
        BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
        WriteableBitmap image = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
        image.SetSource(stream);

        return image;
      }
    }
  }
}
