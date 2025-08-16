namespace Avalonia.Utilities;

internal struct SpanRider
{
	private readonly SpanVector _spans;

	private SpanPosition _spanPosition;

	public int CurrentSpanStart => _spanPosition.Offset;

	public int Length { get; private set; }

	public int CurrentPosition { get; private set; }

	public object? CurrentElement
	{
		get
		{
			if (_spanPosition.Index < _spans.Count)
			{
				return _spans[_spanPosition.Index].element;
			}
			return _spans.Default;
		}
	}

	public int CurrentSpanIndex => _spanPosition.Index;

	public SpanPosition SpanPosition => _spanPosition;

	public SpanRider(SpanVector spans, SpanPosition latestPosition)
		: this(spans, latestPosition, latestPosition.Offset)
	{
	}

	public SpanRider(SpanVector spans, SpanPosition latestPosition = default(SpanPosition), int cp = 0)
	{
		_spans = spans;
		_spanPosition = default(SpanPosition);
		CurrentPosition = 0;
		Length = 0;
		At(latestPosition, cp);
	}

	public bool At(int cp)
	{
		return At(_spanPosition, cp);
	}

	public bool At(SpanPosition latestPosition, int cp)
	{
		bool num = _spans.FindSpan(cp, latestPosition, out _spanPosition);
		if (num)
		{
			Length = _spans[_spanPosition.Index].length - (cp - _spanPosition.Offset);
			CurrentPosition = cp;
			return num;
		}
		Length = int.MaxValue;
		CurrentPosition = _spanPosition.Offset;
		return num;
	}
}
