using Avalonia.Metadata;

namespace Avalonia.Media;

[NotClientImplementable]
public interface IImmutableSolidColorBrush : ISolidColorBrush, IBrush, IImmutableBrush
{
}
