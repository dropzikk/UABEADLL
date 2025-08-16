using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
internal interface IMutableBrush : IBrush
{
	internal IImmutableBrush ToImmutable();
}
