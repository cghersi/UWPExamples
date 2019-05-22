using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MeasureText
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		private RichTextBlock m_textBlock;
		private RichTextBlock m_textBlock2;
		private RichTextBlock m_textBlock3;

		public static readonly FontFamily FONT_FAMILY = new FontFamily("Assets/paltn.ttf#Palatino-Roman");
		public const int FONT_SIZE = 10;
		public const int LINE_HEAD_INDENT = 10;
		public const float LINE_SPACING = 1.08f;
		private static readonly Color FONT_FOREGROUND_COLOR = Color.FromArgb(255, 59, 52, 26);
		public static readonly SolidColorBrush FONT_FOREGROUND = new SolidColorBrush(FONT_FOREGROUND_COLOR);

		private static AttrString content = new AttrString(
			"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
			AttrString.DefaultCitationFont);

		public MainPage()
		{
			this.InitializeComponent();

			CreateTextBlockLimitedMeasureWidth();
			CreateTextBlockLimitedElementWidth();
			CreateTextAndMeasure();
		}

		private void CreateTextAndMeasure()
		{
			m_textBlock3 = new RichTextBlock()
			{
				FontFamily = FONT_FAMILY,
				FontSize = FONT_SIZE,
				TextWrapping = TextWrapping.Wrap,
				Foreground = FONT_FOREGROUND,
				MaxLines = 0, //Let it use as many lines as it wants
				AllowFocusOnInteraction = false,
				IsHitTestVisible = false,
				Width = 200,
				Height = 4000
			};

			m_textBlock3.Margin = new Thickness(100, 220, 0, 0);
			m_textBlock3.SetRichText(content);

			double h = m_textBlock3.GetElemHeight();
			Workspace.Children.Add(m_textBlock3);
		}

		private void CreateTextBlockLimitedMeasureWidth()
		{
			m_textBlock = new RichTextBlock()
			{
				FontFamily = FONT_FAMILY,
				FontSize = FONT_SIZE,
				TextWrapping = TextWrapping.Wrap,
				Foreground = FONT_FOREGROUND,
				MaxLines = 0, //Let it use as many lines as it wants
				AllowFocusOnInteraction = false,
				IsHitTestVisible = false
			};

			m_textBlock.Margin = new Thickness(20);


			m_textBlock.SetRichText(content);

			Stopwatch sw = Stopwatch.StartNew();
			m_textBlock.Measure(new Size(300, double.PositiveInfinity));
			Size size = m_textBlock.DesiredSize;
			sw.Stop();
			double time = sw.Elapsed.TotalMilliseconds;
			Workspace.Children.Add(m_textBlock);
			m_textBlock.InvalidateMeasure();

			sw.Restart();
			m_textBlock.Measure(new Size(300, double.PositiveInfinity));
			Size size2 = m_textBlock.DesiredSize;
			sw.Stop();
			double time2 = sw.Elapsed.TotalMilliseconds;

			sw.Restart();
			double h = content.GetRichTextHeight(300);
			sw.Stop();
			double time3 = sw.Elapsed.TotalMilliseconds;

			Debug.WriteLine("LimitedMeasureWidth: Size before adding text to scene: {0} - time to measure:{2}ms; after adding to scene: {1} - time to measure:{3}ms - Win2D H:{4} in {5}ms", 
				size, size2, time, time2, h, time3);
		}

		private void CreateTextBlockLimitedElementWidth()
		{
			m_textBlock2 = new RichTextBlock()
			{
				FontFamily = FONT_FAMILY,
				FontSize = FONT_SIZE,
				TextWrapping = TextWrapping.Wrap,
				Foreground = FONT_FOREGROUND,
				MaxLines = 0, //Let it use as many lines as it wants
				AllowFocusOnInteraction = false,
				IsHitTestVisible = false
			};

			m_textBlock2.Margin = new Thickness(40);
			m_textBlock2.Width = 300;

			AttrString content = new AttrString(
				"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
				AttrString.DefaultCitationFont);
			m_textBlock2.SetRichText(content);

			Stopwatch sw = Stopwatch.StartNew();
			m_textBlock2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Size size = m_textBlock2.DesiredSize;
			sw.Stop();
			double time = sw.Elapsed.TotalMilliseconds;
			Workspace.Children.Add(m_textBlock2);
			m_textBlock.InvalidateMeasure();

			sw.Restart();
			m_textBlock2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			Size size2 = m_textBlock2.DesiredSize;
			sw.Stop();
			double time2 = sw.Elapsed.TotalMilliseconds;

			Debug.WriteLine("LimitedElementWidth: Size before adding text to scene: {0} - time to measure:{2}ms; after adding to scene: {1} - time to measure:{3}ms", size, size2, time, time2);
		}

		private void Test_OnClick(object sender, RoutedEventArgs e)
		{
			Stopwatch sw = new Stopwatch();
			double totTime = 0;
			double maxTime = 0;
			double totTimeWin2D = 0;
			double maxTimeWin2D = 0;
			for (int i = 0; i < 1000; i++)
			{
				Size sizeBefore = m_textBlock2.DesiredSize;
				m_textBlock2.Width += 2;
				sw.Restart();
				Size sizeMiddle = m_textBlock2.DesiredSize;
				m_textBlock2.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
				Size sizeAfter = m_textBlock2.DesiredSize;
				sw.Stop();
				double time = sw.Elapsed.TotalMilliseconds;
				totTime += time;
				if (maxTime < time)
					maxTime = time;

				sw.Restart();
				double h = content.GetRichTextHeight(300);
				sw.Stop();
				double timeWin2D = sw.Elapsed.TotalMilliseconds;
				totTimeWin2D += timeWin2D;
				if (maxTimeWin2D < timeWin2D)
					maxTimeWin2D = timeWin2D;

				Debug.WriteLine("Test_OnClick: Size before changing width: {0}; Size after changing width: {1}; Size after measuring: {2} - time to measure:{3}ms - Win2D H:{4} in {5}ms", 
					sizeBefore, sizeMiddle, sizeAfter, time, h, totTimeWin2D);
			}

			Debug.WriteLine("Test_OnClick: tot time: {0}ms; max time:{1}ms; Win2D: {2}ms - max {3}ms", totTime / 1000, maxTime, totTimeWin2D / 1000, maxTimeWin2D);
		}
	}

	public static class TextUtils
	{
		/// <summary>
		/// Computes the height of the given element, when the width is fixed
		/// </summary>
		/// <param name="elem"></param>
		/// <param name="actualWidth">force the method to use the given width, rather than the element Width property</param>
		/// <returns></returns>
		public static double GetElemHeight(this FrameworkElement elem, double? actualWidth = null)
		{
			if (elem == null)
				return 0;
			double currentH = elem.Height;
			if (!double.IsNaN(currentH))
				elem.Height = double.NaN;
			double totalW = (actualWidth ?? elem.Width) + elem.Margin.Left + elem.Margin.Right;
			elem.Measure(new Size(totalW, double.PositiveInfinity));
			Size size = elem.DesiredSize;
			elem.Height = currentH;
			return size.Height - elem.Margin.Top - elem.Margin.Bottom;
		}

		public static void SetRichText(this RichTextBlock label, AttrString str)
		{
			if ((str == null) || (label == null))
				return;
			label.Blocks.Clear();
			foreach (AttributedToken token in str.Tokens)
			{
				Brush foreground = token.Get(AttrString.FOREGROUND_COLOR_KEY, new SolidColorBrush(Colors.Black));
				FontStyle style = token.Get(AttrString.FONT_STYLE_KEY, FontStyle.Normal);
				FontFamily fontFamily = token.Get(AttrString.FONT_FAMILY_KEY, MainPage.FONT_FAMILY);
				float fontSize = token.Get(AttrString.FONT_SIZE_KEY, MainPage.FONT_SIZE);
				float lineSpacing = token.Get(AttrString.LINE_SPACING_KEY, 1.0f);

				Paragraph paragraph = new Paragraph()
				{
					TextAlignment = token.Get(AttrString.TEXT_ALIGN_KEY, TextAlignment.Left),
					Foreground = foreground,
					FontStyle = style,
					FontSize = fontSize,
					FontFamily = fontFamily

					// adding this setting jeopardizes the computation of height for the text view in UWPUtils.GetRichTextHeight()
					//TextIndent = token.Get(AttrString.LINE_HEAD_INDENT_KEY, 0), TODO: check with Craig if this is still needed
				};

				paragraph.LineHeight = fontSize * lineSpacing;
				paragraph.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;

				Run run = new Run
				{
					Text = token.Text,
					FontSize = fontSize,
					Foreground = foreground,
					FontStyle = style,
					FontFamily = fontFamily
				};
				paragraph.Inlines.Add(run);
				label.Blocks.Add(paragraph);
			}
		}

		public static double GetRichTextHeight(this AttrString text, float maxWidth)
		{
			if (text == null)
				return 0;

			CanvasDevice device = CanvasDevice.GetSharedDevice();
			double finalH = 0;
			foreach (AttributedToken textToken in text.Tokens)
			{
				string content = textToken.Text;
				// if we have indentation, we need to add some spaces to fill up this indent value... 

				FontStyle style = textToken.Get(AttrString.FONT_STYLE_KEY, FontStyle.Normal);
				string fontFamily = textToken.Get(AttrString.FONT_FAMILY_KEY, MainPage.FONT_FAMILY).Source;
				float fontSize = textToken.Get(AttrString.FONT_SIZE_KEY, MainPage.FONT_SIZE);
				float lineSpacing = textToken.Get(AttrString.LINE_SPACING_KEY, 1.0f);
				int totChar = content.Length;
				CanvasTextFormat frmt = new CanvasTextFormat()
				{
					Direction = CanvasTextDirection.LeftToRightThenTopToBottom,
					FontFamily = fontFamily,
					FontSize = fontSize,
					FontStyle = style,
					WordWrapping = CanvasWordWrapping.Wrap
				};
				CanvasTextLayout layout = new CanvasTextLayout(device, content, frmt, maxWidth, 0f)
				{
					HorizontalAlignment = TranslateHAlignment(textToken.Get(AttrString.TEXT_ALIGN_KEY, TextAlignment.Left))
				};
				layout.SetFontFamily(0, totChar, fontFamily);
				layout.SetFontSize(0, totChar, fontSize);
				layout.SetFontStyle(0, totChar, style);
				layout.LineSpacing = lineSpacing;
				layout.LineSpacingMode = CanvasLineSpacingMode.Proportional;

				finalH += layout.LayoutBounds.Height;
			}

			return finalH;
		}

		private static CanvasHorizontalAlignment TranslateHAlignment(TextAlignment align)
		{
			switch (align)
			{
				case TextAlignment.Left: return CanvasHorizontalAlignment.Left;
				case TextAlignment.Center: return CanvasHorizontalAlignment.Center;
				case TextAlignment.Justify: return CanvasHorizontalAlignment.Justified;
				case TextAlignment.Right: return CanvasHorizontalAlignment.Right;
			}

			return CanvasHorizontalAlignment.Left;
		}
	}


