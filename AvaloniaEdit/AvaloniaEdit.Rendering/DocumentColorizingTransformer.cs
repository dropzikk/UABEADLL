using System;
using System.Linq;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Rendering;

public abstract class DocumentColorizingTransformer : ColorizingTransformer
{
	private DocumentLine _currentDocumentLine;

	private int _firstLineStart;

	private int _currentDocumentLineStartOffset;

	private int _currentDocumentLineEndOffset;

	protected ITextRunConstructionContext CurrentContext { get; private set; }

	protected override void Colorize(ITextRunConstructionContext context)
	{
		CurrentContext = context ?? throw new ArgumentNullException("context");
		_currentDocumentLine = context.VisualLine.FirstDocumentLine;
		_firstLineStart = (_currentDocumentLineStartOffset = _currentDocumentLine.Offset);
		_currentDocumentLineEndOffset = _currentDocumentLineStartOffset + _currentDocumentLine.Length;
		int num = _currentDocumentLineStartOffset + _currentDocumentLine.TotalLength;
		if (context.VisualLine.FirstDocumentLine == context.VisualLine.LastDocumentLine)
		{
			ColorizeLine(_currentDocumentLine);
		}
		else
		{
			ColorizeLine(_currentDocumentLine);
			VisualLineElement[] array = context.VisualLine.Elements.ToArray();
			foreach (VisualLineElement visualLineElement in array)
			{
				int num2 = _firstLineStart + visualLineElement.RelativeTextOffset;
				if (num2 >= num)
				{
					_currentDocumentLine = context.Document.GetLineByOffset(num2);
					_currentDocumentLineStartOffset = _currentDocumentLine.Offset;
					_currentDocumentLineEndOffset = _currentDocumentLineStartOffset + _currentDocumentLine.Length;
					num = _currentDocumentLineStartOffset + _currentDocumentLine.TotalLength;
					ColorizeLine(_currentDocumentLine);
				}
			}
		}
		_currentDocumentLine = null;
		CurrentContext = null;
	}

	protected abstract void ColorizeLine(DocumentLine line);

	protected void ChangeLinePart(int startOffset, int endOffset, Action<VisualLineElement> action)
	{
		if (startOffset < _currentDocumentLineStartOffset || startOffset > _currentDocumentLineEndOffset)
		{
			throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between " + _currentDocumentLineStartOffset + " and " + _currentDocumentLineEndOffset);
		}
		if (endOffset < startOffset || endOffset > _currentDocumentLineEndOffset)
		{
			throw new ArgumentOutOfRangeException("endOffset", endOffset, "Value must be between " + startOffset + " and " + _currentDocumentLineEndOffset);
		}
		VisualLine visualLine = CurrentContext.VisualLine;
		int visualColumn = visualLine.GetVisualColumn(startOffset - _firstLineStart);
		int visualColumn2 = visualLine.GetVisualColumn(endOffset - _firstLineStart);
		if (visualColumn < visualColumn2)
		{
			ChangeVisualElements(visualColumn, visualColumn2, action);
		}
	}
}
