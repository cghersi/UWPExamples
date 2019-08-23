using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using DirectXPanels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InputManagerWithCoreIndependentInputSource
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : ILTExcerptDraggable
	{
		private readonly DrawingPanel m_chainPanel;
		private readonly LTGestureRecognizerEngine m_gestRecEngine = new LTGestureRecognizerEngine();

		private static readonly StorageFolder FOLDER = ApplicationData.Current.LocalFolder;

		private readonly List<string> m_logs = new List<string>();

		public MainPage()
		{
			InitializeComponent();

			// make sure the SwapChainPanel is on top of everything:
			m_chainPanel = new DrawingPanel();
			MainContainer.Children.Add(m_chainPanel);
			m_chainPanel.Width = MainContainer.Width;
			m_chainPanel.Height = MainContainer.Height;
			m_chainPanel.StartProcessingInput();
			m_gestRecEngine.AttachEvents(m_chainPanel);
			m_gestRecEngine.AddGestureRecognizer(new LTDragGestureRecognizer(this));

			CompositionTarget.Rendering += LTGestureRecognizerEngine.CompositionTarget_Rendering;

			m_logs.Add("End Loading");
		}

		public async Task<bool> StartExcerptGesture(LTTouch touch)
		{
			//Debug.WriteLine("Start moving item with touch: {0}", touch);

			// simulate some long lasting op...
			string filename = FileName(touch);
			try
			{
				StorageFile file = await FOLDER.CreateFileAsync(filename);
				await FileIO.WriteTextAsync(file, "Test " + touch);
			}
			catch (Exception e)
			{
				m_logs.Add(string.Format("Cannot save file {0} due to {1}", filename, e.Message));
				return false;
			}

			return true;
		}

		public bool ContinueExcerptGesture(LTTouch touch)
		{
			m_logs.Add(string.Format("Continuing moving item with touch: {0}", touch));
			MovingItem.Move(touch.X - touch.LastX, touch.Y - touch.LastY);
			return true;
		}

		public async void EndExcerptGesture(LTTouch touch)
		{
			m_logs.Add(string.Format("End of dragging gesture with touch: {0}", touch));

			// simulate some long lasting op...
			string filename = FileName(touch);
			try
			{
				StorageFile file = await FOLDER.GetFileAsync(filename);
				string content = await FileIO.ReadTextAsync(file);
				m_logs.Add(string.Format("Reading gesture with touch: {0} => {1}", touch, content));
			}
			catch (Exception e)
			{
				m_logs.Add(string.Format("Cannot read file {0} due to {1}", filename, e.Message));
			}
		}

		public void CancelExcerptGesture()
		{
			m_logs.Add("Cancelled dragging gesture.");
			MovingItem.Margin = new Thickness(200, 150, 0, 0);
		}

		private static string FileName(LTTouch touch)
		{
			return string.Format("file_{0}.txt", touch.TouchUID);
		}

		private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			string filename = "log.txt";
			StorageFile file;
			try
			{
				file = await FOLDER.CreateFileAsync(filename);
			}
			catch (Exception)
			{
				file = await FOLDER.GetFileAsync(filename);
			}

			StringBuilder sbResults = new StringBuilder();
			sbResults.AppendLine(LTGestureRecognizerEngine.ProfilerDown.PrintResults());
			sbResults.AppendLine(LTGestureRecognizerEngine.ProfilerMoved.PrintResults());
			sbResults.AppendLine(LTGestureRecognizerEngine.ProfilerUp.PrintResults());
			sbResults.AppendLine(LTGestureRecognizerEngine.ProfilerCapLost.PrintResults());
			sbResults.AppendLine(LTGestureRecognizerEngine.ProfilerRendering.PrintResults());
			Results.Text = "Log File in " + file.Path + Environment.NewLine + sbResults.ToString();

			StringBuilder sb = new StringBuilder();
			foreach (string log in m_logs)
			{
				sb.AppendLine(log);
			}
			m_logs.Clear();
			sb.AppendLine(sbResults.ToString());
			sb.AppendLine("");

			try
			{
				await FileIO.WriteTextAsync(file, sb.ToString());
			}
			catch (Exception ex)
			{
				m_logs.Add(string.Format("Cannot write on file due to {0}", ex.Message));
			}
		}
	}
}
