using System;
using System.Collections.Generic;
using Avalonia.Media.TextFormatting;
using AvaloniaEdit.Document;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Rendering;

public class VisualLineText : VisualLineElement
{
	public VisualLine ParentVisualLine { get; }

	public override bool CanSplit => true;

	public VisualLineText(VisualLine parentVisualLine, int length)
		: base(length, length)
	{
		ParentVisualLine = parentVisualLine ?? throw new ArgumentNullException("parentVisualLine");
	}

	protected virtual VisualLineText CreateInstance(int length)
	{
		return new VisualLineText(ParentVisualLine, length);
	}

	public override TextRun CreateTextRun(int startVisualColumn, ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		int num = startVisualColumn - base.VisualColumn;
		int offset = context.VisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset + num;
		StringSegment text = context.GetText(offset, base.DocumentLength - num);
		return new TextCharacters(text.Text.AsMemory().Slice(text.Offset, text.Count), base.TextRunProperties);
	}

	public override bool IsWhitespace(int visualColumn)
	{
		int offset = visualColumn - base.VisualColumn + ParentVisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset;
		return char.IsWhiteSpace(ParentVisualLine.Document.GetCharAt(offset));
	}

	public override ReadOnlyMemory<char> GetPrecedingText(int visualColumnLimit, ITextRunConstructionContext context)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}
		int length = visualColumnLimit - base.VisualColumn;
		StringSegment text = context.GetText(context.VisualLine.FirstDocumentLine.Offset + base.RelativeTextOffset, length);
		return text.Text.AsMemory().Slice(text.Offset, text.Count);
	}

	public override void Split(int splitVisualColumn, IList<VisualLineElement> elements, int elementIndex)
	{
		if (splitVisualColumn <= base.VisualColumn || splitVisualColumn >= base.VisualColumn + base.VisualLength)
		{
			throw new ArgumentOutOfRangeException("splitVisualColumn", splitVisualColumn, "Value must be between " + (base.VisualColumn + 1) + " and " + (base.VisualColumn + base.VisualLength - 1));
		}
		if (elements == null)
		{
			throw new ArgumentNullException("elements");
		}
		if (elements[elementIndex] != this)
		{
			throw new ArgumentException("Invalid elementIndex - couldn't find this element at the index");
		}
		int num = splitVisualColumn - base.VisualColumn;
		VisualLineText visualLineText = CreateInstance(base.DocumentLength - num);
		SplitHelper(this, visualLineText, splitVisualColumn, num + base.RelativeTextOffset);
		elements.Insert(elementIndex + 1, visualLineText);
	}

	public override int GetRelativeOffset(int visualColumn)
	{
		return base.RelativeTextOffset + visualColumn - base.VisualColumn;
	}

	public override int GetVisualColumn(int relativeTextOffset)
	{
		return base.VisualColumn + relativeTextOffset - base.RelativeTextOffset;
	}

	public override int GetNextCaretPosition(int visualColumn, AvaloniaEdit.Document.LogicalDirection direction, CaretPositioningMode mode)
	{
		int num = ParentVisualLine.StartOffset + base.RelativeTextOffset;
		int nextCaretPosition = TextUtilities.GetNextCaretPosition(ParentVisualLine.Document, num + visualColumn - base.VisualColumn, direction, mode);
		if (nextCaretPosition < num || nextCaretPosition > num + base.DocumentLength)
		{
			return -1;
		}
		return base.VisualColumn + nextCaretPosition - num;
	}
}
