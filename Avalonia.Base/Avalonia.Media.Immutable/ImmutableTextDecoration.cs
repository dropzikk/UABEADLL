namespace Avalonia.Media.Immutable;

public class ImmutableTextDecoration
{
	public TextDecorationLocation Location { get; }

	public ImmutablePen Pen { get; }

	public TextDecorationUnit PenThicknessUnit { get; }

	public double PenOffset { get; }

	public TextDecorationUnit PenOffsetUnit { get; }

	public ImmutableTextDecoration(TextDecorationLocation location, ImmutablePen pen, TextDecorationUnit penThicknessUnit, double penOffset, TextDecorationUnit penOffsetUnit)
	{
		Location = location;
		Pen = pen;
		PenThicknessUnit = penThicknessUnit;
		PenOffset = penOffset;
		PenOffsetUnit = penOffsetUnit;
	}
}
