using SimpleImageComparisonClassLibrary.ExtensionMethods;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace SimpleImageComparisonClassLibrary
{
    /// <summary>
    /// ImageInfo is a class which can analyze and store information about an bitmap for easy comparison of images.
    /// The information is stored as a 16x16 px grayscaled version of the image.
    /// ImageInfo can also store the path of the source image, if the ImageInfo was created from an image file.
    /// </summary>
    public class ImageInfo : IEquatable<ImageInfo>
    {
        
        #region Variables and properties

        private byte[,] _grayValues;
        public byte[,] GrayValues { 
            get => _grayValues; 
            private set { _grayValues = value; CalculateAverageBrightness(); } 
        }
        /// <summary>
        /// The average brightness in the image
        /// </summary>
        public int AverageBrightness { get; private set; }
        #endregion

        public async Task Init(WriteableBitmap image)
        {
           GrayValues = await image.GetGrayScaleValues();
        }


        #region Methods
        private void CalculateAverageBrightness()
        {
            AverageBrightness = (int)GrayValues.GetAverage();
        }

        public override string ToString()
        {
            return $"ImageInfo {{ AverageBrightness:{AverageBrightness}}}";
        }
        #endregion


        #region IEquatable implementation

        public bool Equals(ImageInfo other)
        {
            if (other == null) throw new ArgumentException("Cannot compare to null!");
            if (other is ImageInfo)
            {
                return GetHashCode() == other.GetHashCode();
            }
            else
            {
                throw new ArgumentException($"Cannot compare ImageInfo with {other.GetType().Name}");
            }
        }

        public override int GetHashCode()
        {
            var values = GrayValues.All();
            int hashCode = values.Count();
            foreach (int value in values)
            {
                hashCode = unchecked(hashCode * 314159 + value);
            }
            return hashCode;
        }
        #endregion

    }
}