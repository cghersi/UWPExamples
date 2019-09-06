using System;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas.Geometry;
// ReSharper disable InconsistentNaming

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


		private readonly InkCanvas m_mainInkCanvas = new InkCanvas();


		public MainPage()
		{
			InitializeComponent();
			m_inkCanvas.IsHitTestVisible = false;
			m_inkCanvas.Width = Workspace.Width;
			m_inkCanvas.Height = Workspace.Height;
			Workspace.Children.Add(m_inkCanvas);
			m_inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
			m_inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;

			m_mainInkCanvas.IsHitTestVisible = false;
			m_mainInkCanvas.Width = LTInkMainManager.Width;
			m_mainInkCanvas.Height = LTInkMainManager.Height;
			LTInkMainManager.Children.Add(m_mainInkCanvas);
			//m_mainInkCanvas.Width = Desktop.Width;
			//m_mainInkCanvas.Height = Desktop.Height;
			//Desktop.Children.Add(m_mainInkCanvas);
			m_mainInkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Mouse;
			m_mainInkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;

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

		private static void ClipAreaWithHoles(FrameworkElement target, params FrameworkElement[] excluded)
		{
			Visual visual = ElementCompositionPreview.GetElementVisual(target);
			Compositor comp = visual.Compositor;
			CanvasGeometry clipGeometry =
				CanvasGeometry.CreateRectangle(null, 0, 0, (float) target.Width, (float) target.Height);

			foreach (FrameworkElement excl in excluded)
			{
				Rect rect = new Rect(0, 0, (float)excl.ActualWidth, (float)excl.ActualHeight);
				Rect forGeom = excl.ConvertSafeSlow(rect, target);
				CanvasGeometry excludedGeom = CanvasGeometry.CreateRectangle(null, forGeom);
				clipGeometry = clipGeometry.CombineWith(excludedGeom, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
			}
			visual.Clip = comp.CreateGeometricClip(clipGeometry);
		}

		private void ExpandBtn_OnClick(object _1, RoutedEventArgs _2)
		{
			ExpandClipArea();
		}

		private void ExpandBtnWithHoles_OnClick(object _1, RoutedEventArgs _2)
		{
			ClipAreaWithHoles(m_inkCanvas, ClickBtn, ClickBtn2, PanelBtn);
			ClipAreaWithHoles(m_mainInkCanvas, TitleBarCloseBtn);
			//ClipAreaWithHoles(m_mainInkCanvas, TitleBarPanel);
			//ClipAreaWithHoles(m_mainInkCanvas, LTMultiDocContainerView);
			//ClipAreaWithHoles(m_mainInkCanvas, MultiDocView);
		}

		private void PanelBtn_OnClick(object _1, PointerRoutedEventArgs _2)
		{
			m_clickCount++;
			ShowClickCount();
		}

		private void ClickBtn_OnClick(object _1, RoutedEventArgs _2)
		{
			m_clickCount++;
			ShowClickCount();
		}

		private void InkBelowBtn_OnClick(object _1, RoutedEventArgs _2)
		{
			Canvas.SetZIndex(Workspace, 1);
		}

		private void InkAboveBtn_OnClick(object _1, RoutedEventArgs _2)
		{
			Canvas.SetZIndex(Workspace, 20);
		}

		private void LotOfHolesBtn_OnClick(object _1, RoutedEventArgs _2)
		{
			Stopwatch sw = Stopwatch.StartNew();
			Visual visual = ElementCompositionPreview.GetElementVisual(m_inkCanvas);
			Compositor comp = visual.Compositor;
			CanvasGeometry clipGeometry =
				CanvasGeometry.CreateRectangle(null, 0, 0, (float)m_inkCanvas.Width, (float)m_inkCanvas.Height);

			sw.Stop();
			double time1 = sw.Elapsed.TotalMilliseconds;
			sw.Restart();

			int numOfHoles = int.Parse(HolesNum.Text);
			for (int i = 0; i < numOfHoles; i++)
			{ 
				CanvasGeometry excludedGeom = CanvasGeometry.CreateRectangle(null,
					i, i * 2, i * 2 - (i / 3), i * 5);
				clipGeometry = clipGeometry.CombineWith(excludedGeom, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
			}

			sw.Stop();
			double time2 = sw.Elapsed.TotalMilliseconds;
			sw.Restart();

			visual.Clip = comp.CreateGeometricClip(clipGeometry);

			sw.Stop();
			double time3 = sw.Elapsed.TotalMilliseconds;
			TimeResults.Text = string.Format("First: {0}ms; Exclusions: {1}ms; geomClip: {2}ms", time1, time2, time3);
		}

		private void TitleBarCloseBtn_OnClick(object _1, RoutedEventArgs _2)
		{
			m_clickCount++;
			ShowClickCount();
		}

		private void TitleBarPanel_OnPointerPressed(object _1, RoutedEventArgs _2)
		{
			m_clickCount++;
			ShowClickCount();
		}

		private void LTMultiDocContainerView_OnPointerPressed(object _1, RoutedEventArgs _2)
		{
			m_clickCount++;
			ShowClickCount();
		}

		private void MultiDocView_OnPointerPressed(object _1, RoutedEventArgs _2)
		{
			m_clickCount++;
			ShowClickCount();
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

		public static Rect ConvertSafeSlow(this UIElement fromView, Rect input, UIElement toView)
		{
			if ((toView == null) || (fromView == null))
				return input;
			GeneralTransform transform = fromView.TransformToVisual(toView);
			Rect res = transform.TransformBounds(input);
			return res;
		}
	}
}
