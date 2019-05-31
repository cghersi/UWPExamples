using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RichText
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		public static readonly FontFamily FONT_FAMILY = new FontFamily("Assets/paltn.ttf#Palatino-Roman");
		public const int FONT_SIZE = 10;
		public const int LINE_HEAD_INDENT = 3;
		public const float LINE_SPACING = 1.08f;
		private static readonly Color FONT_FOREGROUND_COLOR = Color.FromArgb(255, 59, 52, 26);
		public static readonly SolidColorBrush FONT_FOREGROUND = new SolidColorBrush(FONT_FOREGROUND_COLOR);

		private readonly Dictionary<string, object> FONT = new Dictionary<string, object>
		{
			{ AttrString.FONT_FAMILY_KEY, FONT_FAMILY },
			{ AttrString.FONT_SIZE_KEY, FONT_SIZE },
			{ AttrString.LINE_HEAD_INDENT_KEY, LINE_HEAD_INDENT },
			{ AttrString.LINE_SPACING_KEY, LINE_SPACING },
			{ AttrString.FOREGROUND_COLOR_KEY, FONT_FOREGROUND }
		};

		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly RichTextBlock m_displayedText;

		public MainPage()
		{
			InitializeComponent();

			// I haven't found a way to set those properties in XAML:
			ScrollViewer.SetVerticalScrollBarVisibility(ContentInBox, ScrollBarVisibility.Hidden);
			ScrollViewer.SetHorizontalScrollBarVisibility(ContentInBox, ScrollBarVisibility.Hidden);
			SetEditBoxFormat(ContentInBox);

			//ScrollViewer.SetVerticalScrollBarVisibility(TestBox, ScrollBarVisibility.Hidden);
			//ScrollViewer.SetHorizontalScrollBarVisibility(TestBox, ScrollBarVisibility.Hidden);
			//SetEditBoxFormat(TestBox);

			//// set the format and content for comparison:
			//string comparisonContent =
			//	"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Suspendisse rhoncus pharetra euismod. " +
			//	Environment.NewLine +
			//	"Suspendisse potenti. In vel eros elit. In aliquam sit amet lorem sed iaculis. Nam sollicitudin volutpat.";
			//ScrollViewer.SetVerticalScrollBarVisibility(ContentInBox, ScrollBarVisibility.Hidden);
			//ScrollViewer.SetHorizontalScrollBarVisibility(ContentInBox, ScrollBarVisibility.Hidden);
			//SetEditBoxFormat(ContentInBox);
			//ContentInBox.Document.SetText(TextSetOptions.None, comparisonContent);

			//SetRichText(ContentInBlock, new AttrString(comparisonContent, FONT));

			//SetSimpleEditBoxFormat(ContentInSimpleBox);
			//ContentInSimpleBox.Text = comparisonContent;

			//// create the text block:
			//m_displayedText = new RichTextBlock
			//{
			//	MaxLines = 0, //Let it use as many lines as it wants
			//	TextWrapping = TextWrapping.Wrap,
			//	AllowFocusOnInteraction = false,
			//	IsHitTestVisible = false,
			//	Width = 80,
			//	Height = 30,
			//	Margin = new Thickness(100)
			//};

			//RichEditBox editText = new RichEditBox()
			//{
			//	Background = new SolidColorBrush(Colors.Aqua),
			//	BorderThickness = new Thickness(3),
			//	BorderBrush = new SolidColorBrush(Colors.Blue),
			//	Width = 80,
			//	Height = 30,
			//	Margin = new Thickness(300)
			//};
			//editText.Document.SetText(TextSetOptions.None, "test edit box");
			//ScrollViewer.SetVerticalScrollBarVisibility(editText, ScrollBarVisibility.Hidden);
			//ScrollViewer.SetHorizontalScrollBarVisibility(editText, ScrollBarVisibility.Hidden);

			//// set the content with the right properties:
			//AttrString content = new AttrString("Excerpt1 InkLink", FONT);
			//SetRichText(m_displayedText, content);

			//// add to the main panel:
			//MainPanel.Children.Add(m_displayedText);
			//MainPanel.Children.Add(editText);

			//// compute the text height: (this gives the wrong answer!!):
			//double textH = GetRichTextHeight(content, (float)m_displayedText.Width);
			//Console.WriteLine("text height: {0}", textH);
		}

		public static double GetRichTextHeight(AttrString text, float maxWidth)
		{
			if (text == null)
				return 0;

			CanvasDevice device = CanvasDevice.GetSharedDevice();
			double finalH = 0;
			foreach (AttributedToken textToken in text.Tokens)
			{
				CanvasTextFormat frmt = new CanvasTextFormat()
				{
					Direction = CanvasTextDirection.LeftToRightThenTopToBottom,
					FontFamily = textToken.Get(AttrString.FONT_FAMILY_KEY, FONT_FAMILY).Source,
					FontSize = textToken.Get(AttrString.FONT_SIZE_KEY, FONT_SIZE),
					WordWrapping = CanvasWordWrapping.Wrap
				};
				CanvasTextLayout layout = new CanvasTextLayout(device, textToken.Text, frmt, maxWidth, 0f);
				finalH += layout.LayoutBounds.Height;
			}

			return finalH;

			//return textBlock.Blocks.Sum(block => block.LineHeight);
		}

		private static void SetRichText(RichTextBlock label, AttrString str)
		{
			if ((str == null) || (label == null))
				return;
			label.Blocks.Clear();
			foreach (AttributedToken token in str.Tokens)
			{
				Brush foreground = token.Get(AttrString.FOREGROUND_COLOR_KEY, FONT_FOREGROUND);
				FontStyle style = token.Get(AttrString.FONT_STYLE_KEY, FontStyle.Normal);
				FontFamily fontFamily = token.Get(AttrString.FONT_FAMILY_KEY, FONT_FAMILY);
				float fontSize = token.Get(AttrString.FONT_SIZE_KEY, FONT_SIZE);
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

		private static void SetEditBoxFormat(RichEditBox editBox)
		{
			// set paragraph format:
			ITextParagraphFormat paragraphFrmt = editBox.Document.GetDefaultParagraphFormat();
			paragraphFrmt.Alignment = ParagraphAlignment.Left;
			paragraphFrmt.SetLineSpacing(LineSpacingRule.Exactly, FONT_SIZE * 72f / 96f * 1.08f);

			// adding this setting jeopardizes the computation of height for the text view in UWPUtils.GetRichTextHeight()
			paragraphFrmt.SetIndents(LINE_HEAD_INDENT * 72f / 96f, 0, 0);
			editBox.Document.SetDefaultParagraphFormat(paragraphFrmt);

			// set character format:
			ITextCharacterFormat charFrmt = editBox.Document.GetDefaultCharacterFormat();
			charFrmt.FontStyle = FontStyle.Normal;
			charFrmt.ForegroundColor = FONT_FOREGROUND_COLOR;
			charFrmt.Name = FONT_FAMILY.Source;
			charFrmt.Size = FONT_SIZE;
			editBox.Document.SetDefaultCharacterFormat(charFrmt);
		}

		private static void SetSimpleEditBoxFormat(TextBox editBox)
		{
			// set paragraph format:
			editBox.TextAlignment = TextAlignment.Left;
			editBox.Foreground = FONT_FOREGROUND;
			editBox.FontFamily = FONT_FAMILY;
			editBox.FontSize = FONT_SIZE;
			editBox.FontStyle = FontStyle.Normal;

			// no line spacing in simple text boxes...
		}

		private void TestBox_OnLayoutUpdated(object sender, object e)
		{
			//Debug.WriteLine("LayoutUpdated");
		}

		private void TestBox_OnSizeChanged(object sender, SizeChangedEventArgs e)
		{
			Debug.WriteLine("NewSize:{0}; PrevSize:{1}",  e.NewSize, e.PreviousSize);
			TextView.Height += 30;
		}
	}

	public class AttrString
	{
		public const string FONT_FAMILY_KEY = "Fam";
		public const string FONT_SIZE_KEY = "Size";
		public const string LINE_HEAD_INDENT_KEY = "LhI";
		public const string LINE_SPACING_KEY = "LSpace";
		public const string FOREGROUND_COLOR_KEY = "Color";
		public const string FONT_STYLE_KEY = "FSty";
		public const string ITALIC_KEY = "Ita";
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
