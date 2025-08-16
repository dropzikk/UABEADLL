using System;
using AvaloniaEdit.Rendering;

namespace AvaloniaEdit.Editing;

internal sealed class SelectionColorizer : ColorizingTransformer
{
	private readonly TextArea _textArea;

	public SelectionColorizer(TextArea textArea)
	{
		_textArea = textArea ?? throw new ArgumentNullException("textArea");
	}

	protected override void Colorize(ITextRunConstructionContext context)
	{
		if (_textArea.SelectionForeground == null)
		{
			return;
		}
		int offset = context.VisualLine.FirstDocumentLine.Offset;
		int num = context.VisualLine.LastDocumentLine.Offset + context.VisualLine.LastDocumentLine.TotalLength;
		foreach (SelectionSegment segment in _textArea.Selection.Segments)
		{
			int startOffset = segment.StartOffset;
			int endOffset = segment.EndOffset;
			if (endOffset > offset && startOffset < num)
			{
				int visualStartColumn = ((startOffset >= offset) ? context.VisualLine.ValidateVisualColumn(segment.StartOffset, segment.StartVisualColumn, _textArea.Selection.EnableVirtualSpace) : 0);
				int visualEndColumn = ((endOffset <= num) ? context.VisualLine.ValidateVisualColumn(segment.EndOffset, segment.EndVisualColumn, _textArea.Selection.EnableVirtualSpace) : (_textArea.Selection.EnableVirtualSpace ? int.MaxValue : context.VisualLine.VisualLengthWithEndOfLineMarker));
				ChangeVisualElements(visualStartColumn, visualEndColumn, delegate(VisualLineElement element)
				{
					element.TextRunProperties.SetForegroundBrush(_textArea.SelectionForeground);
				});
			}
		}
	}
}
