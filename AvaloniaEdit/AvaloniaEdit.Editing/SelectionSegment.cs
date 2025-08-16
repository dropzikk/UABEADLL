using System;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Editing;

public class SelectionSegment : ISegment
{
	public int StartOffset { get; }

	public int EndOffset { get; }

	public int StartVisualColumn { get; }

	public int EndVisualColumn { get; }

	int ISegment.Offset => StartOffset;

	public int Length => EndOffset - StartOffset;

	public SelectionSegment(int startOffset, int endOffset)
	{
		StartOffset = Math.Min(startOffset, endOffset);
		EndOffset = Math.Max(startOffset, endOffset);
		StartVisualColumn = (EndVisualColumn = -1);
	}

	public SelectionSegment(int startOffset, int startVisualColumn, int endOffset, int endVisualColumn)
	{
		if (startOffset < endOffset || (startOffset == endOffset && startVisualColumn <= endVisualColumn))
		{
			StartOffset = startOffset;
			StartVisualColumn = startVisualColumn;
			EndOffset = endOffset;
			EndVisualColumn = endVisualColumn;
		}
		else
		{
			StartOffset = endOffset;
			StartVisualColumn = endVisualColumn;
			EndOffset = startOffset;
			EndVisualColumn = startVisualColumn;
		}
	}

	public override string ToString()
	{
		return $"[SelectionSegment StartOffset={StartOffset}, EndOffset={EndOffset}, StartVC={StartVisualColumn}, EndVC={EndVisualColumn}]";
	}
}
