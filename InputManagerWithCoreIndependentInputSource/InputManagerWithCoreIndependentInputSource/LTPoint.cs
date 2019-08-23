//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// Client representation of a point.
	/// </summary>
	public struct LTPoint : IEquatable<LTPoint>
	{
		/// <summary>
		/// X coordinate of the point.
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// Y coordinate of the point.
		/// </summary>
		public double Y { get; set; }

		public LTPoint(double x, double y)
		{
			X = x;
			Y = y;
		}

		public override string ToString()
		{
			return string.Format("{0:N1}, {1:N1}", X, Y);
		}

		public bool Equals(LTPoint other)
		{
			return X.Equal(other.X) && Y.Equal(other.Y);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = X.GetHashCode();
				hashCode = (hashCode * 397) ^ Y.GetHashCode();
				return hashCode;
			}
		}
	}
}