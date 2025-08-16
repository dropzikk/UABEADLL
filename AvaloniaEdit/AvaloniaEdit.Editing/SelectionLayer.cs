using System;
using Avalonia.Media;
using AvaloniaEdit.Rendering;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Editing;

internal sealed class SelectionLayer : Layer
{
	private readonly TextArea _textArea;

	public SelectionLayer(TextArea textArea)
		: base(textArea.TextView, KnownLayer.Selection)
	{
		base.IsHitTestVisible = false;
		_textArea = textArea;
		WeakEventManagerBase<TextViewWeakEventManager.VisualLinesChanged, TextView, EventHandler, EventArgs>.AddHandler(TextView, ReceiveWeakEvent);
		WeakEventManagerBase<TextViewWeakEventManager.ScrollOffsetChanged, TextView, EventHandler, EventArgs>.AddHandler(TextView, ReceiveWeakEvent);
	}

	private void ReceiveWeakEvent(object sender, EventArgs e)
	{
		InvalidateVisual();
	}

	public override void Render(DrawingContext drawingContext)
	{
		base.Render(drawingContext);
		Pen selectionBorder = _textArea.SelectionBorder;
		BackgroundGeometryBuilder backgroundGeometryBuilder = new BackgroundGeometryBuilder
		{
			AlignToWholePixels = true,
			BorderThickness = (selectionBorder?.Thickness ?? 0.0),
			ExtendToFullWidthAtLineEnd = _textArea.Selection.EnableVirtualSpace,
			CornerRadius = _textArea.SelectionCornerRadius
		};
		foreach (SelectionSegment segment in _textArea.Selection.Segments)
		{
			backgroundGeometryBuilder.AddSegment(TextView, segment);
		}
		Geometry geometry = backgroundGeometryBuilder.CreateGeometry();
		if (geometry != null)
		{
			drawingContext.DrawGeometry(_textArea.SelectionBrush, selectionBorder, geometry);
		}
	}
}
