using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaEdit.Document;

namespace AvaloniaEdit.Editing;

public class TextSegmentReadOnlySectionProvider<T> : IReadOnlySectionProvider where T : TextSegment
{
	public TextSegmentCollection<T> Segments { get; }

	public TextSegmentReadOnlySectionProvider(TextDocument textDocument)
	{
		Segments = new TextSegmentCollection<T>(textDocument);
	}

	public TextSegmentReadOnlySectionProvider(TextSegmentCollection<T> segments)
	{
		Segments = segments ?? throw new ArgumentNullException("segments");
	}

	public virtual bool CanInsert(int offset)
	{
		return Segments.FindSegmentsContaining(offset).All((T segment) => segment.StartOffset >= offset || offset >= segment.EndOffset);
	}

	public virtual IEnumerable<ISegment> GetDeletableSegments(ISegment segment)
	{
		if (segment == null)
		{
			throw new ArgumentNullException("segment");
		}
		if (segment.Length == 0 && CanInsert(segment.Offset))
		{
			yield return segment;
			yield break;
		}
		int readonlyUntil = segment.Offset;
		foreach (T item in Segments.FindOverlappingSegments(segment))
		{
			int startOffset = item.StartOffset;
			int end = startOffset + item.Length;
			if (startOffset > readonlyUntil)
			{
				yield return new SimpleSegment(readonlyUntil, startOffset - readonlyUntil);
			}
			if (end > readonlyUntil)
			{
				readonlyUntil = end;
			}
		}
		int endOffset = segment.EndOffset;
		if (readonlyUntil < endOffset)
		{
			yield return new SimpleSegment(readonlyUntil, endOffset - readonlyUntil);
		}
	}
}
