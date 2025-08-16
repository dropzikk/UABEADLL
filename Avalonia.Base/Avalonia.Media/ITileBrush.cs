using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface ITileBrush : IBrush
{
	AlignmentX AlignmentX { get; }

	AlignmentY AlignmentY { get; }

	RelativeRect DestinationRect { get; }

	RelativeRect SourceRect { get; }

	Stretch Stretch { get; }

	TileMode TileMode { get; }
}