public class AttrString
	{
		public const string FONT_FAMILY_KEY = "Fam";
		public const string FONT_SIZE_KEY = "FSize";
		public const string LINE_HEAD_INDENT_KEY = "LhI";
		public const string LINE_SPACING_KEY = "LSpace";
		public const string FOREGROUND_COLOR_KEY = "FColor";
		public const string FONT_STYLE_KEY = "FSty";
		public const string TEXT_ALIGN_KEY = "Align";
		public const string LINE_BREAK_MODE_KEY = "LineBreak";

		public static Dictionary<string, object> DefaultCitationFont { get; set; }
		public static Dictionary<string, object> DefaultFont { get; set; }

		public List<AttributedToken> Tokens { get; set; }

		public AttrString(string text, Dictionary<string, object> attributes)
		{
			Tokens = new List<AttributedToken>();
			Append(text, attributes);
		}

		public AttrString(AttrString copy)
		{
			if (copy?.Tokens == null)
				return;
			Tokens = new List<AttributedToken>(copy.Tokens);
		}

		public AttrString Append(string text, Dictionary<string, object> attributes)
		{
			Tokens.Add(new AttributedToken(text, attributes));
			return this;
		}

		public bool IsEmpty()
		{
			foreach (AttributedToken t in Tokens)
			{
				if (!string.IsNullOrEmpty(t.Text))
					return false;
			}

			return true;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (AttributedToken t in Tokens)
			{
				sb.Append(t.Text);
			}
			return sb.ToString();
		}
	}

	public class AttributedToken
	{
		public string Text { get; set; }

		public Dictionary<string, object> Attributes { get; set; }

		public AttributedToken(string text, Dictionary<string, object> attributes)
		{
			Text = text;
			Attributes = attributes;
		}

		public T Get<T>(string key, T defaultValue)
		{
			if (string.IsNullOrEmpty(key) || (Attributes == null))
				return defaultValue;
			if (Attributes.ContainsKey(key))
				return (T)Attributes[key];
			else
				return defaultValue;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
