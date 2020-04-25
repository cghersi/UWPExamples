namespace FoxitTestBench
{
	public struct LTPDFPageInfo
	{
		public static readonly LTPDFPageInfo EMPTY = new LTPDFPageInfo()
		{
			Rect = Encoders.RECT_ZERO,
			CropRect = Encoders.RECT_ZERO,
			Rotation = -1
		};

		public LTRect Rect { get; set; }
		public LTRect CropRect { get; set; }
		public int Rotation { get; set; }
	}
}
