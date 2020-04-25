//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------

using System;
// ReSharper disable NonReadonlyMemberInGetHashCode

namespace FoxitTestBench
{
	/// <summary>
	/// Client representation of a size (width + height).
	/// </summary>
	public struct LTSize : IEquatable<LTSize>
	{
		public double Width { get; set; }

		public double Height { get; set; }

		public LTSize(double width, double height)
		{
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return string.Format("W:{0:N1}-H:{1:N1}", Width, Height);
		}

		public bool Equals(LTSize other)
		{
			return Width.Equal(other.Width) && Height.Equal(other.Height);
		}

		public override bool Equals(object other)
		{
			if (!(other is LTSize s))
				return false;
			return Equals(s);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Width.GetHashCode();
				hashCode = (hashCode * 397) ^ Height.GetHashCode();
				return hashCode;
			}
		}
	}
}