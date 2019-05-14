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

		public static readonly FontFamily FONT_FAMILY = new FontFamily("Assets/paltn.ttf#Palatino-Roman");
		public const int FONT_SIZE = 10;
		public const int LINE_HEAD_INDENT = 10;
		public const float LINE_SPACING = 1.08f;
		private static readonly Color FONT_FOREGROUND_COLOR = Color.FromArgb(255, 59, 52, 26);
		public static readonly SolidColorBrush FONT_FOREGROUND = new SolidColorBrush(FONT_FOREGROUND_COLOR);


		public MainPage()
		{
			this.InitializeComponent();

			CreateTextBlockLimitedMeasureWidth();
			CreateTextBlockLimitedMeasureWidth();
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

			m_textBlock.Margin = new Thickness(200);

			AttrString content = new AttrString(
				"Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.",
				AttrString.DefaultCitationFont);
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

			Debug.WriteLine("LimitedMeasureWidth: Size before adding text to scene: {0} - time to measure:{2}ms; after adding to scene: {1} - time to measure:{3}ms", size, size2, time, time2);
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

			m_textBlock2.Margin = new Thickness(400);
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
	}

	public static class TextUtils
	{
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
