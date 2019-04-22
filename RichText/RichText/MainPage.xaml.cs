using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
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
		private readonly Dictionary<string, object> FONT = new Dictionary<string, object>
		{
			{ AttrString.FONT_FAMILY_KEY, FONT_FAMILY },
			{ AttrString.FONT_SIZE_KEY, FONT_SIZE },
			{ AttrString.LINE_HEAD_INDENT_KEY, 10 },
			{ AttrString.LINE_SPACING_KEY, 1.08 },
			{ AttrString.FOREGROUND_COLOR_KEY, new SolidColorBrush(Colors.Black) }
		};

		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private readonly RichTextBlock m_displayedText;

		public MainPage()
		{
			InitializeComponent();

			// create the text block:
			m_displayedText = new RichTextBlock
			{
				MaxLines = 0, //Let it use as many lines as it wants
				TextWrapping = TextWrapping.Wrap,
				AllowFocusOnInteraction = false,
				IsHitTestVisible = false,
				Width = 80,
				Height = 30,
				Margin = new Thickness(100)
			};

			// set the content with the right properties:
			AttrString content = new AttrString("Excerpt1 InkLink", FONT);
			SetRichText(m_displayedText, content);

			// add to the main panel:
			MainPanel.Children.Add(m_displayedText);

			// compute the text height: (this gives the wrong answer!!):
			double textH = GetRichTextHeight(content, (float)m_displayedText.Width);
			Console.WriteLine("text height: {0}", textH);
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
				Paragraph paragraph = new Paragraph()
				{
					TextAlignment = token.Get(AttrString.TEXT_ALIGN_KEY, TextAlignment.Left),
					//TextIndent = token.Get(AttrString.LINE_HEAD_INDENT_KEY, 0),
				};
				double fontSize = token.Get(AttrString.FONT_SIZE_KEY, FONT_SIZE);
				double lineSpacing = token.Get(AttrString.LINE_SPACING_KEY, 1.0);
				paragraph.LineHeight = fontSize * lineSpacing;
				paragraph.LineStackingStrategy = LineStackingStrategy.BlockLineHeight;
				Run run = new Run
				{
					Text = token.Text,
					FontFamily = token.Get(AttrString.FONT_FAMILY_KEY, FONT_FAMILY),
					FontSize = fontSize,
					Foreground = token.Get(AttrString.FOREGROUND_COLOR_KEY, new SolidColorBrush(Colors.Black)),
					FontStyle = token.Get(AttrString.ITALIC_KEY, false) ? 
						Windows.UI.Text.FontStyle.Italic : Windows.UI.Text.FontStyle.Normal
				};
				paragraph.Inlines.Add(run);
				label.Blocks.Add(paragraph);
			}
		}
	}

	public class AttrString
	{
		public const string FONT_FAMILY_KEY = "Fam";
		public const string FONT_SIZE_KEY = "Size";
		public const string LINE_HEAD_INDENT_KEY = "LhI";
		public const string LINE_SPACING_KEY = "LSpace";
		public const string FOREGROUND_COLOR_KEY = "Color";
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
