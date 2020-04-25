//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

namespace FoxitTestBench
{
	/// <summary>
	/// Provides extension methods to encode/decode Binary objects.
	/// </summary>
	public static class Encoders
	{
		public static readonly LTPoint POINT_NULL = BuildLTPoint(double.MaxValue, double.MaxValue);
		public static readonly LTPoint POINT_ZERO = BuildLTPoint(0, 0);
		public static readonly LTSize SIZE_ZERO = BuildLTSize(0, 0);
		public static readonly LTSize SIZE_ONE = BuildLTSize(1, 1);
		public static readonly LTRect RECT_ZERO = BuildLTRect(0, 0, 0, 0);
		public static readonly LTRect RECT_ONE = BuildLTRect(0, 0, 1, 1);

		/// <summary>
		/// Creates a new LTPoint with the given input.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>an LTPoint with the given properties. It is never null.</returns>
		public static LTPoint BuildLTPoint(double x, double y)
		{
			return new LTPoint(x, y);
		}

		/// <summary>
		/// Creates a new LTPoint with the given input.
		/// </summary>
		/// <param name="x">X coordinate</param>
		/// <param name="y">Y coordinate</param>
		/// <returns>an LTPoint with the given properties; null if all input are null.</returns>
		public static LTPoint BuildLTPoint(double? x, double? y)
		{
			if (!x.HasValue && !y.HasValue)
				return POINT_ZERO;
			return new LTPoint(x ?? 0, y ?? 0);
		}

		/// <summary>
		/// Creates a new LTRect with the given input. 
		/// </summary>
		/// <param name="originX">X coordinate of the position of the top left corner of the rectangle</param>
		/// <param name="originY">Y coordinate of the position of the top left corner of the rectangle</param>
		/// <param name="sizeW">Width of the rectangle</param>
		/// <param name="sizeH">Height of the rectangle</param>
		/// <returns>an LTRect with the given properties. It is never null.</returns>
		public static LTRect BuildLTRect(double originX, double originY, double sizeW, double sizeH)
		{
			return new LTRect(originX, originY, sizeW, sizeH);
		}

		/// <summary>
		/// Creates a new LTRect with the given input. 
		/// </summary>
		/// <param name="topLeft">top left corner of the rectangle</param>
		/// <param name="bottomRight">bottom right corner of the rectangle</param>
		/// <returns>an LTRect with the given properties. It is never null.</returns>
		public static LTRect BuildLTRect(LTPoint topLeft, LTPoint bottomRight)
		{
			double tlX = topLeft.X;
			double tlY = topLeft.Y;
			return new LTRect(tlX, tlY, bottomRight.X - tlX, bottomRight.Y - tlY);
		}

		/// <summary>
		/// Creates a new LTRect with the given input. 
		/// </summary>
		/// <param name="topLeft">top left corner of the rectangle</param>
		/// <param name="size">width and height of the rectangle</param>
		/// <returns>an LTRect with the given properties. It is never null.</returns>
		public static LTRect BuildLTRect(LTPoint topLeft, LTSize size)
		{
			return new LTRect(topLeft.X, topLeft.Y, size.Width, size.Height);
		}

		/// <summary>
		/// Creates a new LTSize with the given input. 
		/// </summary>
		/// <param name="width">Width of the rectangle</param>
		/// <param name="height">Height of the rectangle</param>
		/// <returns>an LTSize with the given properties. It is never null.</returns>
		public static LTSize BuildLTSize(double width, double height)
		{
			return new LTSize(width, height);
		}

		/// <summary>
		/// Creates a new LTPositionInDoc with the given input.
		/// It will be glyphBased, with null PointStable and only relying on the GlyphIndex.
		/// </summary>
		/// <param name="page">Zero-based number of page in the PDF document</param>
		/// <param name="glyphIndex">Character index in the page in the PDF document, taken directly from PSPDFKit</param>
		/// <returns>an LTPositionInDoc with the given properties. It is never null.</returns>
		public static LTPositionInDoc BuildLTPositionInDoc(int page, int glyphIndex)
		{
			return new LTPositionInDoc
			{
				Page = page,
				GlyphBased = true,
				GlyphIndex = glyphIndex,
				PointStable = null
			};
		}
	}
}