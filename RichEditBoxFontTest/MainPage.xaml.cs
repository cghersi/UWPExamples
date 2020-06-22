using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RichEditBoxFontTest
{
	public sealed partial class MainPage : Page
	{
		#region Members
		SolidColorBrush m_selectedButtonColor = new SolidColorBrush(Colors.CornflowerBlue);
		private SolidColorBrush[] m_fontColors = new SolidColorBrush[]
		{
			new SolidColorBrush(Colors.Black),
			new SolidColorBrush(Colors.Red),
			new SolidColorBrush(Colors.Green),
			new SolidColorBrush(Colors.DodgerBlue),
			new SolidColorBrush(Colors.Aquamarine),
			new SolidColorBrush(Colors.Yellow),
		};
		private readonly float[] m_fontSizes = new float[] { 9, 10, 11, 12, 13, 14, 18, 24, 36, 48, 64 };
		private float m_defaultFontSize = 14;
		private RichEditBox MyRichEditBox;
		#endregion Members

		/// <summary>
		///  Constructor
		/// </summary>
		public MainPage()
		{
			this.InitializeComponent();

			// Add font toolbar
			AddToolbar();

			// Add RichEditBox
			AddRichEditBox();

			// Add demo video
			AddVideo();
			
			// Add Comment TextBlock
			AddComment();
		}

		private void AddToolbar()
		{
			InitFontSizeCombo();
			InitColorCombo();
		}
		private void AddRichEditBox()
		{
			StackPanel liveRichEditBox = new StackPanel()
			{
				Orientation = Orientation.Vertical,
				Margin = new Thickness(0, 0, 50, 0),
			};
			TextBlock title = new TextBlock()
			{
				FontSize = 18,
				FontWeight = new FontWeight() { Weight = 700 },
				Text = "Live RichEditBox"
			};
			liveRichEditBox.Children.Add(title);

			// Create RichEditBox
			MyRichEditBox = new RichEditBox
			{
				FontFamily = new FontFamily("Times New Roman"),
				Padding = new Thickness(0),
				TextWrapping = TextWrapping.Wrap,
				Foreground = new SolidColorBrush(Colors.Black),
				BorderThickness = new Thickness(1),
				AcceptsReturn = true,
				AllowFocusWhenDisabled = false,
				SelectionFlyout = null,
				ContextFlyout = null,
				Width = 400,
				Height = 300,
				Style = Resources["LTRichEditBox"] as Style,
			};
			ScrollViewer.SetVerticalScrollBarVisibility(MyRichEditBox, ScrollBarVisibility.Hidden);
			ScrollViewer.SetHorizontalScrollBarVisibility(MyRichEditBox, ScrollBarVisibility.Hidden);

			// Add handlers
			MyRichEditBox.SelectionChanged += MyRichEditBoxOnSelectionChanged;

			// Set RTF text
			string t = "{\\rtf1\\fbidis\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang2057{\\fonttbl{\\f0\\fswiss\\fprq2\\fcharset0 Segoe UI;}{\\f1\\fnil\\fcharset0 Segoe UI;}}{\\colortbl ;\\red0\\green0\\blue0 ;\\red0\\green128\\blue0 ;\\red255\\green0\\blue0 ;}{\\*\\generator Riched20 10.0.18362}\\viewkind4\\uc1\\pard\\nowidctlpar\\tx720\\cf1\\f0\\fs21 This is normal_10_black. \\par\\b\\fs24 This is Bold_12_Black\\par\\cf2\\b\\fs36 This normal_18_Green\\par\\cf3\\b\\fs72 This Bold_36_Red}";
			t = AdjustRtfFontSizes(t, true);
			MyRichEditBox.Document.SetText(TextSetOptions.FormatRtf, t);
			liveRichEditBox.Children.Add(MyRichEditBox);

			// Attach RichEditBox to the MainPanel
			ContentSP.Children.Add(liveRichEditBox);
		}

		private void AddVideo()
		{
			StackPanel demoVideo = new StackPanel()
			{
				Orientation = Orientation.Vertical
			};
			TextBlock title = new TextBlock()
			{
				FontSize = 18,
				FontWeight = new FontWeight() {Weight = 700},
				Text = "Demo video"
			};
			demoVideo.Children.Add(title);

			MediaElement me = new MediaElement()
			{
				Source = new Uri("ms-appx:///Assets/RichEditBoxFontTest.mp4"),
				AreTransportControlsEnabled = true,
				Width = 400,
				AutoPlay = false
			};
			demoVideo.Children.Add(me);

			ContentSP.Children.Add(demoVideo);
		}
		private void AddComment()
		{
			TextBlock commentTb = new TextBlock()
			{
				FontSize = 18,
				Text = "*** Steps to reproduce the issue ***\n" +
							 "1) Go to the end of the text (after 'Bold_36_Red')\n" +
							 "2) Add two new lines (i.e. hit enter button twice)\n" +
							 "3) Use the up-arrow key to move to the first new line\n" +
							 "4) Type some text: it should be bold 36 red, that is correct\n" +
							 "5) Now move with the down-arrow key to the second new line previously created\n" +
							 "6) Type some text: the font has been reset to normal 14 black, that is wrong.",
				HorizontalAlignment = HorizontalAlignment.Center,
				Margin = new Thickness(0, 50, 0, 0)
			};

			// Attach CommentTb to the MainPanel
			MainPanel.Children.Add(commentTb);
		}


		#region Toolbar methods
		private void InitFontSizeCombo()
		{
			foreach (float fontSize in m_fontSizes)
			{
				FontSizeCombo.Items?.Add(fontSize);
			}
			FontSizeCombo.SelectedItem = m_defaultFontSize;

			FontSizeCombo.SelectionChanged += FontSizeCombo_OnSelectionChanged;
		}

		private void InitColorCombo()
		{
			foreach (SolidColorBrush color in m_fontColors)
			{
				Rectangle r = new Rectangle()
				{
					Width = 20,
					Height = 20,
					Fill = color
				};
				ComboBoxItem cbi = new ComboBoxItem()
				{
					Content = r,
					Tag = color
				};
				FontColorCombo.Items?.Add(cbi);
			}

			FontColorCombo.SelectedIndex = 0;
			FontColorCombo.SelectionChanged += FontColorCombo_OnSelectionChanged;
		}

		private void FontSizeCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox cbx && cbx.SelectedItem is float size)
			{
				ITextSelection selectedText = MyRichEditBox.Document.Selection;
				if (selectedText != null)
				{
					ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
					if (size > 0)
					{
						//charFormatting.Size = size;
						charFormatting.Size = FontSizeResizer(size, true);
					}
				}
			}
		}

		private void FontColorCombo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (sender is ComboBox cbx && cbx.SelectedItem is ComboBoxItem cbi && cbi.Tag is SolidColorBrush color)
			{
				ITextSelection selectedText = MyRichEditBox.Document.Selection;
				if (selectedText != null)
				{
					ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
					charFormatting.ForegroundColor = color.Color;
				}
			}
		}

		private void BoldButton_Click(object sender, RoutedEventArgs e)
		{
			ITextSelection selectedText = MyRichEditBox.Document.Selection;
			if (selectedText != null)
			{
				ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
				charFormatting.Bold = FormatEffect.Toggle;
				selectedText.CharacterFormat = charFormatting;
				BoldButton.Foreground = m_selectedButtonColor;
			}
		}

		private void ItalicButton_Click(object sender, RoutedEventArgs e)
		{
			ITextSelection selectedText = MyRichEditBox.Document.Selection;
			if (selectedText != null)
			{
				ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
				charFormatting.Italic = FormatEffect.Toggle;
				selectedText.CharacterFormat = charFormatting;
				ItalicButton.Foreground = m_selectedButtonColor;
			}
		}

		private void UnderlineButton_Click(object sender, RoutedEventArgs e)
		{
			ITextSelection selectedText = MyRichEditBox.Document.Selection;
			if (selectedText != null)
			{
				ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
				if (charFormatting.Underline == UnderlineType.None)
				{
					charFormatting.Underline = UnderlineType.Single;
					UnderlineButton.Foreground = m_selectedButtonColor;
				}
				else
				{
					charFormatting.Underline = UnderlineType.None;
				}
				selectedText.CharacterFormat = charFormatting;
			}
		}

		private void StrikeThroughButton_Click(object sender, RoutedEventArgs e)
		{
			ITextSelection selectedText = MyRichEditBox.Document.Selection;
			if (selectedText != null)
			{
				ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
				charFormatting.Strikethrough = FormatEffect.Toggle;
				selectedText.CharacterFormat = charFormatting;
				StrikeThroughButton.Foreground = m_selectedButtonColor;
			}
		}

		private void MyRichEditBoxOnSelectionChanged(object sender, RoutedEventArgs e)
		{
			if (sender is RichEditBox reb)
			{
				ITextSelection selectedText = MyRichEditBox?.Document.Selection;
				if (selectedText != null)
				{
					SetHighLighted(BoldButton, selectedText.CharacterFormat.Bold == FormatEffect.On);
					SetHighLighted(ItalicButton, selectedText.CharacterFormat.Italic == FormatEffect.On);
					SetHighLighted(UnderlineButton, selectedText.CharacterFormat.Underline != UnderlineType.None);
					SetHighLighted(StrikeThroughButton, selectedText.CharacterFormat.Strikethrough == FormatEffect.On);

					if (FontSizeCombo.Items != null)
					{
						float originalSelectedFontSize = selectedText.CharacterFormat.Size;
						float realSelectedFontSize = FontSizeResizer(originalSelectedFontSize, false);

						Debug.WriteLine("selectedText.CharacterFormat.Size: " + realSelectedFontSize);

						if (realSelectedFontSize > 0)
						{
							float fontSize = FontSizeCombo.Items.Contains(realSelectedFontSize) ? realSelectedFontSize : (float)MyRichEditBox.FontSize;
							FontSizeCombo.SelectedItem = fontSize;
						}
						else
						{
							FontSizeCombo.SelectedIndex = -1;
						}
					}

					Color selectedColor = selectedText.CharacterFormat.ForegroundColor;
					IEnumerable<object> query = FontColorCombo.Items?.Where(item =>
					{
						if (item is ComboBoxItem cbi && cbi.Tag is SolidColorBrush col)
						{

							bool found = (
								col.Color.A == selectedColor.A &&
								col.Color.R == selectedColor.R &&
								col.Color.G == selectedColor.G &&
								col.Color.B == selectedColor.B);
							return found;
						}

						return false;
					});
					if (query?.FirstOrDefault() != null)
					{
						ComboBoxItem selectedColorItem = (ComboBoxItem)(query?.FirstOrDefault() ?? FontColorCombo.Items?.First());
						FontColorCombo.SelectedItem = selectedColorItem;
					}
					else
					{
						FontColorCombo.SelectedIndex = -1;
					}

				}
			}
		}

		private void SetHighLighted(Button bt, bool condition)
		{
			bt.Foreground = condition ? m_selectedButtonColor : new SolidColorBrush(Colors.Black);
		}

		private string AdjustRtfFontSizes(string originalRtf, bool isInput)
		{
			string result = Regex.Replace(originalRtf, @"\\fs(\d+)", (match) => FontSizeMatchResizer(match, isInput), RegexOptions.Multiline);
			return result;
		}

		const double FACTOR = 3f / 4f;
		private string FontSizeMatchResizer(Match match, bool isInput)
		{
			string newSizeTag = string.Empty;
			if (match?.Groups?.Count > 1)
			{
				newSizeTag = match.Groups[0].Value; // \fs24
				double originalSize = Convert.ToDouble(match.Groups[1].Value); // 24
				float newIntSize = FontSizeResizer(originalSize, isInput);
				newSizeTag = newSizeTag.Replace(originalSize.ToString(CultureInfo.InvariantCulture), newIntSize.ToString(CultureInfo.InvariantCulture));
			}

			return newSizeTag;
		}

		private float FontSizeResizer(double value, bool isInput)
		{
			float newIntSize;
			if (isInput)
			{
				newIntSize = (int)Math.Ceiling(value * FACTOR);
			}
			else
			{
				newIntSize = (int)Math.Floor(value / FACTOR);
			}

			return newIntSize;
		}

		#endregion Toolbar methods

	}
}
