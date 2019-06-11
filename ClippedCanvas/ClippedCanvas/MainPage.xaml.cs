using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Microsoft.Graphics.Canvas.Geometry;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ClippedCanvas
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage
	{
		private readonly InkCanvas m_inkCanvas = new InkCanvas();
		private int m_strokeCount = 0;
		private int m_clickCount = 0;

		private float m_radius = 0;

		public MainPage()
		{
			InitializeComponent();
			m_inkCanvas.IsHitTestVisible = false;
			m_inkCanvas.Width = Workspace.Width;
			m_inkCanvas.Height = Workspace.Height;
			Workspace.Children.Add(m_inkCanvas);
			//Workspace.Children.Insert(0, m_inkCanvas);
			m_inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
			m_inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;

			Explanation.Text =
				"1. The InkCanvas is the area in AntiqueWhite. But it has a circular Clip, therefore you can draw ink strokes only in a circle near the button 'Click'." +
				Environment.NewLine +
				"2. Every time you click one of the buttons, and the click is actually accepted, the counter on the top ('Clicked XXX times') increases by one. " + Environment.NewLine +
				"Try e.g. clicking the 'CanClick?' button." +
				Environment.NewLine +
				"3. At The beginning, try to draw some strokes near the 'Click' button. You will see the strokes show only in the circular clip area (expected). " + Environment.NewLine +
				"You should be able to click the 'CanClick?' button, but not the 'Click' button, since it is inside the clip area." +
				Environment.NewLine +
				"However, you should also be able to click the 'ClickOutside' button, instead you can't (unexpected)." +
				Environment.NewLine;

			ExpandClipArea();

			ShowStrokeCount();
			ShowClickCount();
		}

		private void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
		{
			m_strokeCount += args.Strokes.Count;
			ShowStrokeCount();
			//sender.StrokeContainer.Clear();
		}

		private void ShowStrokeCount()
		{
			StrokeCounter.Text = string.Format("Counted {0} Strokes", m_strokeCount);
		}
		private void ShowClickCount()
		{
			ClickedCounter.Text = string.Format("Clicked {0} times", m_clickCount);
		}

		private void ExpandClipArea()
		{
			m_radius += 50;
			Visual visual = ElementCompositionPreview.GetElementVisual(m_inkCanvas);
			Compositor comp = visual.Compositor;
			CanvasGeometry clipGeometry = CanvasGeometry.CreateCircle(null, new Vector2(200, 200), m_radius);
			visual.Clip = comp.CreateGeometricClip(clipGeometry);
		}

		private void ExpandBtn_OnClick(object sender, RoutedEventArgs e)
		{
			ExpandClipArea();
		}

		private void ClickBtn_OnClick(object sender, RoutedEventArgs e)
		{
			m_clickCount++;
			ShowClickCount();
		}

		private void InkBelowBtn_OnClick(object sender, RoutedEventArgs e)
		{
			Canvas.SetZIndex(Workspace, 1);
		}

		private void InkAboveBtn_OnClick(object sender, RoutedEventArgs e)
		{
			Canvas.SetZIndex(Workspace, 20);
		}
	}

	public static class Utils
	{
		public static CompositionGeometricClip CreateGeometricClip(
			this Compositor compositor,
			CanvasGeometry geometry)
		{
			// Create the CompositionPath
			CompositionPath path = new CompositionPath(geometry);
			// Create the CompositionPathGeometry
			CompositionPathGeometry pathGeometry = compositor.CreatePathGeometry(path);
			// Create the CompositionGeometricClip
			return compositor.CreateGeometricClip(pathGeometry);
		}
	}
}
