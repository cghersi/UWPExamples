//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace FoxitTestBench
{
	/// <summary>
	/// A collection of common utilities for basic purposes.
	/// </summary>
	public static class Utilities
	{
		public static readonly DateTime EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public const double REAL_MIN_PRECISION_FOR_EQUALS = 0.0000001F;

		#region Dates and Times
		///////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Returns the time span between the given value and the current instance.
		/// </summary>
		/// <param name="input"></param>
		/// <returns>Positive value if the input is later than current instant.
		/// Negative value if the input is earlier than current instant.</returns>
		public static TimeSpan DiffFromNow(this DateTime input)
		{
			return DateTime.Now - input;
		}

		#endregion Dates and Times
		///////////////////////////////////////////////////////////////////////////

		#region Equality
		///////////////////////////////////////////////////////////////////////////

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="prec">minimum difference to return false</param>
		/// <returns></returns>
		public static bool Equal(this double arg1, double arg2, double prec = REAL_MIN_PRECISION_FOR_EQUALS)
		{
			return Math.Abs(arg1 - arg2) < prec;
		}

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="prec">minimum difference to return false</param>
		/// <returns></returns>
		public static bool Equal(this float arg1, float arg2, double prec = REAL_MIN_PRECISION_FOR_EQUALS)
		{
			return Math.Abs(arg1 - arg2) < prec;
		}

		/// <summary>
		/// Returns true if the two inputs differ for less than <see cref="REAL_MIN_PRECISION_FOR_EQUALS"/>.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="prec">minimum difference to return false</param>
		/// <returns></returns>
		public static bool Equal(this double? arg1, double? arg2, double prec = REAL_MIN_PRECISION_FOR_EQUALS)
		{
			if (arg1 == null)
				return (arg2 == null);
			if (arg2 == null)
				return false;
			return Math.Abs(arg1.Value - arg2.Value) < prec;
		}

		/// <summary>
		/// Returns true if the two inputs are equal (both Size and Origin).
		/// It checks for null inputs also.
		/// </summary>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		/// <param name="prec">minimum difference to return false</param>
		/// <returns></returns>
		public static bool Equal(this LTRect arg1, LTRect arg2, double prec = REAL_MIN_PRECISION_FOR_EQUALS)
		{
			return arg1.X.Equal(arg2.X, prec) && arg1.Y.Equal(arg2.Y, prec) &&
						 arg1.W.Equal(arg2.W, prec) && arg1.H.Equal(arg2.H, prec);
		}

		#endregion Equality
		///////////////////////////////////////////////////////////////////////////

		#region Geometry
		///////////////////////////////////////////////////////////////////////////

		public static LTRect ApplyRotation(this LTRect input, int rotation)
		{
			if (input.Equal(Encoders.RECT_ZERO))
				return Encoders.RECT_ZERO;

			if ((rotation == 90) || (rotation == 270))
				return Encoders.BuildLTRect(input.X, input.Y, input.H, input.W); //flip W <-> H
			else
				return input;
		}

		/// <summary>
		/// Returns the largest X coordinate for the given rectangle, i.e. Left + Width
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static double GetMaxX(this LTRect input)
		{
			return input.X + input.W;
		}

		/// <summary>
		/// Returns the X coordinate in the middle of the bottom edge for the given rectangle, i.e. Left + Width / 2
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static double GetMidX(this LTRect input)
		{
			return input.X + (input.W / 2);
		}

		/// <summary>
		/// Returns the Y coordinate in the middle of the left edge for the given rectangle,, i.e. Top + Height / 2
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static double GetMidY(this LTRect input)
		{
			return input.Y + (input.H / 2);
		}

		/// <summary>
		/// Returns the largest Y coordinate for the given rectangle, i.e. Top + Height
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static double GetMaxY(this LTRect input)
		{
			return input.Y + input.H;
		}

		/// <summary>
		/// This does 2 things. First, it optionally rotates the rectangle (90, 180, 270 degrees), and then converts
		/// from one size space to another.
		/// </summary>
		/// <param name="pt1"></param>
		/// <param name="fromRect"></param>
		/// <param name="toRect"></param>
		/// <param name="flip">flips the point around the central x-axis of the source rect.
		/// So if the source rect was 0,0,1,1, and our point is a,b, then flip switches the source point to (a,1-b).
		/// This is super useful when we go from top-origin coordinate systems (like UIKit) to
		/// bottom-origin coordinate systems (like PSPDFKit PDF space).</param>
		/// <param name="degrees"></param>
		/// <returns></returns>
		public static LTPoint ConvertPoint(this LTPoint pt1, LTRect fromRect, LTRect toRect, bool flip, int? degrees = null)
		{
			// Optionally rotate the source rect (with its contents) by 90, 180, or 270 degrees:
			double fromRectH = fromRect.H;
			double fromRectW = fromRect.W;
			if (degrees != null)
			{
				double a = pt1.X - fromRect.X;
				double b = pt1.Y - fromRect.Y;

				switch (degrees.Value)
				{
					case 90:
						fromRectW = fromRect.H;
						fromRectH = fromRect.W;
						pt1 = Encoders.BuildLTPoint(b + fromRect.X, fromRect.H - a + fromRect.Y);
						break;
					case 180:
						pt1 = Encoders.BuildLTPoint(fromRect.W - a + fromRect.X, fromRect.H - b + fromRect.Y);
						break;
					case 270:
					case -90:
						fromRectW = fromRect.H;
						fromRectH = fromRect.W;
						pt1 = Encoders.BuildLTPoint(fromRect.W - b + fromRect.X, a + fromRect.Y);
						break;
				}
			}

			double x = (pt1.X - fromRect.X) / fromRectW * toRect.W + toRect.X;
			double y = flip ?
				(1.0f - (pt1.Y - fromRect.Y) / fromRectH) * toRect.H + toRect.Y :
				((pt1.Y - fromRect.Y) / fromRectH) * toRect.H + toRect.Y;
			//double y = ((pt1.Y - fromRect.Y) / fromRectH) * toRect.H + toRect.Y;

			return Encoders.BuildLTPoint(x, y);
		}

		/// <summary>
		/// Determines the rectangle that includes the two given points.
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <returns></returns>
		public static LTRect GetRect(this LTPoint p1, LTPoint p2)
		{
			double xMin = Math.Min(p1.X, p2.X);
			double yMin = Math.Min(p1.Y, p2.Y);
			double xMax = Math.Max(p1.X, p2.X);
			double yMax = Math.Max(p1.Y, p2.Y);
			return Encoders.BuildLTRect(xMin, yMin, xMax - xMin, yMax - yMin);
		}

		#endregion Geometry
		///////////////////////////////////////////////////////////////////////////

		private static BitmapPropertySet s_jpgPropertySet = null;

		/// <summary>
		/// Turns the given input into a byte array.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="returnRawFormat">true to return the raw byte[], false to return a JPG-encoded image.</param>
		/// <returns></returns>
		public static async Task<byte[]> ToByteArray(this SoftwareBitmap image, bool returnRawFormat)
		{
			if (image == null)
				return null;

			if (returnRawFormat)
			{
				byte[] res = new byte[4 * image.PixelWidth * image.PixelHeight];
				image.CopyToBuffer(res.AsBuffer());
				return res;
			}

			if (s_jpgPropertySet == null)
			{
				s_jpgPropertySet = new BitmapPropertySet();
				BitmapTypedValue qualityValue = new BitmapTypedValue(0.85, PropertyType.Single);
				s_jpgPropertySet.Add("ImageQuality", qualityValue);
			}

			byte[] array;
			using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
			{
				BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ms, s_jpgPropertySet);
				encoder.SetSoftwareBitmap(image);

				try
				{
					await encoder.FlushAsync();
				}
				catch
				{
					//nothing to do here...
				}

				array = new byte[ms.Size];
				await ms.ReadAsync(array.AsBuffer(), (uint)ms.Size, InputStreamOptions.None);
			}

			return array;
		}
	}
}