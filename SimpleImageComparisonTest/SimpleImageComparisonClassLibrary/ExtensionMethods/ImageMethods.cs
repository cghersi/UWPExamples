using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

//For reference:
//http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx


namespace SimpleImageComparisonClassLibrary.ExtensionMethods
{
  public static class ImageMethods
  {

    #region Properties
    //the font to use for the DifferenceImages
    private static readonly FontFamily DefaultFont = new FontFamily("Arial");

    //the brushes to use for the DifferenceImages
    //private static Brush[] brushes = new Brush[256];

    ////the colormatrix used to grayscale an image
    ////http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    //static readonly ColorMatrix ColorMatrix = new ColorMatrix(new float[][]
    //{
    //        new float[] {.3f, .3f, .3f, 0, 0},
    //        new float[] {.59f, .59f, .59f, 0, 0},
    //        new float[] {.11f, .11f, .11f, 0, 0},
    //        new float[] {0, 0, 0, 1, 0},
    //        new float[] {0, 0, 0, 0, 1}
    //});

    #endregion


    #region Constructor
    //static ImageMethods()
    //{
    //  try
    //  {
    //    //Create the brushes in varying intensities
    //    for (byte i = 0; i <= 255; i++)
    //    {
    //      brushes[i] = new SolidColorBrush(Color.FromArgb(255, i, (byte)(i / 3), (byte)(i / 2)));
    //    }
    //  }
    //  catch (Exception e)
    //  {
    //    int a = 0;
    //  }
    //}
    #endregion


    #region Difference methods
    /// <summary>
    /// Gets the difference between two images as a percentage
    /// </summary>
    /// <param name="img1">The first image</param>
    /// <param name="img2">The image to compare to</param>
    /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
    /// <returns>The difference between the two images as a percentage</returns>
    public static async Task<float> GetPercentageDifference(this WriteableBitmap img1, WriteableBitmap img2, int threshold = 3)
    {
      ImageInfo i1 = new ImageInfo();
      await i1.Init(img1);
      ImageInfo i2 = new ImageInfo();
      await i2.Init(img2);
      return GetPercentageDifference(i1, i2, threshold);
    }

    /// <summary>
    /// Gets the difference between two images as a percentage
    /// </summary>
    /// <param name="img1">The first imageinfo</param>
    /// <param name="img2">The imageinfo to compare to</param>
    /// <param name="threshold">How big a difference (out of 255) will be ignored - the default is 3.</param>
    /// <returns>The difference between the two images as a percentage</returns>
    public static float GetPercentageDifference(this ImageInfo img1, ImageInfo img2, int threshold = 3)
    {
      byte[] differences = img1.GrayValues.GetDifferences(img2.GrayValues);
      int numberOfPixels = differences.Length;
      float diffPixels = differences.Count(b => b > threshold);
      return diffPixels / numberOfPixels;
    }


    ///// <summary>
    ///// Finds the differences between two images and returns them in a doublearray
    ///// </summary>
    ///// <param name="img1">The first image</param>
    ///// <param name="img2">The image to compare with</param>
    ///// <returns>the differences between the two images as a doublearray</returns>
    //public static byte[,] GetDifferences(this Image img1, Image img2, int arraySize = 16)
    //{
    //  return new ImageInfo(img1).GetDifferences(new ImageInfo(img2));
    //}


    /// <summary>
    /// Finds the differences between two images and returns them in a doublearray
    /// </summary>
    /// <param name="imageInfo1">The first ImageInfo</param>
    /// <param name="imageInfo2">The ImageInfo to compare with</param>
    /// <returns>the differences between the two imageinfos as a doublearray</returns>
    public static byte[] GetDifferences(this ImageInfo imageInfo1, ImageInfo imageInfo2)
    {
      return imageInfo1.GrayValues.GetDifferences(imageInfo2.GrayValues);
    }

    #endregion


    #region Visualization methods

    ///// <summary>
    ///// Gets an image which displays the differences between two images
    ///// </summary>
    ///// <param name="img1">The first image</param>
    ///// <param name="img2">The image to compare with</param>
    ///// <param name="adjustColorSchemeToMaxDifferenceFound">Whether to adjust the color indicating maximum difference (usually 255) to the maximum difference found in this case.
    ///// E.g. if the maximum difference found is 12, then a true value in adjustColorSchemeToMaxDifferenceFound would result in 0 being black, 6 being dark pink, and 12 being bright pink.
    ///// A false value would still have differences of 255 as bright pink resulting in the 12 difference still being very dark.</param>
    ///// <param name="percentages">Whether to write percentages in each of the 255 squares (true) or the absolute value (false)</param>
    ///// <returns>an image which displays the differences between two images</returns>
    //public static Bitmap GetDifferenceImage(this Image img1, Image img2, bool adjustColorSchemeToMaxDifferenceFound = false, bool absoluteText = false)
    //{
    //  //create a 16x16 tiles image with information about how much the two images differ
    //  int cellsize = 16;  //each tile is 16 pixels wide and high
    //  Bitmap bmp = new Bitmap(16 * cellsize + 1, 16 * cellsize + 1); //16 blocks * 16 pixels + a borderpixel at left/bottom

