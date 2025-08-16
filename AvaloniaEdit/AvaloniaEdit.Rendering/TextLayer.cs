using System.Collections.Generic;
using Avalonia;

namespace AvaloniaEdit.Rendering;

internal sealed class TextLayer : Layer
{
	internal int Index;

	private readonly List<VisualLineDrawingVisual> _visuals = new List<VisualLineDrawingVisual>();

	public TextLayer(TextView textView)
		: base(textView, KnownLayer.Text)
	{
	}

	internal void SetVisualLines(ICollection<VisualLine> visualLines)
	{
		foreach (VisualLineDrawingVisual visual in _visuals)
		{
			if (visual.VisualLine.IsDisposed)
			{
				base.VisualChildren.Remove(visual);
			}
		}
		_visuals.Clear();
		foreach (VisualLine visualLine in visualLines)
		{
			VisualLineDrawingVisual visualLineDrawingVisual = visualLine.Render();
			if (!visualLineDrawingVisual.IsAdded)
			{
				base.VisualChildren.Add(visualLineDrawingVisual);
				visualLineDrawingVisual.IsAdded = true;
			}
			_visuals.Add(visualLineDrawingVisual);
		}
		InvalidateArrange();
	}

	protected override void ArrangeCore(Rect finalRect)
	{
		base.ArrangeCore(finalRect);
		TextView.ArrangeTextLayer(_visuals);
	}
}
