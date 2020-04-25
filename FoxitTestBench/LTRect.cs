//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

// ReSharper disable UnusedMember.Global

// ReSharper disable NonReadonlyMemberInGetHashCode

using System;

namespace FoxitTestBench
{
	/// <summary>
	/// Client representation of a rectangle, natively defined in terms of X-Y coordinates of the TopLeft corner,
	/// and width-height.
	/// </summary>
	public struct LTRect : IEquatable<LTRect>
	{
		public LTRect(double originX, double originY, double sizeW, double sizeH)
		{
			X = originX;
			Y = originY;
			W = sizeW;
			H = sizeH;
		}

		/// <summary>
		/// X coordinate of the TopLeft corner.
		/// </summary>
		public double X { get; private set; }

		/// <summary>
		/// Y coordinate of the TopLeft corner.
		/// </summary>
		public double Y { get; private set; }

		/// <summary>
		/// Width of the Rectangle.
		/// </summary>
		public double W { get; }

		/// <summary>
		/// Height of the Rectangle.
		/// </summary>
		public double H { get; }

		/// <summary>
		/// Size of the rectangle.
		/// </summary>
		public LTSize Size()
		{
			return Encoders.BuildLTSize(W, H);
		}

		/// <summary>
		/// Retrieves the coordinates of the TopLeft corner.
		/// </summary>
		public LTPoint Origin()
		{
			return Encoders.BuildLTPoint(X, Y);
		}

		/// <summary>
		/// Retrieves the coordinates of the BottomRight corner.
		/// </summary>
		/// <returns></returns>
		public LTPoint BottomRight()
		{
			return Encoders.BuildLTPoint(X + W, Y + H);
		}

		/// <summary>
		/// Retrieves the coordinates of the Top right corner.
		/// </summary>
		/// <returns></returns>
		public LTPoint TopRight()
		{
			return Encoders.BuildLTPoint(X + W, Y);
		}

		/// <summary>
		/// Retrieves the coordinates of the Bottom left corner.
		/// </summary>
		/// <returns></returns>
		public LTPoint BottomLeft()
		{
			return Encoders.BuildLTPoint(X, Y + H);
		}

		/// <summary>
		/// Sets the coordinates of the TopLeft corner.
		/// </summary>
		/// <param name="newOrigin">if null, nothing will be set;
		/// otherwise the X and Y coordinates will be set to this value.</param>
		public void SetOrigin(LTPoint newOrigin)
		{
			if (newOrigin.Equals(Encoders.POINT_NULL))
				return;
			X = newOrigin.X;
			Y = newOrigin.Y;
		}

		public override string ToString()
		{
			return string.Format("{0:N1}, {1:N1} | w:{2:N1} h:{3:N1}", X, Y, W, H);
		}

		public bool Equals(LTRect other)
		{
			return X.Equal(other.X) && Y.Equal(other.Y) && W.Equal(other.W) && H.Equal(other.H);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				hashCode = (hashCode * 397) ^ W.GetHashCode();
				hashCode = (hashCode * 397) ^ H.GetHashCode();
				return hashCode;
			}
		}
	}
}