    //  using (Graphics g = Graphics.FromImage(bmp))
    //  {
    //    g.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
    //    byte[,] differences = img1.GetDifferences(img2);
    //    byte maxDifference = 255;

    //    //if wanted - adjust the color scheme, by finding the new maximum difference
    //    if (adjustColorSchemeToMaxDifferenceFound)
    //    {
    //      maxDifference = 0;
    //      foreach (byte b in differences)
    //      {
    //        if (b > maxDifference)
    //        {
    //          maxDifference = b;
    //        }
    //      }

    //      if (maxDifference == 0)
    //      {
    //        maxDifference = 1;
    //      }
    //    }

    //    DrawDifferencesToBitmap(absoluteText, cellsize, g, differences, maxDifference);
    //  }
    //  return bmp;
    //}

    //private static void DrawDifferencesToBitmap(bool absoluteText, int cellsize, Graphics g, byte[,] differences, byte maxDifference)
    //{
    //  for (int y = 0; y < differences.GetLength(1); y++)
    //  {
    //    for (int x = 0; x < differences.GetLength(0); x++)
    //    {
    //      byte cellValue = differences[x, y];
    //      string cellText;

    //      if (absoluteText)
    //      {
    //        cellText = cellValue.ToString();
    //      }
    //      else
    //      {
    //        cellText = string.Format("{0}%", (int)cellValue);
    //      }

    //      float percentageDifference = (float)differences[x, y] / maxDifference;
    //      int colorIndex = (int)(255 * percentageDifference);

    //      g.FillRectangle(brushes[colorIndex], x * cellsize, y * cellsize, cellsize, cellsize);
    //      g.DrawRectangle(Pens.Blue, x * cellsize, y * cellsize, cellsize, cellsize);
    //      SizeF size = g.MeasureString(cellText, DefaultFont);
    //      g.DrawString(cellText, DefaultFont, Brushes.Black, x * cellsize + cellsize / 2 - size.Width / 2 + 1, y * cellsize + cellsize / 2 - size.Height / 2 + 1);
    //      g.DrawString(cellText, DefaultFont, Brushes.White, x * cellsize + cellsize / 2 - size.Width / 2, y * cellsize + cellsize / 2 - size.Height / 2);
    //    }
    //  }
    //}


    #endregion


    /// <summary>
    /// Gets the lightness of the image in 256 sections (16x16)
    /// </summary>
    /// <param name="img">The image to get the lightness for</param>
    /// <returns>A doublearray (16x16) containing the lightness of the 256 sections</returns>
    public static async Task<byte[]> GetGrayScaleValues(this WriteableBitmap img, uint arraySize = 16)
    {
      byte[] resized = await img.GetResizedVersion(arraySize, arraySize);
      return resized.GetGrayScaleVersion();
    }

    #region Image conversion methods

    /// <summary>
    /// Gets a grayscaled version of an image
    /// </summary>
    /// <param name="original">The image to grayscale</param>
    /// <returns>A grayscale version of the image</returns>
    //public static Image GetGrayScaleVersion(this Image original)
    //{
    //    //http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
    //    //create a blank bitmap the same size as original
    //    Bitmap newBitmap = new Bitmap(original.Width, original.Height);

    //    //get a graphics object from the new image
    //    using (Graphics g = Graphics.FromImage(newBitmap))
    //    {
    //        //create some image attributes
    //        ImageAttributes attributes = new ImageAttributes();

    //        //set the color matrix attribute
    //        attributes.SetColorMatrix(ColorMatrix);

    //        //draw the original image on the new image
    //        //using the grayscale color matrix
    //        g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
    //           0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
    //    }
    //    return newBitmap;

    //}

    public static byte[] GetGrayScaleVersion(this byte[] srcPixels, uint arraySize = 16)
    {
      //// Get the source bitmap pixels
      //byte[] srcPixels = new byte[4 * srcBitmap.PixelWidth * srcBitmap.PixelHeight];

      //using (Stream pixelStream = srcBitmap.PixelBuffer.AsStream())
      //{
      //  await pixelStream.ReadAsync(srcPixels, 0, srcPixels.Length);
      //}

      // Create a destination bitmap and pixels array
      //WriteableBitmap dstBitmap = new WriteableBitmap(srcBitmap.PixelWidth, srcBitmap.PixelHeight);
      //byte[] dstPixels = new byte[4 * dstBitmap.PixelWidth * dstBitmap.PixelHeight];

      byte[] res = new byte[arraySize * arraySize];
      int resIdx = 0;
      for (int i = 0; i < srcPixels.Length; i += 4)
      {
        double b = (double)srcPixels[i] / 255.0;
        double g = (double)srcPixels[i + 1] / 255.0;
        double r = (double)srcPixels[i + 2] / 255.0;

        byte a = srcPixels[i + 3];

        double e = (0.3 * r + 0.59 * g + 0.11 * b) * 255;
        //double e = (0.21 * r + 0.71 * g + 0.07 * b) * 255;
        byte f = Convert.ToByte(e);

        res[resIdx++] = f;
        //dstPixels[i] = f;
        //dstPixels[i + 1] = f;
        //dstPixels[i + 2] = f;
        //dstPixels[i + 3] = a;
      }
      return res;

      //// Move the pixels into the destination bitmap
      //using (Stream pixelStream = dstBitmap.PixelBuffer.AsStream())
      //{
      //  await pixelStream.WriteAsync(dstPixels, 0, dstPixels.Length);
      //}
      //dstBitmap.Invalidate();

      //return dstBitmap;
    }

