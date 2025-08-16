using System;
using System.Globalization;
using AvaloniaEdit.Utils;

namespace AvaloniaEdit.Document;

public sealed class AnchorSegment : ISegment
{
	private readonly TextAnchor _start;

	private readonly TextAnchor _end;

	public int Offset => _start.Offset;

	public int Length => Math.Max(0, _end.Offset - _start.Offset);

	public int EndOffset => Math.Max(_start.Offset, _end.Offset);

	public AnchorSegment(TextAnchor start, TextAnchor end)
	{
		if (start == null)
		{
			throw new ArgumentNullException("start");
		}
		if (end == null)
		{
			throw new ArgumentNullException("end");
		}
		if (!start.SurviveDeletion)
		{
			throw new ArgumentException("Anchors for AnchorSegment must use SurviveDeletion", "start");
		}
		if (!end.SurviveDeletion)
		{
			throw new ArgumentException("Anchors for AnchorSegment must use SurviveDeletion", "end");
		}
		_start = start;
		_end = end;
	}

	public AnchorSegment(TextDocument document, ISegment segment)
		: this(document, ThrowUtil.CheckNotNull(segment, "segment").Offset, segment.Length)
	{
	}

	public AnchorSegment(TextDocument document, int offset, int length)
	{
		_start = document?.CreateAnchor(offset) ?? throw new ArgumentNullException("document");
		_start.SurviveDeletion = true;
		_start.MovementType = AnchorMovementType.AfterInsertion;
		_end = document.CreateAnchor(offset + length);
		_end.SurviveDeletion = true;
		_end.MovementType = AnchorMovementType.BeforeInsertion;
	}

	public override string ToString()
	{
		return "[Offset=" + Offset.ToString(CultureInfo.InvariantCulture) + ", EndOffset=" + EndOffset.ToString(CultureInfo.InvariantCulture) + "]";
	}
}
