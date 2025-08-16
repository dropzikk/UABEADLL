using Avalonia.Media.Imaging;

namespace Avalonia.Media.Immutable;

internal class ImmutableImageBrush : ImmutableTileBrush, IImageBrush, ITileBrush, IBrush
{
	public IImageBrushSource? Source { get; }

	public ImmutableImageBrush(Bitmap? source, AlignmentX alignmentX = AlignmentX.Center, AlignmentY alignmentY = AlignmentY.Center, RelativeRect? destinationRect = null, double opacity = 1.0, ImmutableTransform? transform = null, RelativePoint transformOrigin = default(RelativePoint), RelativeRect? sourceRect = null, Stretch stretch = Stretch.Uniform, TileMode tileMode = TileMode.None)
		: base(alignmentX, alignmentY, destinationRect ?? RelativeRect.Fill, opacity, transform, transformOrigin, sourceRect ?? RelativeRect.Fill, stretch, tileMode)
	{
		Source = source;
	}

	public ImmutableImageBrush(IImageBrush source)
		: base(source)
	{
		Source = source.Source;
	}
}