    public static async Task<byte[]> GetResizedVersion(this WriteableBitmap baseWriteBitmap, uint width, uint height)
    {
      // Get the pixel buffer of the writable bitmap in bytes
      Stream stream = baseWriteBitmap.PixelBuffer.AsStream();
      byte[] pixels = new byte[(uint)stream.Length];
      await stream.ReadAsync(pixels, 0, pixels.Length);
      //Encoding the data of the PixelBuffer we have from the writable bitmap
      var inMemoryRandomStream = new InMemoryRandomAccessStream();
      var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, inMemoryRandomStream);
      encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)baseWriteBitmap.PixelWidth, (uint)baseWriteBitmap.PixelHeight, 96, 96, pixels);
      await encoder.FlushAsync();
      // At this point we have an encoded image in inMemoryRandomStream
      // We apply the transform and decode
      var transform = new BitmapTransform
      {
        ScaledWidth = width,
        ScaledHeight = height
      };
      inMemoryRandomStream.Seek(0);
      var decoder = await BitmapDecoder.CreateAsync(inMemoryRandomStream);
      var pixelData = await decoder.GetPixelDataAsync(
              BitmapPixelFormat.Bgra8,
              BitmapAlphaMode.Straight,
              transform,
              ExifOrientationMode.IgnoreExifOrientation,
              ColorManagementMode.DoNotColorManage);
      // An array containing the decoded image data
      byte[] sourceDecodedPixels = pixelData.DetachPixelData();
      return sourceDecodedPixels;
      //// Approach 1 : Encoding the image buffer again:
      //// Encoding data
      //var bitmap = new WriteableBitmap((int)width, (int)height);
      //using (var memStream = new MemoryStream(sourceDecodedPixels))
      //{
      //  using (Stream streamOut = bitmap.PixelBuffer.AsStream())
      //  {
      //    await memStream.CopyToAsync(stream);
      //  }
      //}
      //return bitmap;
    }

    ///// <summary>
    ///// Gets a resized copy of an image
    ///// </summary>
    ///// <param name="originalImage">The image to resize</param>
    ///// <param name="newWidth">The new width in pixels</param>
    ///// <param name="newHeight">The new height in pixels</param>
    ///// <returns>A resized version of the original image</returns>
    //public static Image GetResizedVersion(this Image originalImage, int newWidth, int newHeight, bool highSpeed = true)
    //    {
    //        Image smallVersion = new Bitmap(newWidth, newHeight);
    //        using (Graphics g = Graphics.FromImage(smallVersion))
    //        {
    //            if (highSpeed)
    //            {
    //                g.SmoothingMode = SmoothingMode.Default;
    //                g.InterpolationMode = InterpolationMode.Low;
    //                g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
    //            }
    //            else
    //            {
    //                g.SmoothingMode = SmoothingMode.HighQuality;
    //                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
    //                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
    //            }

    //            g.DrawImage(originalImage, 0, 0, newWidth, newHeight);
    //        }

    //        return smallVersion;
    //    }

    #endregion


    #region Histogram methods
    ///// <summary>
    ///// Gets a bitmap with the RGB histograms of a bitmap
    ///// </summary>
    ///// <param name="bitmap">The bitmap to get the histogram for</param>
    ///// <returns>A bitmap with the histogram for R, G and B values</returns>
    //public static async Task<WriteableBitmap> GetRgbHistogramBitmap(this WriteableBitmap bitmap)
    //    {
    //        return (await GetRgbHistogram(bitmap)).Visualize();
    //    }

    ///// <summary>
    ///// Get a histogram for a bitmap
    ///// </summary>
    ///// <param name="bitmap">The bitmap to get the histogram for</param>
    ///// <returns>A histogram for the bitmap</returns>
    //public static async Task<Histogram> GetRgbHistogram(this WriteableBitmap bitmap)
    //{
    //  return await Histogram.CalculateHistogram(bitmap);
    //}
    #endregion


    #region ImageInfo methods

    //public static ImageInfo ToImageInfo(this Image image) => new ImageInfo(image);

    #endregion

  }
}