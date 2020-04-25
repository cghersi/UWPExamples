//------------------------------------------------------------------------------
// (c) 2018 LiquidText Inc.
// This software is property of LiquidText Inc. Use or reproduction without permission is prohibited  
//------------------------------------------------------------------------------


namespace FoxitTestBench
{
	public class LTPositionInDoc
	{
		public int Page { get; set; }

		public bool GlyphBased { get;  set; }

		public int GlyphIndex { get; set; }

		public LTPoint? PointStable { get; set; }
	}
}