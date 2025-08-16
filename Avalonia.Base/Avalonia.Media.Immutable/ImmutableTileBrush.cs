namespace Avalonia.Media.Immutable;

public abstract class ImmutableTileBrush : ITileBrush, IBrush, IImmutableBrush
{
	public AlignmentX AlignmentX { get; }

	public AlignmentY AlignmentY { get; }

	public RelativeRect DestinationRect { get; }

	public double Opacity { get; }

	public ITransform? Transform { get; }

	public RelativePoint TransformOrigin { get; }

	public RelativeRect SourceRect { get; }

	public Stretch Stretch { get; }

	public TileMode TileMode { get; }

	private protected ImmutableTileBrush(AlignmentX alignmentX, AlignmentY alignmentY, RelativeRect destinationRect, double opacity, ImmutableTransform? transform, RelativePoint transformOrigin, RelativeRect sourceRect, Stretch stretch, TileMode tileMode)
	{
		AlignmentX = alignmentX;
		AlignmentY = alignmentY;
		DestinationRect = destinationRect;
		Opacity = opacity;
		Transform = transform;
		TransformOrigin = transformOrigin;
		SourceRect = sourceRect;
		Stretch = stretch;
		TileMode = tileMode;
	}

	protected ImmutableTileBrush(ITileBrush source)
		: this(source.AlignmentX, source.AlignmentY, source.DestinationRect, source.Opacity, source.Transform?.ToImmutable(), source.TransformOrigin, source.SourceRect, source.Stretch, source.TileMode)
	{
	}
}
