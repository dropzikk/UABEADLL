namespace Avalonia.Utilities;

internal readonly struct SpanPosition
{
	internal int Index { get; }

	internal int Offset { get; }

	internal SpanPosition(int spanIndex, int spanOffset)
	{
		Index = spanIndex;
		Offset = spanOffset;
	}
}
